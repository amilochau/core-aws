using Milochau.Core.Aws.Abstractions;
using Milochau.Core.Aws.ApiGateway;
using Milochau.Core.Aws.Core.Lambda.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction
{
    public class FunctionRequest(IdentityUser? user) : MaybeAuthenticatedRequest(user), IParsableAndValidatable<FunctionRequest>
    {
        public static bool TryParse(APIGatewayHttpApiV2ProxyRequest request, [NotNullWhen(true)] out FunctionRequest? result)
        {
            if (!request.TryGetIdentityUser(out var user))
            {
                user = null; // Default value
            }
            result = new FunctionRequest(user);
            return true;
        }

        public void Validate(Dictionary<string, Collection<string>> modelStateDictionary)
        {
        }
    }
}
