using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Model.Expressions
{
    /// <summary>Comparator type</summary>
    public enum ComparatorType
    {
        /// <summary>Less</summary>
        Less,
        /// <summary>LessOrEqual</summary>
        LessOrEqual,
        /// <summary>Greater</summary>
        Greater,
        /// <summary>GreaterOrEqual</summary>
        GreaterOrEqual,
    }

    /// <summary>Attribute type</summary>
    public enum AttributeType
    {
        /// <summary>String</summary>
        S,
        /// <summary>String Set</summary>
        SS,
        /// <summary>Number</summary>
        N,
        /// <summary>Number Set</summary>
        NS,
        /// <summary>Binary</summary>
        B,
        /// <summary>Binary Set</summary>
        BS,
        /// <summary>Boolean</summary>
        BOOL,
        /// <summary>Null</summary>
        NULL,
        /// <summary>List</summary>
        L,
        /// <summary>Map</summary>
        M,
    }

    /// <inheritdoc cref="EqualExpression"/>
    public class EqualValueExpression(AttributePathOperand path, AttributeValue value) : EqualExpression(path, new AttributeValueOperand(value)), IExpression, IQueryPartitionKeyConditionExpression, IQuerySortKeyConditionExpression { }

    /// <summary>Expression <c>path = operand</c></summary>
    public class EqualExpression(AttributePathOperand path, IOperand operand) : IExpression, IFilterExpression
    {
        private readonly AttributePathOperand path = path;
        private readonly IOperand operand = operand;

        /// <inheritdoc/>
        public string Expression => $"{path.Expression} = {operand.Expression}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operand.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues, .. operand.AttributeValues];
    }

    /// <summary>Expression <c>path &lt;&gt; operand</c></summary>
    public class NotEqualExpression(AttributePathOperand path, IOperand operand) : IExpression, IFilterExpression
    {
        private readonly AttributePathOperand path = path;
        private readonly IOperand operand = operand;

        /// <inheritdoc/>
        public string Expression => $"{path.Expression} <> {operand.Expression}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operand.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues, .. operand.AttributeValues];
    }

    /// <inheritdoc cref="ComparatorExpression"/>
    public class ComparatorValueExpression(AttributePathOperand path, ComparatorType comparator, AttributeValue value) : ComparatorExpression(path, comparator, new AttributeValueOperand(value)), IExpression, IQuerySortKeyConditionExpression { }

    /// <summary>Expression <c>path *comparator* operand</c></summary>
    public class ComparatorExpression(AttributePathOperand path, ComparatorType comparator, IOperand operand) : IExpression, IFilterExpression
    {
        private readonly AttributePathOperand path = path;
        private readonly ComparatorType comparator = comparator;
        private readonly IOperand operand = operand;

        /// <inheritdoc/>
        public string Expression => comparator switch
        {
            ComparatorType.Less => $"{path.Expression} < {operand.Expression}",
            ComparatorType.LessOrEqual => $"{path.Expression} <= {operand.Expression}",
            ComparatorType.Greater => $"{path.Expression} > {operand.Expression}",
            ComparatorType.GreaterOrEqual => $"{path.Expression} >= {operand.Expression}",
            _ => $"{path.Expression} = {operand.Expression}",
        };

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operand.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues, .. operand.AttributeValues];
    }

    /// <inheritdoc cref="BetweenExpression"/>
    public class BetweenValueExpression(AttributePathOperand path, AttributeValue valueLow, AttributeValue valueHigh) : BetweenExpression(path, new AttributeValueOperand(valueLow), new AttributeValueOperand(valueHigh)), IExpression, IQuerySortKeyConditionExpression { }

    /// <summary>Expression <c>path BETWEEN operand AND operand</c></summary>
    public class BetweenExpression(AttributePathOperand path, IOperand operandLow, IOperand operandHigh) : IExpression, IFilterExpression
    {
        private readonly AttributePathOperand path = path;
        private readonly IOperand operandLow = operandLow;
        private readonly IOperand operandHigh = operandHigh;

        /// <inheritdoc/>
        public string Expression => $"{path.Expression} BETWEEN {operandLow.Expression} AND {operandHigh.Expression}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operandLow.AttributeNames, .. operandHigh.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues, .. operandLow.AttributeValues, .. operandHigh.AttributeValues];
    }

    /// <summary>Expression <c>path IN (operand1, operand2, ...)</c></summary>
    public class InExpression(AttributePathOperand path, params IOperand[] operands) : IExpression, IFilterExpression
    {
        private readonly AttributePathOperand path = path;
        private readonly IOperand[] operands = operands;

        /// <inheritdoc/>
        public string Expression => new StringBuilder().Append($"{path.Expression} IN (").AppendJoin(", ", operands.Select(x => x.Expression)).Append(')').ToString();

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operands.SelectMany(x => x.AttributeNames)];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues, .. operands.SelectMany(x => x.AttributeValues)];
    }

    /// <summary>Expression <c>attribute_exists(path)</c></summary>
    public class AttributeExistsExpression(AttributePathOperand path) : IExpression, IFilterExpression
    {
        private readonly AttributePathOperand path = path;

        /// <inheritdoc/>
        public string Expression => $"attribute_exists({path.Expression})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues];
    }

    /// <summary>Expression <c>attribute_not_exists(path)</c></summary>
    public class AttributeNotExistsExpression(AttributePathOperand path) : IExpression, IFilterExpression
    {
        private readonly AttributePathOperand path = path;

        /// <inheritdoc/>
        public string Expression => $"attribute_not_exists({path.Expression})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues];
    }

    /// <summary>Expression <c>attribute_type(path, type)</c></summary>
    public class AttributeTypeExpression(AttributePathOperand path, AttributeType type) : IExpression, IFilterExpression
    {
        private readonly string id = Guid.NewGuid().ToString("N");

        private readonly AttributePathOperand path = path;
        private readonly AttributeType type = type;

        /// <inheritdoc/>
        public string Expression => $"attribute_type({path.Expression}, :v_{id})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues, new($":v_{id}", type.ToString("G"))];
    }

    /// <summary>Expression <c>begins_with(path, substring)</c></summary>
    public class BeginsWithExpression(AttributePathOperand path, string substring) : IExpression, IQuerySortKeyConditionExpression
    {
        private readonly string id = Guid.NewGuid().ToString("N");

        private readonly AttributePathOperand path = path;
        private readonly string substring = substring;

        /// <inheritdoc/>
        public string Expression => $"begins_with({path.Expression}, :v_{id})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues, new($":v_{id}", substring)];
    }

    /// <summary>Expression <c>contains(path, operand)</c></summary>
    public class ContainsExpression(AttributePathOperand path, IOperand operand) : IExpression, IFilterExpression
    {
        private readonly AttributePathOperand path = path;
        private readonly IOperand operand = operand;

        /// <inheritdoc/>
        public string Expression => $"contains({path.Expression}, {operand.Expression})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operand.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. path.AttributeValues, .. operand.AttributeValues];
    }

    /// <remarks>Not implemented</remarks>
    public class SizeExpression
    {
        // @todo This class is not implemented - see https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.OperatorsAndFunctions.html
    }

    /// <summary>Expression <c>expression1 AND expression2 AND ...</c></summary>
    public class AndExpression(params IExpression?[] expressions) : IExpression, IFilterExpression
    {
        private readonly IExpression[] expressions = expressions.Where(x => x != null).Cast<IExpression>().ToArray();

        /// <inheritdoc/>
        public string Expression => new StringBuilder().AppendJoin(" AND ", expressions.Select(x => x.Expression)).ToString();

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => expressions.SelectMany(x => x.AttributeNames).ToList();

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => expressions.SelectMany(x => x.AttributeValues).ToList();
    }

    /// <summary>Expression <c>expression1 OR expression2 OR ...</c></summary>
    public class OrExpression(params IExpression?[] expressions) : IExpression, IFilterExpression
    {
        private readonly IExpression[] expressions = expressions.Where(x => x != null).Cast<IExpression>().ToArray();

        /// <inheritdoc/>
        public string Expression => new StringBuilder().AppendJoin(" OR ", expressions.Select(x => x.Expression)).ToString();

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => expressions.SelectMany(x => x.AttributeNames).ToList();

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => expressions.SelectMany(x => x.AttributeValues).ToList();
    }

    /// <summary>Expression <c>NOT expression</c></summary>
    public class NotExpression(IExpression expression) : IExpression, IFilterExpression
    {
        private readonly IExpression expression = expression;

        /// <inheritdoc/>
        public string Expression => $"NOT {expression.Expression}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => expression.AttributeNames;

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => expression.AttributeValues;
    }

    /// <summary>Expression <c>(expression)</c></summary>
    public class ParenthesesExpression(IExpression expression) : IExpression, IFilterExpression
    {
        private readonly IExpression expression = expression;

        /// <inheritdoc/>
        public string Expression => $"({expression.Expression})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => expression.AttributeNames;

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => expression.AttributeValues;
    }
}
