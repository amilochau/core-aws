using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable CS1591 // @todo
namespace Milochau.Core.Aws.DynamoDB.Model.External
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

    /// <summary>Expression, to be used in conditions and filters</summary>
    /// <remarks>See <see href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.OperatorsAndFunctions.html"/></remarks>
    public interface IExpression
    {
        string Build();
        List<KeyValuePair<string, string>> GetAttributeNames();
        List<KeyValuePair<string, AttributeValue>> GetAttributeValues();
    }

    public interface IQueryPartitionKeyConditionExpression : IExpression { }
    public interface IQuerySortKeyConditionExpression : IExpression { }
    public interface IQueryFilterExpression : IExpression { }

    public class EqualExpression(AttributePath path, AttributeOperand operand) : IExpression, IQueryPartitionKeyConditionExpression, IQuerySortKeyConditionExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;
        public AttributeOperand Operand { get; set; } = operand;

        public string Build() => $"{Path.AttributeName} = {Operand.AttributeName ?? Path.AttributeValueName}";

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames, .. Operand.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Operand.Value == null ? [] : [new(Path.AttributeValueName, Operand.Value)];
    }

    public class NotEqualExpression(AttributePath path, AttributeOperand operand) : IExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;
        public AttributeOperand Operand { get; set; } = operand;

        public string Build() => $"{Path.AttributeName} <> {Operand.AttributeName ?? Path.AttributeValueName}";

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames, .. Operand.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Operand.Value == null ? [] : [new(Path.AttributeValueName, Operand.Value)];
    }

    public class ComparatorExpression(AttributePath path, ComparatorType comparator, AttributeOperand operand) : IExpression, IQuerySortKeyConditionExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;
        public ComparatorType Comparator { get; set; } = comparator;
        public AttributeOperand Operand { get; set; } = operand;

        public string Build() => Comparator switch
        {
            ComparatorType.Less => $"{Path.AttributeName} < {Operand.AttributeName ?? Path.AttributeValueName}",
            ComparatorType.LessOrEqual => $"{Path.AttributeName} <= {Operand.AttributeName ?? Path.AttributeValueName}",
            ComparatorType.Greater => $"{Path.AttributeName} > {Operand.AttributeName ?? Path.AttributeValueName}",
            ComparatorType.GreaterOrEqual => $"{Path.AttributeName} >= {Operand.AttributeName ?? Path.AttributeValueName}",
            _ => $"{Path.AttributeName} = {Operand.AttributeName ?? Path.AttributeValueName}",
        };

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames, .. Operand.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Operand.Value == null ? [] : [new(Path.AttributeValueName, Operand.Value)];
    }

    public class BetweenExpression(AttributePath path, AttributeOperand operandLow, AttributeOperand operandHigh) : IExpression, IQuerySortKeyConditionExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;
        public AttributeOperand OperandLow { get; set; } = operandLow;
        public AttributeOperand OperandHigh { get; set; } = operandHigh;

        public string Build() => $"{Path.AttributeName} >= {OperandLow.AttributeName ?? $"{Path.AttributeValueName}_1"} AND {OperandHigh.AttributeName ?? $"{Path.AttributeValueName}_2"}";

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames, .. OperandLow.AttributeNames, .. OperandHigh.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => [
            ..OperandLow.Value == null ? new List<KeyValuePair<string, AttributeValue>>() : [new($"{Path.AttributeValueName}_1", OperandLow.Value)],
            ..OperandHigh.Value == null ? new List<KeyValuePair<string, AttributeValue>>() : [new($"{Path.AttributeValueName}_2", OperandHigh.Value)]
        ];
    }

    public class InExpression(AttributePath path, params AttributeOperand[] operands) : IExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;
        public AttributeOperand[] Operands { get; set; } = operands;

        public string Build() => new StringBuilder().Append($"{Path.AttributeName} IN (").AppendJoin(", ", Operands.Select((x, i) => x.AttributeName ?? $"{Path.AttributeValueName}_{i + 1}")).Append(')').ToString();

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames, .. Operands.SelectMany(x => x.AttributeNames).ToList()];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Operands.Where(x => x.Value != null).Select<AttributeOperand, KeyValuePair<string, AttributeValue>>((x, i) => new($"{Path.AttributeValueName}_{i + 1}", x.Value!)).ToList();
    }

    public class AttributeExistsExpression(AttributePath path) : IExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;

        public string Build() => $"attribute_exists({Path.AttributeName})";

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => [];
    }

    public class AttributeNotExistsExpression(AttributePath path) : IExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;

        public string Build() => $"attribute_not_exists({Path.AttributeName})";

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => [];
    }

    public class AttributeTypeExpression(AttributePath path, AttributeType type) : IExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;
        public AttributeType Type { get; } = type;

        public string Build() => $"attribute_type({Path.AttributeName}, {Path.AttributeValueName})";

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => [new(Path.AttributeValueName, Type.ToString("G"))];
    }

    public class BeginsWithExpression(AttributePath path, string substring) : IExpression, IQuerySortKeyConditionExpression
    {
        public AttributePath Path { get; set; } = path;
        public string Substring { get; set; } = substring;

        public string Build() => $"begins_with({Path.AttributeName}, {Path.AttributeValueName})";

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => [new(Path.AttributeValueName, Substring)];
    }

    public class ContainsExpression(AttributePath path, AttributeOperand operand) : IExpression, IQueryFilterExpression
    {
        public AttributePath Path { get; set; } = path;
        public AttributeOperand Operand { get; set; } = operand;

        public string Build() => $"contains({Path.AttributeName}, {Operand.AttributeName ?? Path.AttributeValueName})";

        public List<KeyValuePair<string, string>> GetAttributeNames() => [.. Path.AttributeNames, .. Operand.AttributeNames];

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Operand.Value == null ? [] : [new(Path.AttributeValueName, Operand.Value)];
    }

    public class SizeExpression
    {
        // @todo This class is not implemented - see https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.OperatorsAndFunctions.html
    }

    public class AndExpression(params IExpression?[] expressions) : IExpression, IQueryFilterExpression
    {
        public IExpression[] Expressions { get; set; } = expressions.Where(x => x != null).Cast<IExpression>().ToArray();

        public string Build() => new StringBuilder().AppendJoin(" AND ", Expressions.Select(x => x.Build())).ToString();

        public List<KeyValuePair<string, string>> GetAttributeNames() => Expressions.SelectMany(x => x.GetAttributeNames()).ToList();

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Expressions.SelectMany(x => x.GetAttributeValues()).ToList();
    }

    public class OrExpression(params IExpression?[] expressions) : IExpression, IQueryFilterExpression
    {
        public IExpression[] Expressions { get; set; } = expressions.Where(x => x != null).Cast<IExpression>().ToArray();

        public string Build() => new StringBuilder().AppendJoin(" OR ", Expressions.Select(x => x.Build())).ToString();

        public List<KeyValuePair<string, string>> GetAttributeNames() => Expressions.SelectMany(x => x.GetAttributeNames()).ToList();

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Expressions.SelectMany(x => x.GetAttributeValues()).ToList();
    }

    public class NotExpression(IExpression expression) : IExpression, IQueryFilterExpression
    {
        public IExpression Expression { get; set; } = expression;

        public string Build() => $"NOT {Expression.Build()}";

        public List<KeyValuePair<string, string>> GetAttributeNames() => Expression.GetAttributeNames();

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Expression.GetAttributeValues();
    }

    public class ParenthesesExpression(IExpression expression) : IExpression, IQueryFilterExpression
    {
        public IExpression Expression { get; } = expression;

        public string Build() => $"({Expression.Build()})";

        public List<KeyValuePair<string, string>> GetAttributeNames() => Expression.GetAttributeNames();

        public List<KeyValuePair<string, AttributeValue>> GetAttributeValues() => Expression.GetAttributeValues();
    }

    // @todo
    public class AttributeOperand
    {
        public AttributeOperand(AttributePath path) => Path = path;
        public AttributeOperand(AttributeValue value) => Value = value;
        public AttributeOperand(string value) => Value = value;

        public static implicit operator AttributeOperand(AttributePath path) => new(path);
        public static implicit operator AttributeOperand(AttributeValue value) => new(value);
        public static implicit operator AttributeOperand(string value) => new(value);

        public AttributePath? Path { get; set; }
        public AttributeValue? Value { get; set; }

        public string? AttributeName => Path?.AttributeName;
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => Path?.AttributeNames ?? [];
    }

    public class AttributePath(string path)
    {
        public static implicit operator AttributePath(string path) => new(path);

        public string Path { get; set; } = path;

        public string AttributeName => new StringBuilder().AppendJoin(".", Path.Split('.').Select(x => $"#{x}")).ToString();
        public string AttributeValueName => $":v_{Path.Replace(".", "_")}";
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => Path.Split('.').Select(x => new KeyValuePair<string, string>($"#{x}", x)).ToList();
    }
}
#pragma warning restore CS1591 // @todo
