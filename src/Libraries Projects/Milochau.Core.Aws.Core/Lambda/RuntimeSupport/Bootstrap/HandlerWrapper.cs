using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Bootstrap
{
    /// <summary>
    /// This class provides methods that help you wrap existing C# Lambda implementations with LambdaBootstrapHandler delegates.
    /// This makes serialization and deserialization simpler and allows you to use existing functions them with an instance of LambdaBootstrap.
    /// </summary>
    public class HandlerWrapper : IDisposable
    {
        private static readonly InvocationResponse EmptyInvocationResponse =
            new InvocationResponse(new MemoryStream(0), false);

        private readonly MemoryStream OutputStream = new MemoryStream();

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
        /// <param name="handler">Func called for each invocation of the Lambda function.</param>
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
        /// Example handler signature: Task&ltStream&gt Handler(Stream, ILambdaContext)
        /// </summary>
        /// <param name="handler">Func called for each invocation of the Lambda function.</param>
        /// <returns>A HandlerWrapper</returns>
        public static HandlerWrapper GetHandlerWrapper(Func<Stream, ILambdaContext, Task<Stream>> handler)
        {
            return new HandlerWrapper(async (invocation) =>
            {
                return new InvocationResponse(await handler(invocation.InputStream, invocation.LambdaContext));
            });
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OutputStream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
