using System;
using System.Collections.Generic;
using System.Net;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    internal static class ExceptionUtils
    {
        internal static HttpStatusCode? DetermineHttpStatusCode(Exception e)
        {
            if ((e as WebException)?.Response is HttpWebResponse response)
            {
                return response.StatusCode;
            }

            var requestException = e as System.Net.Http.HttpRequestException;
            if (requestException?.Data?.Contains("StatusCode") == true)
            {
                return (HttpStatusCode)requestException.Data["StatusCode"]!;
            }

            if (e?.InnerException != null)
            {
                return DetermineHttpStatusCode(e.InnerException);
            }

            return null;
        }

        internal static bool IsInnerException<T>(Exception exception)
            where T : Exception
        {
            return IsInnerException<T>(exception, out _);
        }

        internal static bool IsInnerException<T>(Exception exception, out T? inner)
            where T : Exception
        {
            inner = null;
            Queue<Exception> queue = new Queue<Exception>();

            //Modified BFS for the specific exception type
            var currentException = exception;
            do
            {
                //When queue.Count == 0  this is the initial parent queue item
                //which will not be checked for the exception of type T.
                if(queue.Count > 0)
                {
                    currentException = queue.Dequeue();
                    inner = currentException as T;
                    if (inner != null)
                    {
                        return true;
                    }
                }

                if (currentException is AggregateException aggregateException)
                {
                    foreach (var e in aggregateException.InnerExceptions)
                    {
                        queue.Enqueue(e);
                    }

                    continue;
                }

                if (currentException.InnerException != null)
                {
                    queue.Enqueue(currentException.InnerException);
                }                                
            }
            while (queue.Count > 0);            

            return false;
        }
    }
}
