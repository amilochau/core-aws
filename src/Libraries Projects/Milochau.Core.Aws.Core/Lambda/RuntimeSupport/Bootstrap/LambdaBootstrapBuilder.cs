using System;
using System.IO;
using System.Threading.Tasks;

using Milochau.Core.Aws.Core.Lambda.Core;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Bootstrap
{
    /// <summary>
    /// Class to communicate with the Lambda Runtime API, handle initialization,
    /// and run the invoke loop for an AWS Lambda function
    /// </summary>
    public class LambdaBootstrapBuilder
    {
        private HandlerWrapper _handlerWrapper;

        private LambdaBootstrapBuilder(HandlerWrapper handlerWrapper)
        {
            this._handlerWrapper = handlerWrapper;
        }

        /// <summary>
        /// Create a builder for creating the LambdaBootstrap.
        /// </summary>
        /// <param name="handler">The handler that will be called for each Lambda invocation</param>
        /// <returns></returns>
        public static LambdaBootstrapBuilder Create(Func<Stream, ILambdaContext, Task> handler)
        {
            return new LambdaBootstrapBuilder(HandlerWrapper.GetHandlerWrapper(handler));
        }

        /// <summary>
        /// Create a builder for creating the LambdaBootstrap.
        /// </summary>
        /// <param name="handler">The handler that will be called for each Lambda invocation</param>
        /// <returns></returns>
        public static LambdaBootstrapBuilder Create(Func<Stream, ILambdaContext, Task<Stream>> handler)
        {
            return new LambdaBootstrapBuilder(HandlerWrapper.GetHandlerWrapper(handler));
        }

        public LambdaBootstrap Build()
        {
            return new LambdaBootstrap(_handlerWrapper);
        }
    }
}
