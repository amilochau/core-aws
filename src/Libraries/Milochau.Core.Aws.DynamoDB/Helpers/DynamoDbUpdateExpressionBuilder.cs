using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Update expression builder for DynamoDB</summary>
    [System.Obsolete]
    public class DynamoDbUpdateExpressionBuilder
    {
        /// <summary>Set update expressions</summary>
        public ICollection<string> Set { get; set; } = new HashSet<string>();

        /// <summary>Remove update expressions</summary>
        public ICollection<string> Remove { get; set; } = new HashSet<string>();

        /// <summary>Add update expressions</summary>
        public ICollection<string> Add { get; set; } = new HashSet<string>();

        /// <summary>Delete update expressions</summary>
        public ICollection<string> Delete { get; set; } = new HashSet<string>();

        /// <summary>Get the expression attribute names as a dictionary</summary>
        public Dictionary<string, string> GetExpressionAttributeNames()
        {
            var expressionAttributeNames = new List<KeyValuePair<string, string>>();
            expressionAttributeNames.AddExpressionAttributeNames(Set.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Remove.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Add.ToArray());
            expressionAttributeNames.AddExpressionAttributeNames(Delete.ToArray());
            return expressionAttributeNames.ToDictionary();
        }

        /// <summary>Build the update expression</summary>
        public string Build()
        {
            var updateExpressionItems = new List<string>();
            if (Set.Count != 0)
            {
                var setItems = new StringBuilder().AppendJoin(", ", Set.Select(x => $"#{x} = :v_{x}"));
                updateExpressionItems.Add($"SET {setItems}");
            }
            if (Remove.Count != 0)
            {
                var removeItems = new StringBuilder().AppendJoin(", ", Remove.Select(x => $"#{x}"));
                updateExpressionItems.Add($"REMOVE {removeItems}");
            }
            if (Add.Count != 0)
            {
                var addItems = new StringBuilder().AppendJoin(", ", Add.Select(x => $"#{x} :v_{x}"));
                updateExpressionItems.Add($"ADD {addItems}");
            }
            if (Delete.Count != 0)
            {
                var deleteItems = new StringBuilder().AppendJoin(", ", Delete.Select(x => $"#{x} :v_{x}"));
                updateExpressionItems.Add($"DELETE {deleteItems}");
            }
            return new StringBuilder().AppendJoin(" ", updateExpressionItems).ToString();
        }
    }
}
