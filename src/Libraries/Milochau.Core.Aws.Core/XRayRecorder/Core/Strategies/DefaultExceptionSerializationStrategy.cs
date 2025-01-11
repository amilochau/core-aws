using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies
{
    /// <summary>
    /// Defines default startegy for recording exception. By default <see cref="AmazonServiceException"/> class exeptions are marked as remote. 
    /// </summary>
    [Serializable]
    public class DefaultExceptionSerializationStrategy
    {
        /// <summary>
        /// Default stack frame size for the recorded <see cref="Exception"/>.
        /// </summary>
        public const int DefaultStackFrameSize = 50;

        /// <summary>
        /// Checks whether the exception should be marked as remote.
        /// </summary>
        /// <param name="e">Instance of <see cref="Exception"/>.</param>
        /// <returns>True if the exception is of type present in <see cref="remoteExceptionClasses"/>, else false.</returns>
        private static bool IsRemoteException(Exception e)
        {
            Type exceptionType = e.GetType();
            return exceptionType == typeof(AmazonServiceException) || exceptionType.IsSubclassOf(typeof(AmazonServiceException));
        }

        /// <summary>
        /// Visit each node in the cause chain. For each node:
        /// Determine if it has already been described in one of the child subsegments' causes. If so, link there.
        /// Otherwise, describe it and add it to the Cause and  returns the list of <see cref="ExceptionDescriptor"/>.
        /// </summary>
        /// <param name="e">The exception to be added</param>
        /// <returns> List of <see cref="ExceptionDescriptor"/></returns>
        public static List<ExceptionDescriptor> DescribeException(Exception? e)
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
                if (frames != null && frames.Length > DefaultStackFrameSize)
                {
                    curDescriptor.Truncated = frames.Length - DefaultStackFrameSize > 0 ? frames.Length - DefaultStackFrameSize : null;
                    curDescriptor.Stack = new InternalStackFrame[DefaultStackFrameSize];
                    Array.Copy(frames, curDescriptor.Stack, DefaultStackFrameSize);
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
