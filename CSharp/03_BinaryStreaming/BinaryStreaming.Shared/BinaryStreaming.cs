using System.Threading.Tasks;
using Grpc.Core;

namespace BinaryStreaming.Shared
{    
    /// <summary>
    /// Base class for server-side implementations
    /// </summary>
    [BindServiceMethod(typeof(BinaryStreamingGrpc), "BindService")]
    public abstract partial class BinaryStreamingServiceBase
    {
        public virtual Task Connect(IAsyncStreamReader<byte[]> requestStream, IServerStreamWriter<byte[]> responseStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }
    }
    
    public static partial class BinaryStreamingGrpc
    {
        public static readonly string ServiceName = nameof(BinaryStreamingGrpc);
        
        public static readonly Marshaller<byte[]> ThroughMarshaller = new Marshaller<byte[]>(x => x, x => x);
        
        public static readonly Method<byte[], byte[]> ConnectMethod = new Method<byte[], byte[]>(
            MethodType.DuplexStreaming,
            ServiceName,
            nameof(BinaryStreamingServiceBase.Connect),
            ThroughMarshaller,
            ThroughMarshaller);

        /// <summary>
        /// Creates service definition that can be registered with a server
        /// </summary>
        /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
        public static ServerServiceDefinition BindService(BinaryStreamingServiceBase serviceImpl)
        {
            return ServerServiceDefinition.CreateBuilder().AddMethod(ConnectMethod, serviceImpl.Connect).Build();
        }
        
        /// <summary>
        /// Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
        /// Note: this method is part of an experimental API that can change or be removed without any prior notice.
        /// </summary>
        /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
        /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
        public static void BindService(ServiceBinderBase serviceBinder, BinaryStreamingServiceBase serviceImpl)
        {
            serviceBinder.AddMethod(ConnectMethod, serviceImpl == null ? null : new DuplexStreamingServerMethod<byte[], byte[]>(serviceImpl.Connect));
        }
    }
}