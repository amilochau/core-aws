using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Key condition expression builder for DynamoDB</summary>
    public class DynamoDbKeyConditionExpressionBuilder
    {
        /// <summary>'Equal to' condition expressions</summary>
        public ICollection<string> Equal { get; set; } = new HashSet<string>();

        /// <summary>'Less than' condition expressions</summary>
        public ICollection<string> Less { get; set; } = new HashSet<string>();

        /// <summary>'Less than or equal to' condition expressions</summary>
        public ICollection<string> LessOrEqual { get; set; } = new HashSet<string>();

        /// <summary>'Greater than' condition expressions</summary>
        public ICollection<string> Greater { get; set; } = new HashSet<string>();

        /// <summary>'Greater than or equal to' condition expressions</summary>
        public ICollection<string> GreaterOrEqual { get; set; } = new HashSet<string>();

        /// <summary>'Between' condition expressions</summary>
        public ICollection<string> Between { get; set; } = new HashSet<string>();

        /// <summary>'Begins with' condition expressions</summary>
        public ICollection<string> BeginsWith { get; set; } = new HashSet<string>();

        /// <summary>Get the expression attribute names as a dictionary</summary>
        public Dictionary<string, string> GetExpressionAttributeNames()
        {
            var expressionAttributeNames = new HashSet<KeyValuePair<string, string>>(new KeyValuePairEqualityComparer());
            expressionAttributeNames.AddExpressionAttributeNames(Equal.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Less.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(LessOrEqual.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Greater.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(GreaterOrEqual.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Between.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(BeginsWith.ToArray());
            return expressionAttributeNames.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>Build the key condition expression</summary>
        public string Build()
        {
            var items = new List<string>();
            items.AddRange(Equal.Select(x => $"#{x} = :v_{x}"));
            items.AddRange(Less.Select(x => $"#{x} < :v_{x}"));
            items.AddRange(LessOrEqual.Select(x => $"#{x} <= :v_{x}"));
            items.AddRange(Greater.Select(x => $"#{x} > :v_{x}"));
            items.AddRange(GreaterOrEqual.Select(x => $"#{x} >= :v_{x}"));
            items.AddRange(Between.Select(x => $"#{x} BETWEEN :v_{x}_1 AND :v_{x}_2"));
            items.AddRange(BeginsWith.Select(x => $"begins_with(#{x}, :v_{x})"));
            return new StringBuilder().AppendJoin(" AND ", items).ToString();
        }

        /// <summary>Equality comparer to compare key value pairs, based on keys</summary>
        private class KeyValuePairEqualityComparer : IEqualityComparer<KeyValuePair<string, string>>
        {
            /// <summary>Whether the two items are equal</summary>
            public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
            {
                return x.Key.Equals(y.Key, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>The hash code of an item</summary>
            public int GetHashCode([DisallowNull] KeyValuePair<string, string> obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
