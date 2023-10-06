using System;
using System.Collections.Generic;

namespace Amazon.Runtime.Internal
{
    public interface IAmazonWebServiceRequest
    {
        SignatureVersion SignatureVersion { get; set; }
    }
}
