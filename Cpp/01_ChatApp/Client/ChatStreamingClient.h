#pragma once

#include <iostream>
#include <memory>
#include <string>
#include <thread>

#include "chat.grpc.pb.h"

using std::string;
using grpc::Channel;
using grpc::ClientContext;
using grpc::ClientReaderWriter;
using grpc::Status;
using chat::Chat;
using chat::ChatMessage;

namespace ChatApp::Client
{

class ChatClient
{
    public:
        ChatClient(std::shared_ptr<Channel> channel) : stub_(Chat::NewStub(channel)) {}

        ~ChatClient()
        {
#ifdef DEVELOPMENT_BUILD
            std::cout << std::endl;
            std::cout << "[ChatClient] Destructor" << std::endl;
#endif
            Dispose();
        }

        void AddEventHandler(std::shared_ptr<std::function<void(ChatMessage &message)>> eventHandler)
        {
            auto it = std::find(eventHandlers_.begin(), eventHandlers_.end(), eventHandler);
            if (it == eventHandlers_.end())
            {
                eventHandlers_.push_back(eventHandler);
            }
            else
            {
                std::cout << "[ChatClient] The event handler has already been added." << std::endl;
            }
        }

        void RemoveEventHandler(std::shared_ptr<std::function<void(ChatMessage &message)>> eventHandler)
        {
            auto end = std::remove_if(eventHandlers_.begin(), eventHandlers_.end(), 
                                        [eventHandler](std::shared_ptr<std::function<void(ChatMessage &message)>> func) -> bool 
                                        {
                                            return (func == eventHandler);
                                        });
            eventHandlers_.erase(end, eventHandlers_.end());
        }

        void ConnectAndForget()
        {            
            // std::cout << "-----" << std::endl;
            // const std::thread::id threadId = std::this_thread::get_id();
            // std::cout << "[ChatClient] ConnectAndForget thread id: " << threadId << std::endl;
            // std::cout << "-----" << std::endl;

            // Fire and Forget
            std::thread thread([this]() { Connect(); });
            thread.detach();

#ifdef DEVELOPMENT_BUILD
            std::cout << "[ChatClient] ConnectAndForget function has finished." << std::endl;
#endif
        }

        void Connect()
        {
            // std::cout << "-----" << std::endl;
            // const std::thread::id threadId = std::this_thread::get_id();
            // std::cout << "[ChatClient] Connect thread id: " << threadId << std::endl;
            // std::cout << "-----" << std::endl;

            stream_ = stub_->Connect(&context_);

            // Main loop
            ChatMessage response;
            while (stream_->Read(&response))
            {
                for (auto& func: eventHandlers_)
                {
                    (*func)(response);
                }
            }

            Status status = stream_->Finish();
            std::cout << "[ChatClient] Stream has finished." << std::endl;
            if (!status.ok())
            {
                std::cout << "[ChatClient] Error Code: " << status.error_code() << std::endl;
                std::cout << "[ChatClient] Error Message: " << status.error_message() << std::endl;
            }

#ifdef DEVELOPMENT_BUILD
            std::cout << "[ChatClient] Connect function has finished." << std::endl;
#endif
        }

        void SendMessage(string groupName, string username, string message)
        {
            ChatMessage request;
            request.set_groupname(groupName);
            request.set_username(username);
            request.set_message(message);

            stream_->Write(request);
        }

        void Dispose()
        {
            if (disposed_)
            {
#ifdef DEVELOPMENT_BUILD
            std::cout << std::endl;
            std::cout << "[ChatClient] Already disposed." << std::endl;
#endif
                return;
            }

            disposed_ = true;
            context_.TryCancel();

#ifdef DEVELOPMENT_BUILD
            std::cout << std::endl;
            std::cout << "[ChatClient] Disposed." << std::endl;
#endif
        }

    private:
        bool disposed_ = false;
        ClientContext context_;
        std::unique_ptr<Chat::Stub> stub_;
        std::unique_ptr<ClientReaderWriter<ChatMessage, ChatMessage>> stream_;
        std::list<std::shared_ptr<std::function<void(ChatMessage &message)>>> eventHandlers_;
};
}