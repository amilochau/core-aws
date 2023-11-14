using System.Collections.Generic;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>Proxy request options</summary>
    public class ProxyRequestOptions
    {
        /// <summary>Whether the request is anonymous</summary>
        public bool AnonymousRequest { get; set; }

        /// <summary>User id</summary>
        /// <remarks>Is not used if <see cref="AnonymousRequest"/> is set to true</remarks>
        public string UserId { get; set; } = "00000000000000000000000000000001";

        /// <summary>User name</summary>
        /// <remarks>Is not used if <see cref="AnonymousRequest"/> is set to true</remarks>
        public string UserName { get; set; } = "John Doe";

        /// <summary>User emails</summary>
        /// <remarks>Is not used if <see cref="AnonymousRequest"/> is set to true</remarks>
        public string UserEmail { get; set; } = "john.doe@milochau.com";

        /// <summary>Path parameters</summary>
        public Dictionary<string, string> PathParameters { get; set; } = new Dictionary<string, string>();
    }
}
