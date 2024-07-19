using Milochau.Core.Aws.DynamoDB.Model;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    internal static class AttributeValueHelpers
    {
        public static bool IsSet(this AttributeValue attributeValue)
        {
            return attributeValue.BOOL != null && attributeValue.BOOL.Value
                || attributeValue.L != null && attributeValue.L.Count > 0 && attributeValue.L.Any(x => x.IsSet())
                || attributeValue.M != null && attributeValue.M.Count > 0 && attributeValue.M.Any(x => x.Value.IsSet())
                || attributeValue.N != null && !string.IsNullOrWhiteSpace(attributeValue.N)
                || attributeValue.NS != null && attributeValue.NS.Count > 0 && attributeValue.NS.Any(x => !string.IsNullOrWhiteSpace(x))
                || attributeValue.S != null && !string.IsNullOrWhiteSpace(attributeValue.S)
                || attributeValue.SS != null && attributeValue.SS.Count > 0 && attributeValue.SS.Any(x => !string.IsNullOrWhiteSpace(x));
        }

        public static Dictionary<string, AttributeValue> Sanitize(this Dictionary<string, AttributeValue> dictionary)
        {
            return dictionary.Where(x => x.Value.IsSet()).ToDictionary();
        }

        public static List<AttributeValue> Sanitize(this List<AttributeValue> list)
        {
            return list.Where(x => x.IsSet()).ToList();
        }
    }
}
