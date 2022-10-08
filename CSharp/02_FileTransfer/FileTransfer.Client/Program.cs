using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace FileTransferApp.Client;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"///////////////////////////////////////////");
        Console.WriteLine($"///          gRPC code samples          ///");
        Console.WriteLine($"///       File transfer app client      ///");
        Console.WriteLine($"///////////////////////////////////////////");
        
        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("http://localhost:5247");
        // using var channel = GrpcChannel.ForAddress("https://localhost:7179");
        
        Console.WriteLine($"-------------------------------------------");
        
        var startup = new Startup(channel);
        await startup.StartAsync("./Storage/Downloads");
        startup.Dispose();
        
        Console.WriteLine($"///////////////////////////////////////////");
        Console.WriteLine($"///   End of file transfer app client   ///");
        Console.WriteLine($"///////////////////////////////////////////");
    }
}