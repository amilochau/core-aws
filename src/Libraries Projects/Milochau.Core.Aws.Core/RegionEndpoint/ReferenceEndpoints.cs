using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Internal
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
                                DnsSuffix = "amazonaws.com",
                                Hostname = "{service}-fips.{region}.{dnsSuffix}",
                                Tags = new List<string> { "fips" },
                            },
                            new EndpointsPartitionDefaultsVariant
                            {
                                DnsSuffix = "api.aws",
                                Hostname = "{service}-fips.{region}.{dnsSuffix}",
                                Tags = new List<string> { "dualstack", "fips" },
                            },
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
                    RegionRegex = "^(us|eu|ap|sa|ca|me|af|il)\\-\\w+\\-\\d+$",
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
                                    { "us-east-1", new EndpointsPartitionDefaults
                                        {
                                            Variants = new List<EndpointsPartitionDefaultsVariant>
                                            {
                                                new EndpointsPartitionDefaultsVariant
                                                {
                                                    Hostname = "dynamodb-fips.us-east-1.amazonaws.com",
                                                    Tags = new List<string> { "fips" },
                                                }
                                            }
                                        }
                                    },
                                },
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
        public string? PartitionEndpoint { get; set; }
        public bool? IsRegionalized { get; set; }
        public EndpointsPartitionDefaults? Defaults { get; set; }
    }

    public class MergedEndpoint
    {
        public string Hostname { get; set; }
        public bool? Deprecated { get; set; }
        public MergedEndpointCredentialScope? CredentialScope { get; set; }
        public List<string> SignatureVersions { get; set; }
    }

    public class MergedEndpointCredentialScope
    {
        public string? Region { get; set; }
    }
}
