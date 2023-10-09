namespace Milochau.Core.Aws.Core.Runtime.Endpoints
{
    /// <summary>
    /// Endpoint to be used when calling service operation.
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        /// Constructor used by code-generated EndpointProvider
        /// </summary>
        public Endpoint(string url)
        {
            URL = url;
        }

        /// <summary>
        /// Endpoint's url 
        /// </summary>
        public string URL { get; set; }
    }
}
