using Milochau.Core.Aws.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito
{
    ///<summary>
    /// Common exception for the DynamoDB service.
    /// </summary>
    public partial class AmazonCognitoIdentityProviderException : AmazonServiceException
    {
        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonCognitoIdentityProviderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonCognitoIdentityProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonCognitoIdentityProviderException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonCognitoIdentityProviderException(string message, ErrorType errorType, string? errorCode, string? requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonCognitoIdentityProviderException(string message, Exception innerException, ErrorType errorType, string? errorCode, string? requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode)
        {
        }
    }
}
