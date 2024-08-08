using HotelManagerSystem.Models;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        List<Pessoa> hospedes = new List<Pessoa>();

        var p1 = new Pessoa(nome: "Hóspede 1");
        var p2 = new Pessoa(nome: "Hóspede 2");

        hospedes.Add(p1);
        hospedes.Add(p2);

        Suite suite = new Suite(tipoSuite: "Premium", capacidade: 2, valorDiaria: 30);

        var reserva = new Reserva(diasReservados: 5);
        reserva.CadastrarSuite(suite);
        reserva.CadastrarHospedes(hospedes);

        Console.WriteLine($"Hóspedes: {reserva.ObterQuantidadeHospedes()}");
        Console.WriteLine($"Valor diária: {reserva.CalcularValorDiaria()}");
    }
}