using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Opc.UaFx.Client;

class Program 
{
    static async Task GerarNumerosAleatorios(int intevaloMs, CancellationToken token)
    {
        Random random = new Random();

        string opcUrl = "opc.tcp://desktop-15o0c8q:26543/BatchPlantServer";
        string[] tagName = { "ns=2;i=336", "ns=2;i=342", "ns=2;i=360", "ns=2;i=366", "ns=2;i=384", "ns=2;i=390", "ns=2;i=407", "ns=2;i=414", "ns=2;i=421", "ns=2;i=427" };


        int i = 0;
        var client = new OpcClient(opcUrl);
        client.Connect();

        while (!token.IsCancellationRequested)
        {
            double numeroAleatorio = random.Next(1, 10000);
            Console.WriteLine($"Número gerado:{numeroAleatorio} para {tagName[i]}");

            client.WriteNode(tagName[i], numeroAleatorio);
            i++;

            try
            {
                await Task.Delay(intevaloMs, token);
            }
            catch(TaskCanceledException)
            {
                Console.WriteLine("A geração de numeros foi cancelada");
                break;
            }

            if (i == 10)
                i = 0;
        }

        client.Disconnect();
    }

    static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        Console.WriteLine("Gerando numeros aleatorios a cada 2 segundos...");
        Task tarefa = GerarNumerosAleatorios(2000, cts.Token);

        Console.WriteLine("Pressiona Enter para parar.");
        Console.ReadLine();

        cts.Cancel();
        await tarefa;

    }
}


