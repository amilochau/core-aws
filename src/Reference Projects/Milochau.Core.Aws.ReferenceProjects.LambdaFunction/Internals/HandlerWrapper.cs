using Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals
{
    /// <summary>
    /// This class provides methods that help you wrap existing C# Lambda implementations with LambdaBootstrapHandler delegates.
    /// This makes serialization and deserialization simpler and allows you to use existing functions them with an instance of LambdaBootstrap.
    /// </summary>
    public class HandlerWrapper : IDisposable
    {
        private MemoryStream OutputStream = new MemoryStream();

        public LambdaBootstrapHandler? Handler { get; private set; }

        private HandlerWrapper() { }

        /// <summary>
        /// Get a HandlerWrapper that will call the given method on function invocation.
        /// Note that you may have to cast your handler to its specific type to help the compiler.
        /// Example handler signature: Task&ltPocoOut&gt Handler(PocoIn, ILambdaContext)
        /// </summary>
        /// <param name="handler">Func called for each invocation of the Lambda function.</param>
        /// <param name="serializer">ILambdaSerializer to use when calling the handler</param>
        /// <returns>A HandlerWrapper</returns>
        public static HandlerWrapper GetHandlerWrapper<TInput, TOutput>(Func<TInput, ILambdaContext, Task<TOutput>> handler, Func<Stream, TInput> deserializerInput, Action<TOutput, Stream> serializeOutput)
        {
            var handlerWrapper = new HandlerWrapper();
            handlerWrapper.Handler = async (invocation) =>
            {
                TInput input = deserializerInput(invocation.InputStream!);
                TOutput output = await handler(input, invocation.LambdaContext);
                handlerWrapper.OutputStream.SetLength(0);
                serializeOutput(output, handlerWrapper.OutputStream);
                handlerWrapper.OutputStream.Position = 0;
                return new InvocationResponse(handlerWrapper.OutputStream, false);
            };
            return handlerWrapper;
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
