#define DEVELOPMENT_BUILD

#include <csignal>
#include <iostream>
#include <string>

#include <grpcpp/grpcpp.h>
#include "BinaryStreamingClient.h"

using BinaryStreamingApp::Client::BinaryStreamingClient;

class Startup
{
    public:
        Startup(const Startup &) = delete;
        Startup(Startup &&) = delete;
        Startup &operator=(const Startup &) = delete;
        Startup &operator=(Startup &&) = delete;

        // Return static local var reference is thread safe.
        static Startup& GetInstance()
        {
            static Startup instance;
            return instance;
        }

        void Initialize(std::string server_address)
        {
            client = new BinaryStreamingClient(grpc::CreateChannel(server_address, grpc::InsecureChannelCredentials()));

            auto eventHandler = std::make_shared<std::function<void(ByteBuffer &data)>>([&](ByteBuffer data){ this->OnReceivedMessageEventHandler(data); });
            client->AddEventHandler(eventHandler);

            client->ConnectAndForget();

            while (true)
            {
                std::cout << "Input message ('q' to quit) > ";
                std::string message;
                std::cin >> message;

                if (message == "q" || std::cin.eof()) break;
                if (message.empty()) continue;

                client->Send();
                std::this_thread::sleep_for(std::chrono::milliseconds(100));
            }

            client->RemoveEventHandler(eventHandler);
            client->Dispose();
        }

        void Dispose()
        {
#ifdef DEVELOPMENT_BUILD
            std::cout << "[Startup] Disposing" << std::endl;
#endif
            client->Dispose();
#ifdef DEVELOPMENT_BUILD
            std::cout << "[Startup] Disposed" << std::endl;
#endif
        }

        void OnReceivedMessageEventHandler(ByteBuffer data)
        {
            std::cout << std::endl;
            std::cout << "-------------------------------------------" << std::endl;
            std::cout << "[EventHandler] Received data size: " << data.Length() << std::endl;
            std::cout << "-------------------------------------------" << std::endl;
            std::cout << "Input message ('q' to quit) > ";
        }

    private:
        Startup() = default;
        ~Startup() = default;
        BinaryStreamingClient* client;
};

void SignalHandler(int signal)
{
    std::cout << std::endl;
    std::cout << "[SignalHandler] Get signal: " << signal << std::endl;
    Startup::GetInstance().Dispose();
    exit(1);
}

int main(int argc, char** argv)
{
    signal(SIGTERM, SignalHandler);
    signal(SIGINT, SignalHandler);

    std::cout << "///////////////////////////////////////////" << std::endl;
    std::cout << "///      gRPC code samples for C++      ///" << std::endl;
    std::cout << "///     binary streaming app client     ///" << std::endl;
    std::cout << "///////////////////////////////////////////" << std::endl;

    std::string server_address = "localhost:50051";
    Startup::GetInstance().Initialize(server_address);

    std::cout << std::endl;
    std::cout << "///////////////////////////////////////////" << std::endl;
    std::cout << "///  End of binary streaming app client ///" << std::endl;
    std::cout << "///////////////////////////////////////////" << std::endl;

    return 0;
}