using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon.Runtime;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;

namespace Milochau.Core.Aws.XRayRecorder.Core.Strategies
{
    /// <summary>
    /// Defines default startegy for recording exception. By default <see cref="AmazonServiceException"/> class exeptions are marked as remote. 
    /// </summary>
    [Serializable]
    public class DefaultExceptionSerializationStrategy : IExceptionSerializationStrategy
    {
        private static readonly List<Type> _defaultExceptionClasses = new List<Type>() { typeof(AmazonServiceException)};
        private readonly List<Type> _remoteExceptionClasses = new List<Type>();

        /// <summary>
        /// Default stack frame size for the recorded <see cref="Exception"/>.
        /// </summary>
        public const int DefaultStackFrameSize = 50;

        /// <summary>
        /// The maximum stack frame size for the strategy.
        /// </summary>
        public int MaxStackFrameSize { get; private set; } = DefaultStackFrameSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="_defaultExceptionClasses"/> class.
        /// </summary>
        public DefaultExceptionSerializationStrategy() : this(DefaultStackFrameSize)
        {
        }

        /// <summary>
        /// Initializes <see cref="DefaultExceptionSerializationStrategy"/> instance with provided Stack frame size. 
        /// While setting number consider max trace size limit : https://aws.amazon.com/xray/pricing/
        /// </summary>
        /// <param name="stackFrameSize">Integer value for stack frame size.</param>
        public DefaultExceptionSerializationStrategy(int stackFrameSize) 
        {
            MaxStackFrameSize = GetValidStackFrameSize(stackFrameSize);
            _remoteExceptionClasses.AddRange(_defaultExceptionClasses);
        }

        /// <summary>
        /// Validates and returns valid max stack frame size.
        /// </summary>
        public static int GetValidStackFrameSize(int stackFrameSize)
        {
            if (stackFrameSize < 0)
            {
                return DefaultStackFrameSize;
            }

            return stackFrameSize;
        }

        /// <summary>
        /// Checks whether the exception should be marked as remote.
        /// </summary>
        /// <param name="e">Instance of <see cref="Exception"/>.</param>
        /// <returns>True if the exception is of type present in <see cref="_remoteExceptionClasses"/>, else false.</returns>
        private bool IsRemoteException(Exception e)
        {
            foreach (Type t in _remoteExceptionClasses)
            {
                Type exceptionType = e.GetType();
                if (exceptionType == t || exceptionType.IsSubclassOf(t))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Visit each node in the cause chain. For each node:
        /// Determine if it has already been described in one of the child subsegments' causes. If so, link there.
        /// Otherwise, describe it and add it to the Cause and  returns the list of <see cref="ExceptionDescriptor"/>.
        /// </summary>
        /// <param name="e">The exception to be added</param>
        /// <returns> List of <see cref="ExceptionDescriptor"/></returns>
        public List<ExceptionDescriptor> DescribeException(Exception? e)
        {
            List<ExceptionDescriptor> result = new List<ExceptionDescriptor>();
            if (e == null)
            {
                return result;
            }

            // The exception is not described. Start describe it.
            ExceptionDescriptor curDescriptor = new ExceptionDescriptor(e.Message, e.GetType().Name);
            while (e != null)
            {
                InternalStackFrame[] frames = new StackTrace(e, true).GetFrames().Select(x => new InternalStackFrame
                {
                    Path = x.GetFileName(),
                    Line = x.GetFileLineNumber(),
                }).ToArray();
                if (frames != null && frames.Length > MaxStackFrameSize)
                {
                    curDescriptor.Truncated = frames.Length - MaxStackFrameSize > 0 ? frames.Length - MaxStackFrameSize : null;
                    curDescriptor.Stack = new InternalStackFrame[MaxStackFrameSize];
                    Array.Copy(frames, curDescriptor.Stack, MaxStackFrameSize);
                }
                else
                {
                    curDescriptor.Stack = frames;
                }

                if (IsRemoteException(e))
                {
                    curDescriptor.Remote = true;
                }

                result.Add(curDescriptor);

                e = e.InnerException;
                if (e != null)
                {
                    // Inner exception alreay described
                    var newDescriptor = new ExceptionDescriptor(e.Message, e.GetType().Name);
                    curDescriptor.Cause = newDescriptor.Id;
                    curDescriptor = newDescriptor;
                }
            }

            return result;
        }
    }
}
