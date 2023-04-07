using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>Filter expression builder for DynamoDB</summary>
    public class DynamoDbFilterExpressionBuilder
    {
        /// <summary>'Equal to' filters</summary>
        public ICollection<string> Equal { get; set; } = new HashSet<string>();

        /// <summary>'Not equal to' filters</summary>
        public ICollection<string> NotEqual { get; set; } = new HashSet<string>();

        /// <summary>'Less than' filters</summary>
        public ICollection<string> Less { get; set; } = new HashSet<string>();

        /// <summary>'Less than or equal to' filters</summary>
        public ICollection<string> LessOrEqual { get; set; } = new HashSet<string>();

        /// <summary>'Greater than' filters</summary>
        public ICollection<string> Greater { get; set; } = new HashSet<string>();

        /// <summary>'Greater than or equal to' filters</summary>
        public ICollection<string> GreaterOrEqual { get; set; } = new HashSet<string>();

        /// <summary>'Between' filters</summary>
        public ICollection<string> Between { get; set; } = new HashSet<string>();


        /// <summary>'Attribute exists' filters</summary>
        public ICollection<string> AttributeExists { get; set; } = new HashSet<string>();

        /// <summary>'Attribute not exists' filters</summary>
        public ICollection<string> AttributeNotExists { get; set; } = new HashSet<string>();

        /// <summary>'Attribute type' filters</summary>
        public ICollection<string> AttributeType { get; set; } = new HashSet<string>();

        /// <summary>'Begins with' filters</summary>
        public ICollection<string> BeginsWith { get; set; } = new HashSet<string>();

        /// <summary>'Contains' filters</summary>
        public ICollection<string> Contains { get; set; } = new HashSet<string>();


        /// <summary>Get the expression attribute names as a dictionary</summary>
        public Dictionary<string, string> GetExpressionAttributeNames()
        {
            var expressionAttributeNames = new HashSet<KeyValuePair<string, string>>(new KeyValuePairEqualityComparer());
            expressionAttributeNames.AddExpressionAttributeNames(Equal.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(NotEqual.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Less.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(LessOrEqual.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Greater.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(GreaterOrEqual.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Between.ToArray());

            expressionAttributeNames.AddExpressionAttributeNames(AttributeExists.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(AttributeNotExists.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(AttributeType.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(BeginsWith.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Contains.ToArray());
            return expressionAttributeNames.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>Build the filter expression</summary>
        public string Build()
        {
            var items = new List<string>();
            items.AddRange(Equal.Select(x => $"#{x} = :v_{x}"));
            items.AddRange(NotEqual.Select(x => $"#{x} <> :v_{x}"));
            items.AddRange(Less.Select(x => $"#{x} < :v_{x}"));
            items.AddRange(LessOrEqual.Select(x => $"#{x} <= :v_{x}"));
            items.AddRange(Greater.Select(x => $"#{x} > :v_{x}"));
            items.AddRange(GreaterOrEqual.Select(x => $"#{x} >= :v_{x}"));
            items.AddRange(Between.Select(x => $"#{x} BETWEEN :v_{x}_1 AND :v_{x}_2"));

            items.AddRange(AttributeExists.Select(x => $"attribute_exists(#{x})"));
            items.AddRange(AttributeNotExists.Select(x => $"attribute_not_exists(#{x})"));
            items.AddRange(AttributeType.Select(x => $"attribute_type(#{x}, :v_{x})"));
            items.AddRange(BeginsWith.Select(x => $"begins_with(#{x}, :v_{x})"));
            items.AddRange(Contains.Select(x => $"contains(#{x}, :v_{x})"));
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
