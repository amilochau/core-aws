using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals
{
    public class StackFrameInfo
    {
        public StackFrameInfo(StackFrame stackFrame)
        {
            Path = stackFrame.GetFileName();
            Line = stackFrame.GetFileLineNumber();
        }

        public string? Path { get; }
        public int Line { get; }
    }

    public class ExceptionInfo
    {
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public StackFrameInfo[]? StackFrames { get; set; }
        public string? StackTrace { get; set; }

        public ExceptionInfo? InnerException { get; set; }
        public List<ExceptionInfo> InnerExceptions { get; internal set; } = new List<ExceptionInfo>();

        public ExceptionInfo(Exception exception, bool isNestedException = false)
        {
            ErrorType = exception.GetType().Name;
            ErrorMessage = exception.Message;

            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                StackTrace stackTrace = new StackTrace(exception, true);
                StackTrace = stackTrace.ToString();

                // Only extract the stack frames like this for the top-level exception
                // This is used for Xray Exception serialization
                if (isNestedException || stackTrace?.GetFrames() == null)
                {
                    StackFrames = new StackFrameInfo[0];
                }
                else
                {
                    StackFrames = (
                        from sf in stackTrace.GetFrames()
                        where sf != null
                        select new StackFrameInfo(sf)
                    ).ToArray();
                }
            }

            if (exception.InnerException != null)
            {
                InnerException = new ExceptionInfo(exception.InnerException, true);
            }

            AggregateException? aggregateException = exception as AggregateException;

            if (aggregateException != null && aggregateException.InnerExceptions != null)
            {
                foreach (var innerEx in aggregateException.InnerExceptions)
                {
                    InnerExceptions.Add(new ExceptionInfo(innerEx, true));
                }
            }
        }
    }

}
