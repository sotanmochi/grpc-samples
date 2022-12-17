#define DEVELOPMENT_BUILD

#include <csignal>
#include <iostream>
#include <string>

#include <grpcpp/grpcpp.h>
#include "ChatStreamingClient.h"

using ChatApp::Client::ChatClient;

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
            client = new ChatClient(grpc::CreateChannel(server_address, grpc::InsecureChannelCredentials()));

            auto eventHandler = std::make_shared<std::function<void(ChatMessage &message)>>([&](ChatMessage message){ this->OnReceivedMessageEventHandler(message); });
            client->AddEventHandler(eventHandler);

            std::string group, username;

            std::cout << "-------------------------------------------" << std::endl;
            std::cout << "Input chat group name > ";
            std::cin >> group;

            std::cout << "-------------------------------------------" << std::endl;
            std::cout << "Input username > ";
            std::cin >> username;
            std::cout << "-------------------------------------------" << std::endl;

            client->ConnectAndForget();

            while (true)
            {
                std::cout << "Input message ('q' to quit) > ";
                std::string message;
                std::cin >> message;

                if (message == "q" || message.empty()) break;

                client->SendMessage(group, username, message);
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

        void OnReceivedMessageEventHandler(ChatMessage chatMessage)
        {
            std::cout << std::endl;
            std::cout << "-------------------------------------------" << std::endl;
            std::cout << "Received Message: " << chatMessage.message() << std::endl;
            std::cout << "-------------------------------------------" << std::endl;
            std::cout << "Input message2 ('q' to quit) > ";
        }

    private:
        Startup() = default;
        ~Startup() = default;
        ChatClient* client;
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
    std::cout << "///           Chat app client           ///" << std::endl;
    std::cout << "///////////////////////////////////////////" << std::endl;

    std::string server_address = "localhost:50051";
    Startup::GetInstance().Initialize(server_address);

    std::cout << std::endl;
    std::cout << "///////////////////////////////////////////" << std::endl;
    std::cout << "///        End of chat app client       ///" << std::endl;
    std::cout << "///////////////////////////////////////////" << std::endl;

    return 0;
}