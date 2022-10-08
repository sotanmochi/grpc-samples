// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: file_transfer.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981
#region Designer generated code

using grpc = global::Grpc.Core;

namespace FileTransferApp.Shared {
  public static partial class FileTransfer
  {
    static readonly string __ServiceName = "file_transfer.FileTransfer";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::FileTransferApp.Shared.DownloadRequest> __Marshaller_file_transfer_DownloadRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::FileTransferApp.Shared.DownloadRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::FileTransferApp.Shared.DownloadResponse> __Marshaller_file_transfer_DownloadResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::FileTransferApp.Shared.DownloadResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::FileTransferApp.Shared.UploadRequest> __Marshaller_file_transfer_UploadRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::FileTransferApp.Shared.UploadRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::FileTransferApp.Shared.UploadResponse> __Marshaller_file_transfer_UploadResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::FileTransferApp.Shared.UploadResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::FileTransferApp.Shared.DownloadRequest, global::FileTransferApp.Shared.DownloadResponse> __Method_DownloadFile = new grpc::Method<global::FileTransferApp.Shared.DownloadRequest, global::FileTransferApp.Shared.DownloadResponse>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "DownloadFile",
        __Marshaller_file_transfer_DownloadRequest,
        __Marshaller_file_transfer_DownloadResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::FileTransferApp.Shared.UploadRequest, global::FileTransferApp.Shared.UploadResponse> __Method_UploadFile = new grpc::Method<global::FileTransferApp.Shared.UploadRequest, global::FileTransferApp.Shared.UploadResponse>(
        grpc::MethodType.ClientStreaming,
        __ServiceName,
        "UploadFile",
        __Marshaller_file_transfer_UploadRequest,
        __Marshaller_file_transfer_UploadResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::FileTransferApp.Shared.FileTransferReflection.Descriptor.Services[0]; }
    }

    /// <summary>Client for FileTransfer</summary>
    public partial class FileTransferClient : grpc::ClientBase<FileTransferClient>
    {
      /// <summary>Creates a new client for FileTransfer</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public FileTransferClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for FileTransfer that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public FileTransferClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected FileTransferClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected FileTransferClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncServerStreamingCall<global::FileTransferApp.Shared.DownloadResponse> DownloadFile(global::FileTransferApp.Shared.DownloadRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return DownloadFile(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncServerStreamingCall<global::FileTransferApp.Shared.DownloadResponse> DownloadFile(global::FileTransferApp.Shared.DownloadRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_DownloadFile, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncClientStreamingCall<global::FileTransferApp.Shared.UploadRequest, global::FileTransferApp.Shared.UploadResponse> UploadFile(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return UploadFile(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncClientStreamingCall<global::FileTransferApp.Shared.UploadRequest, global::FileTransferApp.Shared.UploadResponse> UploadFile(grpc::CallOptions options)
      {
        return CallInvoker.AsyncClientStreamingCall(__Method_UploadFile, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override FileTransferClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new FileTransferClient(configuration);
      }
    }

  }
}
#endregion