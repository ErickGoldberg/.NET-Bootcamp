# Criando Máquinas Virtuais na Azure

## O que é uma Máquina Virtual (VM) na Azure?

Uma **Máquina Virtual (VM)** na Azure é um recurso de computação que simula um computador físico, mas é totalmente hospedado na nuvem. As VMs são usadas para rodar aplicativos, hospedar sites, testar ambientes, entre outros usos. Elas oferecem a flexibilidade de escalar o hardware e os recursos de acordo com as necessidades do seu negócio.

## Passos para Criar uma Máquina Virtual na Azure

### 1. Acessar o Portal do Azure

- Acesse o [Portal do Azure](https://portal.azure.com).
- Faça login com suas credenciais.

### 2. Iniciar a Criação de uma Nova VM

- No painel do Azure, clique em **Criar um recurso**.
- Selecione **Máquina Virtual** na seção de Computação.

### 3. Configurar as Opções Básicas

Na aba **Básico**, você deve configurar as seguintes opções:

- **Assinatura**: Selecione a assinatura do Azure que será usada para a VM.
- **Grupo de Recursos**: Escolha um grupo de recursos existente ou crie um novo.
- **Nome da VM**: Dê um nome para a sua máquina virtual.
- **Região**: Selecione a região onde a VM será hospedada. A escolha da região pode impactar a latência e a conformidade legal.
- **Opções de Disponibilidade**: Escolha entre Conjunto de Disponibilidade ou Zona de Disponibilidade para garantir a alta disponibilidade.
- **Imagem**: Escolha o sistema operacional que será instalado na VM (Windows, Linux, etc.).
- **Tamanho da VM**: Selecione o tamanho da VM (CPU, memória, etc.), baseado nas necessidades de sua aplicação.

### 4. Configurar as Credenciais

- **Nome de Usuário**: Defina o nome do usuário administrador.
- **Autenticação**: Escolha entre senha ou chave SSH para autenticação.

### 5. Configurar o Disco

- **Tipo de Disco**: Escolha o tipo de disco (SSD, HDD) para o armazenamento do sistema operacional.
- **Tamanho do Disco**: Defina o tamanho do disco, dependendo dos requisitos de armazenamento.

### 6. Configurar Rede

Na aba **Rede**, você pode:

- **Rede Virtual**: Selecione ou crie uma rede virtual onde a VM será implantada.
- **Sub-rede**: Escolha a sub-rede dentro da rede virtual.
- **Endereço IP Público**: Decida se a VM terá um endereço IP público.
- **Grupo de Segurança de Rede**: Configure regras de firewall para controlar o tráfego de entrada e saída.

### 7. Configurar Opções de Gerenciamento

- **Monitoramento**: Ative ou desative o monitoramento da VM com o Azure Monitor.
- **Backup**: Configure o backup automático da VM para proteger seus dados.

### 8. Revisar e Criar

- Revise todas as configurações.
- Clique em **Criar** para iniciar o processo de criação da VM.

### 9. Acessar a Máquina Virtual

Depois que a VM for criada:

- Acesse-a via **RDP** (para Windows) ou **SSH** (para Linux) usando o endereço IP público ou o nome DNS.

## Considerações Finais

- **Custos**: As VMs são cobradas com base em tempo de uso e recursos alocados. Monitore os custos no portal da Azure.
- **Escalabilidade**: Você pode redimensionar (scale up/down) sua VM conforme necessário.
- **Segurança**: Use as boas práticas de segurança, como restrição de IPs, autenticação multifator e criptografia de dados.

Criar uma máquina virtual na Azure oferece flexibilidade e controle sobre o ambiente de execução das suas aplicações, com a vantagem de escalar conforme as necessidades do seu negócio.
