using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
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

        internal LambdaBootstrapHandler Handler { get; private set; }

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
        public static HandlerWrapper GetHandlerWrapper(Func<Stream, ILambdaContext, CancellationToken, Task> handler)
        {
            return new HandlerWrapper(async (invocation, cancellationToken) =>
            {
                await handler(invocation.InputStream, invocation.LambdaContext, cancellationToken);
                return EmptyInvocationResponse;
            });
        }

        /// <summary>
        /// Get a HandlerWrapper that will call the given method on function invocation.
        /// Note that you may have to cast your handler to its specific type to help the compiler.
        /// Example handler signature: Task Handler(Stream, ILambdaContext)
        /// </summary>
        public static HandlerWrapper GetHandlerWrapper<TRequest>(Func<TRequest, ILambdaContext, CancellationToken, Task> handler, JsonTypeInfo<TRequest> requestInfo)
        {
            return new HandlerWrapper(async (invocation, cancellationToken) =>
            {
                var request = JsonSerializer.Deserialize(invocation.InputStream, requestInfo)!;
                await handler(request, invocation.LambdaContext, cancellationToken);
                return EmptyInvocationResponse;
            });
        }

        /// <summary>
        /// Get a HandlerWrapper that will call the given method on function invocation.
        /// Note that you may have to cast your handler to its specific type to help the compiler.
        /// </summary>
        /// <returns>A HandlerWrapper</returns>
        public static HandlerWrapper GetHandlerWrapper(Func<Stream, ILambdaContext, CancellationToken, Task<Stream>> handler)
        {
            return new HandlerWrapper(async (invocation, cancellationToken) =>
            {
                return new InvocationResponse(await handler(invocation.InputStream, invocation.LambdaContext, cancellationToken));
            });
        }

        /// <summary>
        /// Get a HandlerWrapper that will call the given method on function invocation.
        /// Note that you may have to cast your handler to its specific type to help the compiler.
        /// </summary>
        /// <returns>A HandlerWrapper</returns>
        public static HandlerWrapper GetHandlerWrapper<TRequest, TResponse>(Func<TRequest, ILambdaContext, CancellationToken, Task<TResponse>> handler, JsonTypeInfo<TRequest> requestInfo, JsonTypeInfo<TResponse> responseInfo)
        {
            return new HandlerWrapper(async (invocation, cancellationToken) =>
            {
                var request = JsonSerializer.Deserialize(invocation.InputStream, requestInfo)!;
                var response = await handler(request, invocation.LambdaContext, cancellationToken);

                var responseStream = new MemoryStream();
                JsonSerializer.Serialize(responseStream, response, responseInfo);
                return new InvocationResponse(responseStream);
            });
        }
    }
}
