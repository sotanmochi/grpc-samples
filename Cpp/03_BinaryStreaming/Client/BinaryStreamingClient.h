#pragma once

#include <iostream>
#include <memory>
#include <string>
#include <thread>

#include "binarystreaming.grpc.pb.h"

using std::string;
using grpc::ByteBuffer;
using grpc::Channel;
using grpc::ClientContext;
using grpc::ClientReaderWriter;
using grpc::Slice;
using grpc::Status;
using binarystreaming::BinaryStreaming;

namespace BinaryStreamingApp::Client
{

class BinaryStreamingClient
{
    public:
        BinaryStreamingClient(std::shared_ptr<Channel> channel) : stub_(BinaryStreaming::NewStub(channel)) {}

        ~BinaryStreamingClient()
        {
#ifdef DEVELOPMENT_BUILD
            std::cout << std::endl;
            std::cout << "[BinaryStreamingClient] Destructor" << std::endl;
#endif
            Dispose();
        }

        void AddEventHandler(std::shared_ptr<std::function<void(ByteBuffer &data)>> eventHandler)
        {
            auto it = std::find(eventHandlers_.begin(), eventHandlers_.end(), eventHandler);
            if (it == eventHandlers_.end())
            {
                eventHandlers_.push_back(eventHandler);
            }
            else
            {
                std::cout << "[BinaryStreamingClient] The event handler has already been added." << std::endl;
            }
        }

        void RemoveEventHandler(std::shared_ptr<std::function<void(ByteBuffer &data)>> eventHandler)
        {
            auto end = std::remove_if(eventHandlers_.begin(), eventHandlers_.end(), 
                                        [eventHandler](std::shared_ptr<std::function<void(ByteBuffer &data)>> func) -> bool 
                                        {
                                            return (func == eventHandler);
                                        });
            eventHandlers_.erase(end, eventHandlers_.end());
        }

        void ConnectAndForget()
        {            
            // std::cout << "-----" << std::endl;
            // const std::thread::id threadId = std::this_thread::get_id();
            // std::cout << "[BinaryStreamingClient] ConnectAndForget thread id: " << threadId << std::endl;
            // std::cout << "-----" << std::endl;

            // Fire and Forget
            std::thread thread([this]() { Connect(); });
            thread.detach();

#ifdef DEVELOPMENT_BUILD
            std::cout << "[BinaryStreamingClient] ConnectAndForget function has finished." << std::endl;
#endif
        }

        void Connect()
        {
            // std::cout << "-----" << std::endl;
            // const std::thread::id threadId = std::this_thread::get_id();
            // std::cout << "[BinaryStreamingClient] Connect thread id: " << threadId << std::endl;
            // std::cout << "-----" << std::endl;

            stream_ = stub_->Connect(&context_);

            // Main loop
            ByteBuffer response;
            while (stream_->Read(&response))
            {
                Slice slice;
                if (!response.TrySingleSlice(&slice).ok())
                {
                    if (!response.DumpToSingleSlice(&slice).ok())
                    {
                        std::cout << "[BinaryStreaming] No payload" << std::endl;
                    }
                }

                std::cout << "[BinaryStreaming] Received data size: " << slice.size() << std::endl;

                for (auto& func: eventHandlers_)
                {
                    (*func)(response);
                }
            }

            Status status = stream_->Finish();
            std::cout << "[BinaryStreamingClient] Stream has finished." << std::endl;
            if (!status.ok())
            {
                std::cout << "[BinaryStreamingClient] Error Code: " << status.error_code() << std::endl;
                std::cout << "[BinaryStreamingClient] Error Message: " << status.error_message() << std::endl;
            }

#ifdef DEVELOPMENT_BUILD
            std::cout << "[BinaryStreamingClient] Connect function has finished." << std::endl;
#endif
        }

        void Send()
        {
            std::cout << "[BinaryStreamingClient] Send binary data (Work In Progress)." << std::endl;
            // ByteBuffer request;
            // stream_->Write(request);
        }

        void Dispose()
        {
            if (disposed_)
            {
#ifdef DEVELOPMENT_BUILD
            std::cout << std::endl;
            std::cout << "[BinaryStreamingClient] Already disposed." << std::endl;
#endif
                return;
            }

            disposed_ = true;
            context_.TryCancel();

#ifdef DEVELOPMENT_BUILD
            std::cout << std::endl;
            std::cout << "[BinaryStreamingClient] Disposed." << std::endl;
#endif
        }

    private:
        bool disposed_ = false;
        ClientContext context_;
        std::unique_ptr<BinaryStreaming::Stub> stub_;
        std::unique_ptr<ClientReaderWriter<ByteBuffer, ByteBuffer>> stream_;
        std::list<std::shared_ptr<std::function<void(ByteBuffer &data)>>> eventHandlers_;
};
}