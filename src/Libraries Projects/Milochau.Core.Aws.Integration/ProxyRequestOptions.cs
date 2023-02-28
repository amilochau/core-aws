using System.Collections.Generic;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>Proxy request options</summary>
    public class ProxyRequestOptions
    {
        /// <summary>User id</summary>
        public string UserId { get; set; } = "00000000000000000000000000000001";

        /// <summary>User name</summary>
        public string UserName { get; set; } = "John Doe";

        /// <summary>User emails</summary>
        public string UserEmail { get; set; } = "john.doe@milochau.com";

        /// <summary>Path parameters</summary>
        public Dictionary<string, string> PathParameters { get; set; } = new Dictionary<string, string>();
    }
}
