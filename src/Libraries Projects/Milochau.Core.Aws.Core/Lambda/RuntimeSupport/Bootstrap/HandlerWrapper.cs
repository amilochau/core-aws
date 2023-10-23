using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Bootstrap
{
    /// <summary>
    /// This class provides methods that help you wrap existing C# Lambda implementations with LambdaBootstrapHandler delegates.
    /// This makes serialization and deserialization simpler and allows you to use existing functions them with an instance of LambdaBootstrap.
    /// </summary>
    public class HandlerWrapper
    {
        private static readonly InvocationResponse EmptyInvocationResponse = new InvocationResponse(new MemoryStream(0), false);

        public LambdaBootstrapHandler Handler { get; private set; }

        private HandlerWrapper(LambdaBootstrapHandler handler)
        {
            Handler = handler;
        }

        /// <summary>
        /// Get a HandlerWrapper that will call the given method on function invocation.
        /// Note that you may have to cast your handler to its specific type to help the compiler.
        /// Example handler signature: Task Handler(Stream, ILambdaContext)
        /// </summary>
        /// <returns>A HandlerWrapper</returns>
        public static HandlerWrapper GetHandlerWrapper(Func<Stream, ILambdaContext, Task> handler)
        {
            return new HandlerWrapper(async (invocation) =>
            {
                await handler(invocation.InputStream, invocation.LambdaContext);
                return EmptyInvocationResponse;
            });
        }

        /// <summary>
        /// Get a HandlerWrapper that will call the given method on function invocation.
        /// Note that you may have to cast your handler to its specific type to help the compiler.
        /// Example handler signature: Task Handler(Stream, ILambdaContext)
        /// </summary>
        public static HandlerWrapper GetHandlerWrapper<TRequest>(Func<TRequest, ILambdaContext, Task> handler, JsonTypeInfo<TRequest> requestInfo)
        {
            return new HandlerWrapper(async (invocation) =>
            {
                var request = JsonSerializer.Deserialize(invocation.InputStream, requestInfo)!;
                await handler(request, invocation.LambdaContext);
                return EmptyInvocationResponse;
            });
        }

        /// <summary>
        /// Get a HandlerWrapper that will call the given method on function invocation.
        /// Note that you may have to cast your handler to its specific type to help the compiler.
        /// Example handler signature: Task&ltStream&gt Handler(Stream, ILambdaContext)
        /// </summary>
        /// <returns>A HandlerWrapper</returns>
        public static HandlerWrapper GetHandlerWrapper(Func<Stream, ILambdaContext, Task<Stream>> handler)
        {
            return new HandlerWrapper(async (invocation) =>
            {
                return new InvocationResponse(await handler(invocation.InputStream, invocation.LambdaContext));
            });
        }

        /// <summary>
        /// Get a HandlerWrapper that will call the given method on function invocation.
        /// Note that you may have to cast your handler to its specific type to help the compiler.
        /// Example handler signature: Task&ltStream&gt Handler(Stream, ILambdaContext)
        /// </summary>
        /// <returns>A HandlerWrapper</returns>
        public static HandlerWrapper GetHandlerWrapper<TRequest, TResponse>(Func<TRequest, ILambdaContext, Task<TResponse>> handler, JsonTypeInfo<TRequest> requestInfo, JsonTypeInfo<TResponse> responseInfo)
        {
            return new HandlerWrapper(async (invocation) =>
            {
                var request = JsonSerializer.Deserialize(invocation.InputStream, requestInfo)!;
                var response = await handler(request, invocation.LambdaContext);

                var responseStream = new MemoryStream();
                JsonSerializer.Serialize(responseStream, response, responseInfo);
                return new InvocationResponse(responseStream);
            });
        }
    }
}
