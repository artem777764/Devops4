using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Server.Protos; 

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientCert = new X509Certificate2("certs/client.pfx");

            var caCert = new X509Certificate2("certs/ca.pem");

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(clientCert);
            handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
            {
                chain.ChainPolicy.ExtraStore.Add(caCert);
                chain.ChainPolicy.VerificationFlags = System.Security.Cryptography.X509Certificates.X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                var isValid = chain.Build(cert);
                if (!isValid)
                {
                    Console.WriteLine("Сертификат сервера не доверенный!");
                    foreach (var status in chain.ChainStatus)
                    {
                        Console.WriteLine(status.StatusInformation);
                    }
                }

                return isValid;
            };

            var channel = GrpcChannel.ForAddress("https://localhost:5000", new GrpcChannelOptions
            {
                HttpHandler = handler
            });

            var client = new Greeter.GreeterClient(channel);

            Console.Write("Введите имя: ");
            var name = Console.ReadLine();

            var reply = await client.SayHelloAsync(new HelloRequest { Name = name });

            Console.WriteLine($"Ответ сервера: {reply.Message}");
        }
    }
}
