using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>Key condition expression builder for DynamoDB</summary>
    public class DynamoDbKeyConditionExpressionBuilder
    {
        /// <summary>Equal condition expressions</summary>
        public ICollection<string> Equal { get; set; } = new HashSet<string>();

        /// <summary>Smaller condition expressions</summary>
        public ICollection<string> Smaller { get; set; } = new HashSet<string>();

        /// <summary>Greater condition expressions</summary>
        public ICollection<string> Greater { get; set; } = new HashSet<string>();

        /// <summary>Get the expression attribute names as a dictionary</summary>
        public Dictionary<string, string> GetExpressionAttributeNames()
        {
            var expressionAttributeNames = new List<KeyValuePair<string, string>>();
            expressionAttributeNames.AddExpressionAttributeNames(Equal.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Smaller.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Greater.ToArray());
            return expressionAttributeNames.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>Build the key condition expression</summary>
        public string Build()
        {
            var items = new List<string>();
            items.AddRange(Equal.Select(x => $"#{x} = :v_{x}"));
            items.AddRange(Smaller.Select(x => $"#{x} < :v_{x}"));
            items.AddRange(Greater.Select(x => $"#{x} > :v_{x}"));
            return new StringBuilder().AppendJoin(" AND ", items).ToString();
        }
    }
}
