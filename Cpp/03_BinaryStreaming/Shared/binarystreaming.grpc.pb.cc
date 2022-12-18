#include "binarystreaming.grpc.pb.h"

#include <functional>
#include <grpcpp/impl/codegen/async_stream.h>
#include <grpcpp/impl/codegen/async_unary_call.h>
#include <grpcpp/impl/codegen/channel_interface.h>
#include <grpcpp/impl/codegen/client_unary_call.h>
#include <grpcpp/impl/codegen/client_callback.h>
#include <grpcpp/impl/codegen/message_allocator.h>
#include <grpcpp/impl/codegen/method_handler.h>
#include <grpcpp/impl/codegen/rpc_service_method.h>
#include <grpcpp/impl/codegen/server_callback.h>
#include <grpcpp/impl/codegen/server_callback_handlers.h>
#include <grpcpp/impl/codegen/server_context.h>
#include <grpcpp/impl/codegen/service_type.h>
#include <grpcpp/impl/codegen/sync_stream.h>
namespace binarystreaming {

static const char* BinaryStreaming_method_names[] = {
  // "/binarystreaming.BinaryStreaming/Connect",
  "/BinaryStreamingGrpc/Connect",
};

std::unique_ptr< BinaryStreaming::Stub> BinaryStreaming::NewStub(const std::shared_ptr< ::grpc::ChannelInterface>& channel, const ::grpc::StubOptions& options) {
  (void)options;
  std::unique_ptr< BinaryStreaming::Stub> stub(new BinaryStreaming::Stub(channel, options));
  return stub;
}

BinaryStreaming::Stub::Stub(const std::shared_ptr< ::grpc::ChannelInterface>& channel, const ::grpc::StubOptions& options)
  : channel_(channel), rpcmethod_Connect_(BinaryStreaming_method_names[0], options.suffix_for_stats(),::grpc::internal::RpcMethod::BIDI_STREAMING, channel)
  {}

::grpc::ClientReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* BinaryStreaming::Stub::ConnectRaw(::grpc::ClientContext* context) {
  return ::grpc::internal::ClientReaderWriterFactory< ::grpc::ByteBuffer, ::grpc::ByteBuffer>::Create(channel_.get(), rpcmethod_Connect_, context);
}

void BinaryStreaming::Stub::async::Connect(::grpc::ClientContext* context, ::grpc::ClientBidiReactor< ::grpc::ByteBuffer,::grpc::ByteBuffer>* reactor) {
  ::grpc::internal::ClientCallbackReaderWriterFactory< ::grpc::ByteBuffer,::grpc::ByteBuffer>::Create(stub_->channel_.get(), stub_->rpcmethod_Connect_, context, reactor);
}

::grpc::ClientAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* BinaryStreaming::Stub::AsyncConnectRaw(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq, void* tag) {
  return ::grpc::internal::ClientAsyncReaderWriterFactory< ::grpc::ByteBuffer, ::grpc::ByteBuffer>::Create(channel_.get(), cq, rpcmethod_Connect_, context, true, tag);
}

::grpc::ClientAsyncReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* BinaryStreaming::Stub::PrepareAsyncConnectRaw(::grpc::ClientContext* context, ::grpc::CompletionQueue* cq) {
  return ::grpc::internal::ClientAsyncReaderWriterFactory< ::grpc::ByteBuffer, ::grpc::ByteBuffer>::Create(channel_.get(), cq, rpcmethod_Connect_, context, false, nullptr);
}

BinaryStreaming::Service::Service() {
  AddMethod(new ::grpc::internal::RpcServiceMethod(
      BinaryStreaming_method_names[0],
      ::grpc::internal::RpcMethod::BIDI_STREAMING,
      new ::grpc::internal::BidiStreamingHandler< BinaryStreaming::Service, ::grpc::ByteBuffer, ::grpc::ByteBuffer>(
          [](BinaryStreaming::Service* service,
             ::grpc::ServerContext* ctx,
             ::grpc::ServerReaderWriter<::grpc::ByteBuffer,
             ::grpc::ByteBuffer>* stream) {
               return service->Connect(ctx, stream);
             }, this)));
}

BinaryStreaming::Service::~Service() {
}

::grpc::Status BinaryStreaming::Service::Connect(::grpc::ServerContext* context, ::grpc::ServerReaderWriter< ::grpc::ByteBuffer, ::grpc::ByteBuffer>* stream) {
  (void) context;
  (void) stream;
  return ::grpc::Status(::grpc::StatusCode::UNIMPLEMENTED, "");
}


}  // namespace binarystreaming

