#define DEVELOPMENT_BUILD

#include <csignal>
#include <chrono>
#include <iostream>
#include <string>

#include <grpcpp/grpcpp.h>
#include "BinaryStreamingClient.h"
#include "StreamingMessage.h"

using std::chrono::duration_cast;
using std::chrono::milliseconds;
using std::chrono::system_clock;
using BinaryStreamingApp::Client::BinaryStreamingClient;
using BinaryStreamingApp::Client::StreamingMessage;

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

            auto eventHandler = std::make_shared<std::function<void(const uint8_t* buffer, const size_t length)>>([&](const uint8_t* buffer, const size_t length){ this->OnReceivedMessageEventHandler(buffer, length); });
            client->AddEventHandler(eventHandler);

            client->ConnectAndForget();

            msgpack::sbuffer buffer;
            StreamingMessage streamingMessage;

            while (true)
            {
                std::cout << "Input message ('q' to quit) > ";
                std::string message;
                std::cin >> message;

                if (message == "q" || std::cin.eof()) break;
                if (message.empty()) continue;

                auto timestampMilliseconds = duration_cast<milliseconds>(system_clock::now().time_since_epoch()).count();

                streamingMessage.set_timestamp(timestampMilliseconds);
                streamingMessage.set_textmessage(message);
                msgpack::pack(buffer, streamingMessage);

                client->Send((uint8_t*)buffer.data(), buffer.size());

                buffer.clear();
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

        void OnReceivedMessageEventHandler(const uint8_t* buffer, size_t length)
        {
            msgpack::object_handle handle = msgpack::unpack((char*)buffer, length);
            msgpack::object obj(handle.get());

            StreamingMessage streamingMessage = obj.as<StreamingMessage>();

            time_t timestamp = streamingMessage.timestamp();

            std::cout << std::endl;
            std::cout << "-------------------------------------------" << std::endl;
            std::cout << "Received data size: " << length << std::endl;
            std::cout << "-------------------------------------------" << std::endl;
            std::cout << "Received message: " << std::endl;
            std::cout << "{" << std::endl;
            std::cout << "  Timestamp (Milliseconds): " << streamingMessage.timestamp() << std::endl;
            std::cout << "  TextMessage: " << streamingMessage.textmessage() << std::endl;
            std::cout << "}" << std::endl;
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