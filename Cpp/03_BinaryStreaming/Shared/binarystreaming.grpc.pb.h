#ifndef GRPC_binarystreaming_2eproto__INCLUDED
#define GRPC_binarystreaming_2eproto__INCLUDED

#include <functional>
#include <grpcpp/impl/codegen/async_generic_service.h>
#include <grpcpp/impl/codegen/async_stream.h>
#include <grpcpp/impl/codegen/async_unary_call.h>
#include <grpcpp/impl/codegen/client_callback.h>
#include <grpcpp/impl/codegen/client_context.h>
#include <grpcpp/impl/codegen/completion_queue.h>
#include <grpcpp/impl/codegen/message_allocator.h>
#include <grpcpp/impl/codegen/method_handler.h>
#include <grpcpp/impl/codegen/proto_utils.h>
#include <grpcpp/impl/codegen/rpc_method.h>
#include <grpcpp/impl/codegen/server_callback.h>
#include <grpcpp/impl/codegen/server_callback_handlers.h>
#include <grpcpp/impl/codegen/server_context.h>
#include <grpcpp/impl/codegen/service_type.h>
#include <grpcpp/impl/codegen/status.h>
#include <grpcpp/impl/codegen/stub_options.h>
#include <grpcpp/impl/codegen/sync_stream.h>

namespace binarystreaming {

class BinaryStreaming final {
 public:
  static constexpr char const* service_full_name() {
    // return "binarystreaming.BinaryStreaming";
    return "BinaryStreamingGrpc";
  }
  class StubInterface {
   public:
    virtual ~StubInterface() {}
    std::unique_ptr< ::grpc::ClientReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>> Connect(::grpc::ClientContext* context) {
      return std::unique_ptr< ::grpc::ClientReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>>(ConnectRaw(context));
    }
    std::unique_ptr< ::grpc::ClientAsyncReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>> AsyncConnect(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq, void* tag) {
      return std::unique_ptr< ::grpc::ClientAsyncReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>>(AsyncConnectRaw(context, cq, tag));
    }
    std::unique_ptr< ::grpc::ClientAsyncReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>> PrepareAsyncConnect(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq) {
      return std::unique_ptr< ::grpc::ClientAsyncReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>>(PrepareAsyncConnectRaw(context, cq));
    }
    class async_interface {
     public:
      virtual ~async_interface() {}
      virtual void Connect(::grpc::ClientContext* context, ::grpc::ClientBidiReactor< ::grpc::ByteBuffer,::grpc::ByteBuffer>* reactor) = 0;
    };
    typedef class async_interface experimental_async_interface;
    virtual class async_interface* async() { return nullptr; }
    class async_interface* experimental_async() { return async(); }
   private:
    virtual ::grpc::ClientReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* ConnectRaw(::grpc::ClientContext* context) = 0;
    virtual ::grpc::ClientAsyncReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* AsyncConnectRaw(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq, void* tag) = 0;
    virtual ::grpc::ClientAsyncReaderWriterInterface< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* PrepareAsyncConnectRaw(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq) = 0;
  };
  class Stub final : public StubInterface {
   public:
    Stub(const std::shared_ptr< ::grpc::ChannelInterface>& channel, const ::grpc::StubOptions& options = ::grpc::StubOptions());
    std::unique_ptr< ::grpc::ClientReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>> Connect(::grpc::ClientContext* context) {
      return std::unique_ptr< ::grpc::ClientReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>>(ConnectRaw(context));
    }
    std::unique_ptr<  ::grpc::ClientAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>> AsyncConnect(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq, void* tag) {
      return std::unique_ptr< ::grpc::ClientAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>>(AsyncConnectRaw(context, cq, tag));
    }
    std::unique_ptr<  ::grpc::ClientAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>> PrepareAsyncConnect(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq) {
      return std::unique_ptr< ::grpc::ClientAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>>(PrepareAsyncConnectRaw(context, cq));
    }
    class async final :
      public StubInterface::async_interface {
     public:
      void Connect(::grpc::ClientContext* context, ::grpc::ClientBidiReactor< ::grpc::ByteBuffer,::grpc::ByteBuffer>* reactor) override;
     private:
      friend class Stub;
      explicit async(Stub* stub): stub_(stub) { }
      Stub* stub() { return stub_; }
      Stub* stub_;
    };
    class async* async() override { return &async_stub_; }

   private:
    std::shared_ptr< ::grpc::ChannelInterface> channel_;
    class async async_stub_{this};
    ::grpc::ClientReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* ConnectRaw(::grpc::ClientContext* context) override;
    ::grpc::ClientAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* AsyncConnectRaw(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq, void* tag) override;
    ::grpc::ClientAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* PrepareAsyncConnectRaw(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq) override;
    const ::grpc::internal::RpcMethod rpcmethod_Connect_;
  };
  static std::unique_ptr<Stub> NewStub(const std::shared_ptr< ::grpc::ChannelInterface>& channel, const ::grpc::StubOptions& options = ::grpc::StubOptions());

