using Milochau.Core.Aws.ApiGateway;
using Milochau.Core.Aws.ApiGateway.APIGatewayEvents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction
{
    public class LambdaFunctionRequest : MaybeAuthenticatedRequest, IParsableAndValidatable<LambdaFunctionRequest>
    {
        public static bool TryParse(APIGatewayHttpApiV2ProxyRequest request, [NotNullWhen(true)] out LambdaFunctionRequest? result)
        {
            result = new LambdaFunctionRequest();

            return result.ParseAuthentication(request);
        }

        public void Validate(Dictionary<string, Collection<string>> modelStateDictionary)
        {
        }
    }
}
