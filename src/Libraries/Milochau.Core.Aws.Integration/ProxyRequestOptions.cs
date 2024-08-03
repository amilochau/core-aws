using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>Proxy request options</summary>
    public class ProxyRequestOptions
    {
        /// <summary>User sub</summary>
        /// <remarks>Is not used if <see cref="AnonymousRequest"/> is set to true</remarks>
        public string UserSub { get; set; } = Environment.GetEnvironmentVariable("USER_SUB") ?? "00000000-0000-0000-0000-000000000000";

        /// <summary>User id</summary>
        /// <remarks>Is not used if <see cref="AnonymousRequest"/> is set to true</remarks>
        public string UserId { get; set; } = Environment.GetEnvironmentVariable("USER_ID") ?? "00000000-0000-0000-0000-000000000001";

        /// <summary>User name</summary>
        /// <remarks>Is not used if <see cref="AnonymousRequest"/> is set to true</remarks>
        public string UserName { get; set; } = Environment.GetEnvironmentVariable("USER_NAME") ?? "John Doe";

        /// <summary>User emails</summary>
        /// <remarks>Is not used if <see cref="AnonymousRequest"/> is set to true</remarks>
        public string UserEmail { get; set; } = Environment.GetEnvironmentVariable("USER_EMAIL") ?? "john.doe@milochau.com";

        /// <summary>Whether the request is anonymous</summary>
        public bool AnonymousRequest { get; set; }

        /// <summary>Path parameters</summary>
        public Dictionary<string, string> PathParameters { get; set; } = [];

        /// <summary>Query string parameters</summary>
        public Dictionary<string, string> QueryStringParameters { get; set; } = [];
    }
}
