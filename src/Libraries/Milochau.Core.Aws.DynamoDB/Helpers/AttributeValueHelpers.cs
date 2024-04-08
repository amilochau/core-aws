using Milochau.Core.Aws.DynamoDB.Model;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    internal static class AttributeValueHelpers
    {
        public static bool IsSet(this AttributeValue attributeValue)
        {
            if (attributeValue.B != null)
            {
                return true;
            }

            if (attributeValue.BOOL != null && attributeValue.BOOL.Value)
            {
                return true;
            }

            if (attributeValue.BS != null && attributeValue.BS.Count > 0 && attributeValue.BS.Any(x => x != null))
            {
                return true;
            }

            if (attributeValue.L != null && attributeValue.L.Count > 0 && attributeValue.L.Any(x => x.IsSet()))
            {
                return true;
            }

            if (attributeValue.M != null && attributeValue.M.Count > 0 && attributeValue.M.Any(x => x.Value.IsSet()))
            {
                return true;
            }

            if (attributeValue.N != null && !string.IsNullOrWhiteSpace(attributeValue.N))
            {
                return true;
            }

            if (attributeValue.NS != null && attributeValue.NS.Count > 0 && attributeValue.NS.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                return true;
            }

            if (attributeValue.NULL != null && attributeValue.NULL.Value)
            {
                return true;
            }

            if (attributeValue.S != null && !string.IsNullOrWhiteSpace(attributeValue.S))
            {
                return true;
            }

            if (attributeValue.SS != null && attributeValue.SS.Count > 0 && attributeValue.SS.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                return true;
            }

            return false;
        }
    }
}
