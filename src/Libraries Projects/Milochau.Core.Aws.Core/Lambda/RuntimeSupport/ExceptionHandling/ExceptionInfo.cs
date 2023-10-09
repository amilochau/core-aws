using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling
{
    /// <summary>
    /// Class to hold basic raw information extracted from Exceptions.
    /// The raw information will be formatted as JSON to be reported to the Lambda Runtime API.
    /// </summary>
    internal class ExceptionInfo
    {
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public StackFrameInfo[] StackFrames { get; set; }
        public string StackTrace { get; set; }

        public ExceptionInfo InnerException { get; set; }
        public List<ExceptionInfo> InnerExceptions { get; internal set; } = new List<ExceptionInfo>();

        public ExceptionInfo(Exception exception, bool isNestedException = false)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

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
                    StackFrames = Array.Empty<StackFrameInfo>();
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


            if (exception is AggregateException aggregateException && aggregateException.InnerExceptions != null)
            {
                foreach (var innerEx in aggregateException.InnerExceptions)
                {
                    InnerExceptions.Add(new ExceptionInfo(innerEx, true));
                }
            }
        }

        public static ExceptionInfo GetExceptionInfo(Exception exception)
        {
            return new ExceptionInfo(exception);
        }
    }
}
