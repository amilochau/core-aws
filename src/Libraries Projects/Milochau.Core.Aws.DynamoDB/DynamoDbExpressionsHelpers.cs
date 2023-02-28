using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>Helpers with DynamoDB expressions</summary>
    public static class DynamoDbExpressionsHelpers
    {
        /// <summary>Build a projection expression from an enumerable of attributes</summary>
        public static string BuildProjectionExpression(IEnumerable<string> attributes, params string[] more)
        {
            var allAttributes = attributes.Union(more).Select(x => $"#{x}");
            return new StringBuilder().AppendJoin(", ", allAttributes).ToString();
        }

        /// <summary>Build a dictionary of expression attribute names from an enumerable of attributes</summary>
        public static Dictionary<string, string> BuildExpressionAttributeNames(IEnumerable<string> attributes, params string[] more)
        {
            var allAttributes = attributes.Union(more).Select(x => new KeyValuePair<string, string>($"#{x}", x));
            return allAttributes.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
