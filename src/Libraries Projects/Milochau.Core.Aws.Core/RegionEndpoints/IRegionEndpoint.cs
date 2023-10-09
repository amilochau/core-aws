namespace Milochau.Core.Aws.Core.RegionEndpoints
{
    public interface IRegionEndpoint
    {
        string RegionName { get;  }
        string DisplayName { get; }
        /// <summary>
        /// Gets the endpoint for a service in a region.
        /// <para />
        /// For forwards compatibility, if the service being requested for isn't known in the region, this method 
        /// will generate an endpoint using the AWS endpoint heuristics. In this case, it is not guaranteed the
        /// endpoint will point to a valid service endpoint.
        /// </summary>
        /// <param name="serviceName">
        /// The services system name. Service system names can be obtained from the
        /// RegionEndpointServiceName member of the ClientConfig-derived class for the service.
        /// </param>
        RegionEndpoint.Endpoint? GetEndpointForService(string serviceName);
    }

    public interface IRegionEndpointProvider
    {
        IRegionEndpoint GetRegionEndpoint(string regionName);
    }
}
