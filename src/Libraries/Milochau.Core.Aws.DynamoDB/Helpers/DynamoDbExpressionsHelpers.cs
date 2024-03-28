using Milochau.Core.Aws.DynamoDB.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Helpers
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
            return allAttributes.ToDictionary();
        }

        /// <summary>Build a dictionary of expression attribute names from an enumerable of attributes</summary>
        public static Dictionary<string, string> BuildExpressionAttributeNames(params string[] attributes)
        {
            var expressionAttributeNames = new List<KeyValuePair<string, string>>();
            expressionAttributeNames.AddExpressionAttributeNames(attributes);
            return expressionAttributeNames.ToDictionary();
        }

        /// <summary>Add expression attribute names from an enumerable of attributes to an existing dictionary</summary>
        public static ICollection<KeyValuePair<string, string>> AddExpressionAttributeNames(this ICollection<KeyValuePair<string, string>> expressionAttributeNames, params string[] attributes)
        {
            foreach (var attribute in attributes)
            {
                expressionAttributeNames.Add(new KeyValuePair<string, string>($"#{attribute}", attribute));
            }
            return expressionAttributeNames;
        }

        /// <summary>Add expression attribute names from an enumerable of attributes to an existing dictionary</summary>
        public static ICollection<KeyValuePair<string, AttributeValue>> AddExpressionAttributeValues(this ICollection<KeyValuePair<string, AttributeValue>> expressionAttributeValues, params KeyValuePair<string, AttributeValue>[] attributes)
        {
            foreach (var attribute in attributes.Where(x => x.Value.IsSet()))
            {
                expressionAttributeValues.Add(new KeyValuePair<string, AttributeValue>($":v_{attribute.Key}", attribute.Value));
            }
            return expressionAttributeValues;
        }
    }
}
