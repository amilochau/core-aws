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
    public class EqualValueExpression(AttributePath path, AttributeValue value) : EqualExpression(path, value), IExpression, IQueryPartitionKeyConditionExpression, IQuerySortKeyConditionExpression { }

    /// <summary>Expression <c>path = operand</c></summary>
    public class EqualExpression(AttributePath path, AttributeOperand operand) : IExpression, IFilterExpression
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

    /// <summary>Expression <c>path &lt;&gt; operand</c></summary>
    public class NotEqualExpression(AttributePath path, AttributeOperand operand) : IExpression, IFilterExpression
    {
        private readonly AttributePath path = path;
        private readonly AttributeOperand operand = operand;

        /// <inheritdoc/>
        public string Build() => $"{path.AttributeName} <> {operand.AttributeName ?? path.AttributeValueName}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operand.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => operand.Value == null ? [] : [new(path.AttributeValueName, operand.Value)];
    }

    /// <inheritdoc cref="ComparatorExpression"/>
    public class ComparatorValueExpression(AttributePath path, ComparatorType comparator, AttributeValue value) : ComparatorExpression(path, comparator, value), IExpression, IQuerySortKeyConditionExpression { }

    /// <summary>Expression <c>path *comparator* operand</c></summary>
    public class ComparatorExpression(AttributePath path, ComparatorType comparator, AttributeOperand operand) : IExpression, IFilterExpression
    {
        private readonly AttributePath path = path;
        private readonly ComparatorType comparator = comparator;
        private readonly AttributeOperand operand = operand;

        /// <inheritdoc/>
        public string Build() => comparator switch
        {
            ComparatorType.Less => $"{path.AttributeName} < {operand.AttributeName ?? path.AttributeValueName}",
            ComparatorType.LessOrEqual => $"{path.AttributeName} <= {operand.AttributeName ?? path.AttributeValueName}",
            ComparatorType.Greater => $"{path.AttributeName} > {operand.AttributeName ?? path.AttributeValueName}",
            ComparatorType.GreaterOrEqual => $"{path.AttributeName} >= {operand.AttributeName ?? path.AttributeValueName}",
            _ => $"{path.AttributeName} = {operand.AttributeName ?? path.AttributeValueName}",
        };

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operand.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => operand.Value == null ? [] : [new(path.AttributeValueName, operand.Value)];
    }

    /// <inheritdoc cref="BetweenExpression"/>
    public class BetweenValueExpression(AttributePath path, AttributeValue valueLow, AttributeValue valueHigh) : BetweenExpression(path, valueLow, valueHigh), IExpression, IQuerySortKeyConditionExpression { }

    /// <summary>Expression <c>path BETWEEN operand AND operand</c></summary>
    public class BetweenExpression(AttributePath path, AttributeOperand operandLow, AttributeOperand operandHigh) : IExpression, IFilterExpression
    {
        private readonly AttributePath path = path;
        private readonly AttributeOperand operandLow = operandLow;
        private readonly AttributeOperand operandHigh = operandHigh;

        /// <inheritdoc/>
        public string Build() => $"{path.AttributeName} BETWEEN {operandLow.AttributeName ?? $"{path.AttributeValueName}_1"} AND {operandHigh.AttributeName ?? $"{path.AttributeValueName}_2"}";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operandLow.AttributeNames, .. operandHigh.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [
            ..operandLow.Value == null ? new List<KeyValuePair<string, AttributeValue>>() : [new($"{path.AttributeValueName}_1", operandLow.Value)],
            ..operandHigh.Value == null ? new List<KeyValuePair<string, AttributeValue>>() : [new($"{path.AttributeValueName}_2", operandHigh.Value)]
        ];
    }

    /// <summary>Expression <c>path IN (operand1, operand2, ...)</c></summary>
    public class InExpression(AttributePath path, params AttributeOperand[] operands) : IExpression, IFilterExpression
    {
        private readonly AttributePath path = path;
        private readonly AttributeOperand[] operands = operands;

        /// <inheritdoc/>
        public string Build() => new StringBuilder().Append($"{path.AttributeName} IN (").AppendJoin(", ", operands.Select((x, i) => x.AttributeName ?? $"{path.AttributeValueName}_{i + 1}")).Append(')').ToString();

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operands.SelectMany(x => x.AttributeNames).ToList()];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => operands.Where(x => x.Value != null).Select<AttributeOperand, KeyValuePair<string, AttributeValue>>((x, i) => new($"{path.AttributeValueName}_{i + 1}", x.Value!)).ToList();
    }

    /// <summary>Expression <c>attribute_exists(path)</c></summary>
    public class AttributeExistsExpression(AttributePath path) : IExpression, IFilterExpression
    {
        private readonly AttributePath path = path;

        /// <inheritdoc/>
        public string Build() => $"attribute_exists({path.AttributeName})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [];
    }

    /// <summary>Expression <c>attribute_not_exists(path)</c></summary>
    public class AttributeNotExistsExpression(AttributePath path) : IExpression, IFilterExpression
    {
        private readonly AttributePath path = path;

        /// <inheritdoc/>
        public string Build() => $"attribute_not_exists({path.AttributeName})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [];
    }

    /// <summary>Expression <c>attribute_type(path, type)</c></summary>
    public class AttributeTypeExpression(AttributePath path, AttributeType type) : IExpression, IFilterExpression
    {
        private readonly AttributePath path = path;
        private readonly AttributeType type = type;

        /// <inheritdoc/>
        public string Build() => $"attribute_type({path.AttributeName}, {path.AttributeValueName})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [new(path.AttributeValueName, type.ToString("G"))];
    }

    /// <summary>Expression <c>begins_with(path, substring)</c></summary>
    public class BeginsWithExpression(AttributePath path, string substring) : IExpression, IQuerySortKeyConditionExpression
    {
        private readonly AttributePath path = path;
        private readonly string substring = substring;

        /// <inheritdoc/>
        public string Build() => $"begins_with({path.AttributeName}, {path.AttributeValueName})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [new(path.AttributeValueName, substring)];
    }

    /// <summary>Expression <c>contains(path, operand)</c></summary>
    public class ContainsExpression(AttributePath path, AttributeOperand operand) : IExpression, IFilterExpression
    {
        private readonly AttributePath path = path;
        private readonly AttributeOperand operand = operand;

        /// <inheritdoc/>
        public string Build() => $"contains({path.AttributeName}, {operand.AttributeName ?? path.AttributeValueName})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => [.. path.AttributeNames, .. operand.AttributeNames];

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => operand.Value == null ? [] : [new(path.AttributeValueName, operand.Value)];
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
        public string Build() => new StringBuilder().AppendJoin(" AND ", expressions.Select(x => x.Build())).ToString();

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
        public string Build() => new StringBuilder().AppendJoin(" OR ", expressions.Select(x => x.Build())).ToString();

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
        public string Build() => $"NOT {expression.Build()}";

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
        public string Build() => $"({expression.Build()})";

        /// <inheritdoc/>
        public List<KeyValuePair<string, string>> AttributeNames => expression.AttributeNames;

        /// <inheritdoc/>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => expression.AttributeValues;
    }
}
