using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.References
{
    public class Endpoints
    {
        public static readonly Endpoints Reference = new Endpoints
        {
            Partitions = new List<EndpointsPartition>
            {
                new EndpointsPartition
                {
                    Defaults = new EndpointsPartitionDefaults
                    {
                        Hostname = "{service}.{region}.{dnsSuffix}",
                        Protocols = new List<string> { "https" },
                        SignatureVersions = new List<string> { "v4" },
                        Variants = new List<EndpointsPartitionDefaultsVariant>
                        {
                            new EndpointsPartitionDefaultsVariant
                            {
                                DnsSuffix = "api.aws",
                                Hostname = "{service}.{region}.{dnsSuffix}",
                                Tags = new List<string> { "dualstack" },
                            }
                        }
                    },
                    DnsSuffix = "amazonaws.com",
                    Partition = "aws",
                    PartitionName = "AWS Standard",
                    RegionRegex = "^(us|eu)\\-\\w+\\-\\d+$",
                    Regions = new Dictionary<string, EndpointsPartitionRegion>
                    {
                        { "eu-west-3", new EndpointsPartitionRegion { Description = "Europe (Paris)" } },
                        { "us-east-1", new EndpointsPartitionRegion { Description = "US East (N. Virginia)" } },
                    },
                    Services = new Dictionary<string, EndpointsPartitionService>
                    {
                        {
                            "dynamodb",
                            new EndpointsPartitionService
                            {
                                Defaults = new EndpointsPartitionDefaults
                                {
                                    Protocols = new List<string> { "http", "https" },
                                },
                                Endpoints = new Dictionary<string, EndpointsPartitionDefaults>
                                {
                                    { "eu-west-3", new EndpointsPartitionDefaults() },
                                    { "us-east-1", new EndpointsPartitionDefaults() },
                                },
                            }
                        },
                        {
                            "email", new EndpointsPartitionService
                            {
                                Endpoints = new Dictionary<string, EndpointsPartitionDefaults>
                                {
                                    { "eu-west-3", new EndpointsPartitionDefaults() },
                                    { "us-east-1", new EndpointsPartitionDefaults() },
                                }
                            }
                        },
                        {
                            "lambda",
                            new EndpointsPartitionService
                            {
                                Endpoints = new Dictionary<string, EndpointsPartitionDefaults>
                                {
                                    { "eu-west-3", new EndpointsPartitionDefaults
                                        {
                                            Hostname = "lambda.eu-west-3.api.aws",
                                            Tags = new List<string> { "dualstack" },
                                        }
                                    },
                                    { "us-east-1", new EndpointsPartitionDefaults
                                        {
                                            Variants = new List<EndpointsPartitionDefaultsVariant>
                                            {
                                                new EndpointsPartitionDefaultsVariant
                                                {
                                                    Hostname = "lambda.us-east-1.api.aws",
                                                    Tags = new List<string> { "dualstack" },
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            },
            Version = "3",
        };

        private Endpoints()
        {
        }

        public List<EndpointsPartition> Partitions { get; set; }
        public string Version { get; set; }
    }

    public class EndpointsPartition
    {
        public EndpointsPartitionDefaults Defaults { get; set; }
        public string DnsSuffix { get; set; }
        public string Partition { get; set; }
        public string PartitionName { get; set; }
        public string RegionRegex { get; set; }
        public Dictionary<string, EndpointsPartitionRegion> Regions { get; set; }
        public Dictionary<string, EndpointsPartitionService> Services { get; set; }
    }

    public class EndpointsPartitionDefaults
    {
        public string Hostname { get; set; }
        public List<string> Protocols { get; set; }
        public List<string> SignatureVersions { get; set; }
        public List<string> Tags { get; set; }
        public List<EndpointsPartitionDefaultsVariant> Variants { get; set; } = new List<EndpointsPartitionDefaultsVariant>();
    }

    public class EndpointsPartitionDefaultsVariant
    {
        public string DnsSuffix { get; set; }
        public string Hostname { get; set; }
        public List<string> Tags { get; set; }
    }

    public class EndpointsPartitionRegion
    {
        public string Description { get; set; }
    }

    public class EndpointsPartitionService
    {
        public Dictionary<string, EndpointsPartitionDefaults> Endpoints { get; set; }
        public string PartitionEndpoint { get; set; }
        public bool? IsRegionalized { get; set; }
        public EndpointsPartitionDefaults Defaults { get; set; }
    }

    public class MergedEndpoint
    {
        public string Hostname { get; set; }
        public bool? Deprecated { get; set; }
        public MergedEndpointCredentialScope CredentialScope { get; set; }
        public List<string> SignatureVersions { get; set; }
    }

    public class MergedEndpointCredentialScope
    {
        public string Region { get; set; }
    }
}
