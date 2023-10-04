using Amazon.Runtime;
using System;
using System.Collections.Generic;
using static Amazon.Runtime.Internal.Endpoints.StandardLibrary.Fn;
using Amazon.Runtime.Endpoints;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Internal/AmazonDynamoDBEndpointProvider.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Internal
{
    /// <summary>
    /// Amazon DynamoDB endpoint provider.
    /// Resolves endpoint for given set of DynamoDBEndpointParameters.
    /// Can throw AmazonClientException if endpoint resolution is unsuccessful.
    /// </summary>
    public class AmazonDynamoDBEndpointProvider : IEndpointProvider
    {
        /// <summary>
        /// Resolve endpoint for DynamoDBEndpointParameters
        /// </summary>
        public Endpoint ResolveEndpoint(EndpointParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            if (parameters["UseDualStack"] == null)
                throw new AmazonClientException("UseDualStack parameter must be set for endpoint resolution");
            if (parameters["UseFIPS"] == null)
                throw new AmazonClientException("UseFIPS parameter must be set for endpoint resolution");

            var refs = new Dictionary<string, object>()
            {
                ["Region"] = parameters["Region"],
                ["UseDualStack"] = parameters["UseDualStack"],
                ["UseFIPS"] = parameters["UseFIPS"],
                ["Endpoint"] = parameters["Endpoint"],
            };
            if (IsSet(refs["Endpoint"]))
            {
                if (Equals(refs["UseFIPS"], true))
                {
                    throw new AmazonClientException("Invalid Configuration: FIPS and custom endpoint are not supported");
                }
                if (Equals(refs["UseDualStack"], true))
                {
                    throw new AmazonClientException("Invalid Configuration: Dualstack and custom endpoint are not supported");
                }
                return new Endpoint((string)refs["Endpoint"], InterpolateJson(@"", refs), InterpolateJson(@"", refs));
            }
            if (IsSet(refs["Region"]))
            {
                if ((refs["PartitionResult"] = Partition((string)refs["Region"])) != null)
                {
                    if (Equals(refs["UseFIPS"], true) && Equals(refs["UseDualStack"], true))
                    {
                        if (Equals(true, GetAttr(refs["PartitionResult"], "supportsFIPS")) && Equals(true, GetAttr(refs["PartitionResult"], "supportsDualStack")))
                        {
                            return new Endpoint(Interpolate(@"https://dynamodb-fips.{Region}.{PartitionResult#dualStackDnsSuffix}", refs), InterpolateJson(@"", refs), InterpolateJson(@"", refs));
                        }
                        throw new AmazonClientException("FIPS and DualStack are enabled, but this partition does not support one or both");
                    }
                    if (Equals(refs["UseFIPS"], true))
                    {
                        if (Equals(true, GetAttr(refs["PartitionResult"], "supportsFIPS")))
                        {
                            if (Equals("aws-us-gov", GetAttr(refs["PartitionResult"], "name")))
                            {
                                return new Endpoint(Interpolate(@"https://dynamodb.{Region}.amazonaws.com", refs), InterpolateJson(@"", refs), InterpolateJson(@"", refs));
                            }
                            return new Endpoint(Interpolate(@"https://dynamodb-fips.{Region}.{PartitionResult#dnsSuffix}", refs), InterpolateJson(@"", refs), InterpolateJson(@"", refs));
                        }
                        throw new AmazonClientException("FIPS is enabled but this partition does not support FIPS");
                    }
                    if (Equals(refs["UseDualStack"], true))
                    {
                        if (Equals(true, GetAttr(refs["PartitionResult"], "supportsDualStack")))
                        {
                            return new Endpoint(Interpolate(@"https://dynamodb.{Region}.{PartitionResult#dualStackDnsSuffix}", refs), InterpolateJson(@"", refs), InterpolateJson(@"", refs));
                        }
                        throw new AmazonClientException("DualStack is enabled but this partition does not support DualStack");
                    }
                    if (Equals(refs["Region"], "local"))
                    {
                        return new Endpoint("http://localhost:8000", InterpolateJson(@"{""authSchemes"":[{""name"":""sigv4"",""signingName"":""dynamodb"",""signingRegion"":""us-east-1""}]}", refs), InterpolateJson(@"", refs));
                    }
                    return new Endpoint(Interpolate(@"https://dynamodb.{Region}.{PartitionResult#dnsSuffix}", refs), InterpolateJson(@"", refs), InterpolateJson(@"", refs));
                }
            }
            throw new AmazonClientException("Invalid Configuration: Missing Region");

            throw new AmazonClientException("Cannot resolve endpoint");
        }
    }
}
