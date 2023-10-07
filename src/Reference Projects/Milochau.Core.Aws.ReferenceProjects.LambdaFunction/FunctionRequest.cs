using Milochau.Core.Aws.ApiGateway;
using Milochau.Core.Aws.ApiGateway.APIGatewayEvents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction
{
    public class FunctionRequest : MaybeAuthenticatedRequest, IParsableAndValidatable<FunctionRequest>
    {
        public static bool TryParse(APIGatewayHttpApiV2ProxyRequest request, [NotNullWhen(true)] out FunctionRequest? result)
        {
            result = new FunctionRequest();

            return result.ParseAuthentication(request);
        }

        public void Validate(Dictionary<string, Collection<string>> modelStateDictionary)
        {
        }
    }
}
