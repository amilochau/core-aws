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
    internal class HandlerWrapper
    {
        private static readonly InvocationResponse EmptyInvocationResponse = new InvocationResponse(new MemoryStream(0), false);

        /// <summary>
        /// Build a LambdaBootstrapHandler that will call the given method on function invocation.
        /// Example handler signature: Task Handler(Stream, ILambdaContext, CancellationToken)
        /// </summary>
        internal static LambdaBootstrapHandler BuildHandler(Func<Stream, ILambdaContext, CancellationToken, Task> handler) => async (invocation, cancellationToken) =>
        {
            await handler(invocation.InputStream, invocation.LambdaContext, cancellationToken);
            return EmptyInvocationResponse;
        };

        /// <summary>
        /// Build a LambdaBootstrapHandler that will call the given method on function invocation.
        /// Example handler signature: Task Handler(TRequest, ILambdaContext, CancellationToken)
        /// </summary>
        internal static LambdaBootstrapHandler BuildHandler<TRequest>(Func<TRequest, ILambdaContext, CancellationToken, Task> handler, JsonTypeInfo<TRequest> requestInfo) => async (invocation, cancellationToken) =>
        {
            var request = JsonSerializer.Deserialize(invocation.InputStream, requestInfo)!;
            await handler(request, invocation.LambdaContext, cancellationToken);
            return EmptyInvocationResponse;
        };

        /// <summary>
        /// Build a LambdaBootstrapHandler that will call the given method on function invocation.
        /// Example handler signature: Task<Stream> Handler(Stream, ILambdaContext, CancellationToken)
        /// </summary>
        internal static LambdaBootstrapHandler BuildHandler(Func<Stream, ILambdaContext, CancellationToken, Task<Stream>> handler) => async (invocation, cancellationToken) =>
        {
            return new InvocationResponse(await handler(invocation.InputStream, invocation.LambdaContext, cancellationToken));
        };

        /// <summary>
        /// Build a LambdaBootstrapHandler that will call the given method on function invocation.
        /// Example handler signature: Task<TResponse> Handler(TRequest, ILambdaContext, CancellationToken)
        /// </summary>
        internal static LambdaBootstrapHandler BuildHandler<TRequest, TResponse>(Func<TRequest, ILambdaContext, CancellationToken, Task<TResponse>> handler, JsonTypeInfo<TRequest> requestInfo, JsonTypeInfo<TResponse> responseInfo) => async (invocation, cancellationToken) =>
        {
            var request = JsonSerializer.Deserialize(invocation.InputStream, requestInfo)!;
            var response = await handler(request, invocation.LambdaContext, cancellationToken);

            var responseStream = new MemoryStream();
            JsonSerializer.Serialize(responseStream, response, responseInfo);
            return new InvocationResponse(responseStream);
        };
    }
}
