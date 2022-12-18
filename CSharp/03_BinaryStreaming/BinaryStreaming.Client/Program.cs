using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace BinaryStreaming.Client;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"///////////////////////////////////////////");
        Console.WriteLine($"///          gRPC code samples          ///");
        Console.WriteLine($"///     Binary streaming app client     ///");
        Console.WriteLine($"///////////////////////////////////////////");
        
        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("http://localhost:50051");
        // using var channel = GrpcChannel.ForAddress("https://localhost:50052");
        
        Console.WriteLine($"-------------------------------------------");
        
        var startup = new Startup(channel);
        await startup.StartAsync();
        startup.Dispose();
        
        Console.WriteLine($"//////////////////////////////////////////////");
        Console.WriteLine($"///   End of binary streaming app client   ///");
        Console.WriteLine($"//////////////////////////////////////////////");
    }
}