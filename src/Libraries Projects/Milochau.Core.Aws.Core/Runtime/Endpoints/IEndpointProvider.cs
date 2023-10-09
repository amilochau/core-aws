namespace Milochau.Core.Aws.Core.Runtime.Endpoints
{
    /// <summary>
    /// Interface to be implemented by service specific EndpointProviders.
    /// </summary>
    public interface IEndpointProvider
    {
        /// <summary>
        /// Resolves service endpoint based on EndpointParameters
        /// </summary>
        Endpoint ResolveEndpoint(EndpointParameters parameters);
    }
}
