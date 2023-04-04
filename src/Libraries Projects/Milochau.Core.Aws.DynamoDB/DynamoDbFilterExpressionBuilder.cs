using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>Filter expression builder for DynamoDB</summary>
    public class DynamoDbFilterExpressionBuilder
    {
        /// <summary>Equal filters</summary>
        public ICollection<string> Equal { get; set; } = new HashSet<string>();

        /// <summary>Contains filters</summary>
        public ICollection<string> Contains { get; set; } = new HashSet<string>();

        /// <summary>AttributeNotExists filters</summary>
        public ICollection<string> AttributeNotExists { get; set; } = new HashSet<string>();

        /// <summary>Get the expression attribute names as a dictionary</summary>
        public Dictionary<string, string> GetExpressionAttributeNames()
        {
            var expressionAttributeNames = new List<KeyValuePair<string, string>>();
            expressionAttributeNames.AddExpressionAttributeNames(Equal.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Contains.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(AttributeNotExists.ToArray());
            return expressionAttributeNames.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>Build the filter expression</summary>
        public string Build()
        {
            var items = new List<string>();
            items.AddRange(Equal.Select(x => $"#{x} = :v_{x}"));
            items.AddRange(Contains.Select(x => $"contains(#{x}, :v_{x})"));
            items.AddRange(AttributeNotExists.Select(x => $"attribute_not_exists(#{x})"));
            return new StringBuilder().AppendJoin(" AND ", items).ToString();
        }
    }
}