  class Service : public ::grpc::Service {
   public:
    Service();
    virtual ~Service();
    virtual ::grpc::Status Connect(::grpc::ServerContext* context, ::grpc::ServerReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* stream);
  };
  template <class BaseClass>
  class WithAsyncMethod_Connect : public BaseClass {
   private:
    void BaseClassMustBeDerivedFromService(const Service* /*service*/) {}
   public:
    WithAsyncMethod_Connect() {
      ::grpc::Service::MarkMethodAsync(0);
    }
    ~WithAsyncMethod_Connect() override {
      BaseClassMustBeDerivedFromService(this);
    }
    // disable synchronous version of this method
    ::grpc::Status Connect(::grpc::ServerContext* /*context*/, ::grpc::ServerReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* /*stream*/)  override {
      abort();
      return ::grpc::Status(::grpc::StatusCode::UNIMPLEMENTED, "");
    }
    void RequestConnect(::grpc::ServerContext* context, ::grpc::ServerAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* stream, ::grpc::CompletionQueue* new_call_cq, ::grpc::ServerCompletionQueue* notification_cq, void *tag) {
      ::grpc::Service::RequestAsyncBidiStreaming(0, context, stream, new_call_cq, notification_cq, tag);
    }
  };
  typedef WithAsyncMethod_Connect<Service > AsyncService;
  template <class BaseClass>
  class WithCallbackMethod_Connect : public BaseClass {
   private:
    void BaseClassMustBeDerivedFromService(const Service* /*service*/) {}
   public:
    WithCallbackMethod_Connect() {
      ::grpc::Service::MarkMethodCallback(0,
          new ::grpc::internal::CallbackBidiHandler< ::grpc::ByteBuffer, ::grpc::ByteBuffer>(
            [this](
                   ::grpc::CallbackServerContext* context) { return this->Connect(context); }));
    }
    ~WithCallbackMethod_Connect() override {
      BaseClassMustBeDerivedFromService(this);
    }
    // disable synchronous version of this method
    ::grpc::Status Connect(::grpc::ServerContext* /*context*/, ::grpc::ServerReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* /*stream*/)  override {
      abort();
      return ::grpc::Status(::grpc::StatusCode::UNIMPLEMENTED, "");
    }
    virtual ::grpc::ServerBidiReactor< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* Connect(
      ::grpc::CallbackServerContext* /*context*/)
      { return nullptr; }
  };
  typedef WithCallbackMethod_Connect<Service > CallbackService;
  typedef CallbackService ExperimentalCallbackService;
  template <class BaseClass>
  class WithGenericMethod_Connect : public BaseClass {
   private:
    void BaseClassMustBeDerivedFromService(const Service* /*service*/) {}
   public:
    WithGenericMethod_Connect() {
      ::grpc::Service::MarkMethodGeneric(0);
    }
    ~WithGenericMethod_Connect() override {
      BaseClassMustBeDerivedFromService(this);
    }
    // disable synchronous version of this method
    ::grpc::Status Connect(::grpc::ServerContext* /*context*/, ::grpc::ServerReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* /*stream*/)  override {
      abort();
      return ::grpc::Status(::grpc::StatusCode::UNIMPLEMENTED, "");
    }
  };
  template <class BaseClass>
  class WithRawMethod_Connect : public BaseClass {
   private:
    void BaseClassMustBeDerivedFromService(const Service* /*service*/) {}
   public:
    WithRawMethod_Connect() {
      ::grpc::Service::MarkMethodRaw(0);
    }
    ~WithRawMethod_Connect() override {
      BaseClassMustBeDerivedFromService(this);
    }
    // disable synchronous version of this method
    ::grpc::Status Connect(::grpc::ServerContext* /*context*/, ::grpc::ServerReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* /*stream*/)  override {
      abort();
      return ::grpc::Status(::grpc::StatusCode::UNIMPLEMENTED, "");
    }
    void RequestConnect(::grpc::ServerContext* context, ::grpc::ServerAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* stream, ::grpc::CompletionQueue* new_call_cq, ::grpc::ServerCompletionQueue* notification_cq, void *tag) {
      ::grpc::Service::RequestAsyncBidiStreaming(0, context, stream, new_call_cq, notification_cq, tag);
    }
  };
  template <class BaseClass>
  class WithRawCallbackMethod_Connect : public BaseClass {
   private:
    void BaseClassMustBeDerivedFromService(const Service* /*service*/) {}
   public:
    WithRawCallbackMethod_Connect() {
      ::grpc::Service::MarkMethodRawCallback(0,
          new ::grpc::internal::CallbackBidiHandler< ::grpc::ByteBuffer, ::grpc::ByteBuffer>(
            [this](
                   ::grpc::CallbackServerContext* context) { return this->Connect(context); }));
    }
    ~WithRawCallbackMethod_Connect() override {
      BaseClassMustBeDerivedFromService(this);
    }
    // disable synchronous version of this method
    ::grpc::Status Connect(::grpc::ServerContext* /*context*/, ::grpc::ServerReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* /*stream*/)  override {
      abort();
      return ::grpc::Status(::grpc::StatusCode::UNIMPLEMENTED, "");
    }
    virtual ::grpc::ServerBidiReactor< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* Connect(
      ::grpc::CallbackServerContext* /*context*/)
      { return nullptr; }
  };
  typedef Service StreamedUnaryService;
  typedef Service SplitStreamedService;
  typedef Service StreamedService;
};

}  // namespace binarystreaming


#endif  // GRPC_binarystreaming_2eproto__INCLUDED
