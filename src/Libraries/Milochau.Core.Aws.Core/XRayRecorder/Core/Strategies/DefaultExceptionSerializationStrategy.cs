using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies
{
    /// <summary>
    /// Defines default startegy for recording exception. By default <see cref="AmazonServiceException"/> class exeptions are marked as remote. 
    /// </summary>
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
        /// <param name="subsegments">The subsegments to search for existing exception descriptor.</param>
        /// <returns> List of <see cref="ExceptionDescriptor"/></returns>
        public List<ExceptionDescriptor> DescribeException(Exception? e, IEnumerable<Subsegment> subsegments)
        {
            var result = new List<ExceptionDescriptor>();
            if (e == null)
            {
                return result;
            }

            // First check if the exception has been described in subsegment
            var ex = new ExceptionDescriptor();
            IEnumerable<ExceptionDescriptor>? existingExceptionDescriptors = null;
            if (subsegments != null)
            {
                existingExceptionDescriptors = subsegments.Where(subsegment => subsegment.Cause != null && subsegment.Cause.IsExceptionAdded).SelectMany(subsegment => subsegment.Cause!.ExceptionDescriptors!);
            }

            ExceptionDescriptor? existingDescriptor = null;
            if (existingExceptionDescriptors != null)
            {
                existingDescriptor = existingExceptionDescriptors.FirstOrDefault(descriptor => e.Equals(descriptor.Exception));
            }

            // While referencing exception from child, record id if exists or cause and return.
            if (existingDescriptor != null)
            {
                ex.Cause = existingDescriptor.Id != null ? existingDescriptor.Id : existingDescriptor.Cause;
                ex.Exception = existingDescriptor.Exception; // pass the exception of the cause so that this reference can be found if the same exception is thrown again
                ex.Id = null;  // setting this to null since, cause is already populated with reference to downstream exception
                result.Add(ex);
                return result;
            }

            // The exception is not described. Start describe it.
            ExceptionDescriptor curDescriptor = new ExceptionDescriptor();
            while (e != null)
            {
                curDescriptor.Exception = e;
                curDescriptor.Message = e.Message;
                curDescriptor.Type = e.GetType().Name;
                InternalStackFrame[] frames = new StackTrace(e, true).GetFrames().Select(x => new InternalStackFrame
                {
                    Path = x.GetFileName(),
                    Line = x.GetFileLineNumber(),
                }).ToArray();
                if (frames != null && frames.Length > DefaultStackFrameSize)
                {
                    curDescriptor.Truncated = frames.Length - DefaultStackFrameSize;
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
                    ExceptionDescriptor? innerExceptionDescriptor = existingExceptionDescriptors?.FirstOrDefault(d => d.Exception != null && d.Exception.Equals(e));
                    if (innerExceptionDescriptor != null)
                    {
                        curDescriptor.Cause = innerExceptionDescriptor.Id;
                        e = null;
                    }
                    else
                    {
                        var newDescriptor = new ExceptionDescriptor();
                        curDescriptor.Cause = newDescriptor.Id;
                        curDescriptor = newDescriptor;
                    }
                }
            }

            return result;
        }
    }
}
