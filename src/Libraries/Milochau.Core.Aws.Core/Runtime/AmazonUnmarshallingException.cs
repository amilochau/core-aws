﻿using System;
using System.Net;
using System.Text;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// This exception is thrown when there is a parse error on the response back from AWS.
    /// </summary>
    public class AmazonUnmarshallingException : AmazonServiceException
    {
        #region Constructors

        public AmazonUnmarshallingException(string? requestId, string responseBody, Exception innerException, HttpStatusCode statusCode)
            : base("Error unmarshalling response back from AWS.", innerException, statusCode)
        {
            RequestId = requestId;
            ResponseBody = responseBody;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The entire response body that caused this exception, if available.
        /// </summary>
        public string? ResponseBody { get; private set; }

        #endregion

        #region Overrides

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();

                AppendFormat(sb, "Request ID: {0}", RequestId);
                AppendFormat(sb, "Response Body: {0}", ResponseBody);
                AppendFormat(sb, "HTTP Status Code: {0}", (int)StatusCode + " " + StatusCode.ToString());

                return base.Message + " " + sb.ToString();
            }
        }

        #endregion

        #region Private methods

        private static void AppendFormat(StringBuilder sb, string format, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (sb.Length > 0)
                sb.Append(", ");

            sb.AppendFormat(format, value);
        }

        #endregion
    }
}
