using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace ChatApp.Client;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"///////////////////////////////////////////");
        Console.WriteLine($"///          gRPC code samples          ///");
        Console.WriteLine($"///           Chat app client           ///");
        Console.WriteLine($"///////////////////////////////////////////");

        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("http://localhost:5247");
        // using var channel = GrpcChannel.ForAddress("https://localhost:7179");

        Console.WriteLine($"-------------------------------------------");
        Console.Write("Input chat group name: ");
        var group = Console.ReadLine();
        if (string.IsNullOrEmpty(group))
        {
            Console.WriteLine("Group name is null or empty.");
            return;
        }

        Console.Write("Input username: ");
        var username = Console.ReadLine();
        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine("Username is null or empty.");
            return;
        }

        Console.WriteLine($"-------------------------------------------");
        var startup = new Startup(channel);
        await startup.StartAsync(group, username);
        await startup.DisposeAsync();

        Console.WriteLine($"///////////////////////////////////////////");
        Console.WriteLine($"///        End of chat app client       ///");
        Console.WriteLine($"///////////////////////////////////////////");
    }
}