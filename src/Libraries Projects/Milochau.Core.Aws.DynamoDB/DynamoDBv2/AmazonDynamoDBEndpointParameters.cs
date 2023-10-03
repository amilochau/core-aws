using Amazon.Runtime.Endpoints;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/AmazonDynamoDBEndpointParameters.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2
{
    /// <summary>
    /// Contains parameters used for resolving DynamoDB endpoints
    /// Parameters can be sourced from client config and service operations
    /// Used by internal DynamoDBEndpointProvider and DynamoDBEndpointResolver
    /// Can be used by custom EndpointProvider, see ClientConfig.EndpointProvider
    /// </summary>
    public class DynamoDBEndpointParameters : EndpointParameters
    {
        /// <summary>
        /// DynamoDBEndpointParameters constructor
        /// </summary>
        public DynamoDBEndpointParameters()
        {
            UseDualStack = false;
            UseFIPS = false;
        }

        /// <summary>
        /// Region parameter
        /// </summary>
        public string Region
        {
            get { return (string)this["Region"]; }
            set { this["Region"] = value; }
        }

        /// <summary>
        /// UseDualStack parameter
        /// </summary>
        public bool? UseDualStack
        {
            get { return (bool?)this["UseDualStack"]; }
            set { this["UseDualStack"] = value; }
        }

        /// <summary>
        /// UseFIPS parameter
        /// </summary>
        public bool? UseFIPS
        {
            get { return (bool?)this["UseFIPS"]; }
            set { this["UseFIPS"] = value; }
        }

        /// <summary>
        /// Endpoint parameter
        /// </summary>
        public string Endpoint
        {
            get { return (string)this["Endpoint"]; }
            set { this["Endpoint"] = value; }
        }
    }
}
