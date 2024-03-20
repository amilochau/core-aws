using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Milochau.Core.Aws.DynamoDB.Model.Expressions
{
    /// <summary>Update expression</summary>
    public class UpdateExpression : IExpression
    {
        /// <summary>Set expressions</summary>
        public List<SetUpdateExpression>? SetExpressions { get; set; }

        /// <summary>Remove expressions</summary>
        public List<RemoveUpdateExpression>? RemoveExpressions { get; set; }

        /// <summary>Add expressions</summary>
        public List<AddUpdateExpression>? AddExpressions { get; set; }

        /// <summary>Delete expressions</summary>
        public List<DeleteUpdateExpression>? DeleteExpressions { get; set; }

        /// <summary>Build expression</summary>
        public string Build()
        {
            var expression = new StringBuilder();
            var setExpression = new StringBuilder();
            var removeExpression = new StringBuilder();
            var addExpression = new StringBuilder();
            var deleteExpression = new StringBuilder();
            if (SetExpressions != null && SetExpressions.Count > 0)
            {
                setExpression.Append("SET ").AppendJoin(", ", SetExpressions.Select(x => x.Build()));
            }
            if (RemoveExpressions != null && RemoveExpressions.Count > 0)
            {
                removeExpression.Append("REMOVE ").AppendJoin(", ", RemoveExpressions.Select(x => x.Build()));
            }
            if (AddExpressions != null && AddExpressions.Count > 0)
            {
                addExpression.Append("ADD ").AppendJoin(", ", AddExpressions.Select(x => x.Build()));
            }
            if (DeleteExpressions != null && DeleteExpressions.Count > 0)
            {
                deleteExpression.Append("DELETE ").AppendJoin(", ", DeleteExpressions.Select(x => x.Build()));
            }

            expression.AppendJoin(' ', setExpression, removeExpression, addExpression, deleteExpression);
            return expression.ToString();
        }

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [
                ..SetExpressions?.SelectMany(x => x.AttributeNames).ToList() ?? [],
                ..RemoveExpressions?.SelectMany(x => x.AttributeNames).ToList() ?? [],
                ..AddExpressions?.SelectMany(x => x.AttributeNames).ToList() ?? [],
                ..DeleteExpressions?.SelectMany(x => x.AttributeNames).ToList() ?? [],
            ];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [
                ..SetExpressions?.SelectMany(x => x.AttributeValues).ToList() ?? [],
                ..RemoveExpressions?.SelectMany(x => x.AttributeValues).ToList() ?? [],
                ..AddExpressions?.SelectMany(x => x.AttributeValues).ToList() ?? [],
                ..DeleteExpressions?.SelectMany(x => x.AttributeValues).ToList() ?? [],
            ];
    }


    /// <summary>
    /// Expression <c>SET path value</c>
    /// <list type="bullet">
    /// <item><description>
    /// Add one or more attributes to an item
    /// </description></item>
    /// <item><description>
    /// If any of these attributes already exists, they are overwritten by the new values
    /// </description></item>
    /// <item><description>
    /// Add or subtract from an attribute that is of type Number
    /// </description></item>
    /// </list>
    /// </summary>
    /// <remarks><see href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.UpdateExpressions.html#Expressions.UpdateExpressions.SET"/></remarks>
    public class SetUpdateExpression(AttributePath path, AttributeOperand operand) : IUpdateExpression
    {
        private readonly AttributePath path = path;
        private readonly AttributeOperand operand = operand;

        /// <inheritdoc/>
        public string Build() => $"{path.AttributeName} = {operand.AttributeName ?? path.AttributeValueName}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operand.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => operand.Value == null ? [] : [new(path.AttributeValueName, operand.Value)];
    }

    /// <summary>
    /// Expression <c>REMOVE path</c>
    /// <list type="bullet">
    /// <item><description>
    /// Remove one or more attributes from an item
    /// </description></item>
    /// <item><description>
    /// Delete individual elements from a list
    /// </description></item>
    /// </list>
    /// </summary>
    /// <remarks><see href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.UpdateExpressions.html#Expressions.UpdateExpressions.REMOVE"/></remarks>
    public class RemoveUpdateExpression(AttributePath path) : IUpdateExpression
    {
        private readonly AttributePath path = path;

        /// <inheritdoc/>
        public string Build() => path.AttributeName;

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [];
    }

    /// <summary>
    /// Expression <c>ADD path value</c>
    /// <list type="bullet">
    /// <item><description>
    /// Add a new attribute and its values to an item
    /// </description></item>
    /// <item><description>
    /// If the attribute is a number, and the value you are adding is also a number, the value is mathematically added to the existing attribute
    /// </description></item>
    /// <item><description>
    /// If the attribute is a set, and the value you are adding is also a set, the value is appended to the existing set
    /// </description></item>
    /// </list>
    /// </summary>
    /// <remarks><see href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.UpdateExpressions.html#Expressions.UpdateExpressions.ADD"/></remarks>
    public class AddUpdateExpression(AttributePath path, AttributeValue value) : IUpdateExpression
    {
        private readonly AttributePath path = path;
        private readonly AttributeValue value = value;

        /// <inheritdoc/>
        public string Build() => $"{path.AttributeName} {path.AttributeValueName}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [new(path.AttributeValueName, value)];
    }

    /// <summary>
    /// Expression <c>DELETE path subset</c>
    /// <list type="bullet">
    /// <item><description>
    /// Remove one or more elements from a set
    /// </description></item>
    /// </list>
    /// </summary>
    /// <remarks><see href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.UpdateExpressions.html#Expressions.UpdateExpressions.DELETE"/></remarks>
    public class DeleteUpdateExpression(AttributePath path, AttributeValue subset) : IUpdateExpression
    {
        private readonly AttributePath path = path;
        private readonly AttributeValue subset = subset;

        /// <inheritdoc/>
        public string Build() => $"{path.AttributeName} {path.AttributeValueName}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [new(path.AttributeValueName, subset)];
    }
}
