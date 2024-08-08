using AbstractingCellphone.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        Nokia nokia = new Nokia(numero: "123456789", modelo: "Nokia 3310", imei: "111111111111111", memoria: 64);
        Iphone iphone = new Iphone(numero: "987654321", modelo: "iPhone 12", imei: "222222222222222", memoria: 128);

        nokia.Ligar();
        nokia.ReceberLigacao();
        nokia.InstalarAplicativo("WhatsApp");

        iphone.Ligar();
        iphone.ReceberLigacao();
        iphone.InstalarAplicativo("Instagram");

        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}