using Amazon.XRay.Recorder.Handlers.AwsSdk.Entities;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.References
{
    public class XRayServices : AWSServiceHandlerManifest
    {
        public static readonly XRayServices Instance = new XRayServices()
        {
            Services = new Dictionary<string, AWSServiceHandler>
            {
                {
                    "DynamoDBv2", new AWSServiceHandler
                    {
                        Operations = new Dictionary<string, AWSOperationHandler>
                        {
                            {
                                "BatchWriteItem", new AWSOperationHandler
                                {
                                    RequestDescriptors = new Dictionary<string, AWSOperationRequestDescriptor>
                                    {
                                        { "RequestItems", new AWSOperationRequestDescriptor
                                            {
                                                Map = true,
                                                GetKeys = true,
                                                RenameTo = "table_names",
                                            }
                                        }
                                    },
                                    ResponseParameters = new List<string>
                                    {
                                        "ConsumedCapacity",
                                        "ItemCollectionMetrics",
                                    },
                                }
                            },
                            {
                                "DeleteItem", new AWSOperationHandler
                                {
                                    RequestParameters = new List<string>
                                    {
                                        "TableName",
                                    },
                                    ResponseParameters = new List<string>
                                    {
                                        "ConsumedCapacity",
                                        "ItemCollectionMetrics",
                                    },
                                }
                            },
                            {
                                "GetItem", new AWSOperationHandler
                                {
                                    RequestParameters = new List<string>
                                    {
                                        "ConsistentRead",
                                        "ProjectionExpression",
                                        "TableName",
                                    },
                                    ResponseParameters = new List<string>
                                    {
                                        "ConsumedCapacity",
                                    },
                                }
                            },
                            {
                                "PutItem", new AWSOperationHandler
                                {
                                    RequestParameters = new List<string>
                                    {
                                        "TableName",
                                    },
                                    ResponseParameters = new List<string>
                                    {
                                        "ConsumedCapacity",
                                        "ItemCollectionMetrics",
                                    },
                                }
                            },
                            {
                                "Query", new AWSOperationHandler
                                {
                                    RequestParameters = new List<string>
                                    {
                                        "AttributesToGet",
                                        "ConsistentRead",
                                        "IndexName",
                                        "Select",
                                        "Limit",
                                        "ProjectionExpression",
                                        "ScanIndexForward",
                                        "TableName",
                                    },
                                    ResponseParameters = new List<string>
                                    {
                                        "ConsumedCapacity",
                                    },
                                }
                            },
                            {
                                "UpdateItem", new AWSOperationHandler
                                {
                                    RequestParameters = new List<string>
                                    {
                                        "TableName",
                                    },
                                    ResponseParameters = new List<string>
                                    {
                                        "ConsumedCapacity",
                                        "ItemCollectionMetrics",
                                    },
                                }
                            },
                        }
                    }
                },
                {
                    "Lambda", new AWSServiceHandler
                    {
                        Operations = new Dictionary<string, AWSOperationHandler>
                        {
                            {
                                "InvokeAsync", new AWSOperationHandler
                                {
                                    RequestParameters = new List<string>
                                    {
                                        "FunctionName",
                                    },
                                    ResponseParameters = new List<string>
                                    {
                                        "Status",
                                    },
                                }
                            },
                        }
                    }
                },
            }
        };
    }
}
