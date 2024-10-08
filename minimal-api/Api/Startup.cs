using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private readonly string key = "";
    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureAuthentication(services);
        ConfigureAuthorization(services);

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        ConfigureSwagger(services);

        services.AddDbContext<DbContexto>(options =>
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
            ));

        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()));
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
    }

    private void ConfigureAuthorization(IServiceCollection services)
    {
        services.AddAuthorization();
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            ConfigureHomeEndpoints(endpoints);
            ConfigureAdministradoresEndpoints(endpoints);
            ConfigureVeiculosEndpoints(endpoints);
        });
    }

    private void ConfigureHomeEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => Results.Json(new Home()))
                 .AllowAnonymous()
                 .WithTags("Home");
    }

    private void ConfigureAdministradoresEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/administradores/login", HandleAdministradorLogin)
                 .AllowAnonymous()
                 .WithTags("Administradores");

        endpoints.MapGet("/administradores", HandleGetAdministradores)
                 .RequireAuthorization()
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                 .WithTags("Administradores");

        endpoints.MapGet("/Administradores/{id}", HandleGetAdministradorById)
                 .RequireAuthorization()
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                 .WithTags("Administradores");

        endpoints.MapPost("/administradores", HandleCreateAdministrador)
                 .RequireAuthorization()
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                 .WithTags("Administradores");
    }

    private void ConfigureVeiculosEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/veiculos", HandleCreateVeiculo)
                 .RequireAuthorization()
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
                 .WithTags("Veiculos");

        endpoints.MapGet("/veiculos", HandleGetVeiculos)
                 .RequireAuthorization()
                 .WithTags("Veiculos");

        endpoints.MapGet("/veiculos/{id}", HandleGetVeiculoById)
                 .RequireAuthorization()
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
                 .WithTags("Veiculos");

        endpoints.MapPut("/veiculos/{id}", HandleUpdateVeiculo)
                 .RequireAuthorization()
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                 .WithTags("Veiculos");

        endpoints.MapDelete("/veiculos/{id}", HandleDeleteVeiculo)
                 .RequireAuthorization()
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                 .WithTags("Veiculos");
    }

    private string GerarTokenJwt(Administrador administrador)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("Email", administrador.Email),
            new Claim("Perfil", administrador.Perfil),
            new Claim(ClaimTypes.Role, administrador.Perfil),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private IResult HandleAdministradorLogin([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico)
    {
        var adm = administradorServico.Login(loginDTO);
        if (adm == null) return Results.Unauthorized();

        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdministradorLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }

    private IResult HandleGetAdministradores([FromQuery] int? pagina, IAdministradorServico administradorServico)
    {
        var adms = administradorServico.Todos(pagina)
                                      .Select(adm => new AdministradorModelView
                                      {
                                          Id = adm.Id,
                                          Email = adm.Email,
                                          Perfil = adm.Perfil
                                      })
                                      .ToList();
        return Results.Ok(adms);
    }

    private IResult HandleGetAdministradorById(int id, IAdministradorServico administradorServico)
    {
        var administrador = administradorServico.BuscaPorId(id);
        if (administrador == null) return Results.NotFound();

        return Results.Ok(new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
    }

    private IResult HandleCreateAdministrador([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico)
    {
        var validacao = ValidateAdministradorDTO(administradorDTO);
        if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

        var administrador = new Administrador
        {
            Email = administradorDTO.Email,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
        };

        administradorServico.Incluir(administrador);

        return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
    }

    private ErrosDeValidacao ValidateAdministradorDTO(AdministradorDTO administradorDTO)
    {
        var validacao = new ErrosDeValidacao { Mensagens = new List<string>() };

        if (string.IsNullOrEmpty(administradorDTO.Email))
            validacao.Mensagens.Add("Email não pode ser vazio");
        if (string.IsNullOrEmpty(administradorDTO.Senha))
            validacao.Mensagens.Add("Senha não pode ser vazia");
        if (administradorDTO.Perfil == null)
            validacao.Mensagens.Add("Perfil não pode ser vazio");

        return validacao;
    }

    private IResult HandleCreateVeiculo([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico)
    {
        var validacao = ValidateVeiculoDTO(veiculoDTO);
        if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

        var veiculo = new Veiculo
        {
            Nome = veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Ano = veiculoDTO.Ano
        };

        veiculoServico.Incluir(veiculo);

        return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
    }

    private IResult HandleGetVeiculos([FromQuery] int? pagina, IVeiculoServico veiculoServico)
    {
        var veiculos = veiculoServico.Todos(pagina);
        return Results.Ok(veiculos);
    }

    private IResult HandleGetVeiculoById(int id, IVeiculoServico veiculoServico)
    {
        var veiculo = veiculoServico.BuscaPorId(id);
        if (veiculo == null) return Results.NotFound();

        return Results.Ok(veiculo);
    }

    private IResult HandleUpdateVeiculo(int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico)
    {
        var veiculo = veiculoServico.BuscaPorId(id);
        if (veiculo == null) return Results.NotFound();

        var validacao = ValidateVeiculoDTO(veiculoDTO);
        if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;
        veiculo.Ano = veiculoDTO.Ano;

        veiculoServico.Atualizar(veiculo);

        return Results.Ok(veiculo);
    }

    private IResult HandleDeleteVeiculo(int id, IVeiculoServico veiculoServico)
    {
        var veiculo = veiculoServico.BuscaPorId(id);
        if (veiculo == null) return Results.NotFound();

        veiculoServico.Apagar(veiculo);

        return Results.NoContent();
    }

    private ErrosDeValidacao ValidateVeiculoDTO(VeiculoDTO veiculoDTO)
    {
        var validacao = new ErrosDeValidacao { Mensagens = new List<string>() };

        if (string.IsNullOrEmpty(veiculoDTO.Nome))
            validacao.Mensagens.Add("O nome não pode ser vazio");

        if (string.IsNullOrEmpty(veiculoDTO.Marca))
            validacao.Mensagens.Add("A Marca não pode ficar em branco");

        if (veiculoDTO.Ano < 1950)
            validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950");

        return validacao;
    }
}
