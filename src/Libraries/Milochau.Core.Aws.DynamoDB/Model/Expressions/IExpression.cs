using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model.Expressions
{
    /// <summary>Expression</summary>
    public interface IExpression
    {
        /// <summary>Build expression</summary>
        string Expression { get; }

        /// <summary>Attribute names</summary>
        List<KeyValuePair<string, string>> AttributeNames { get; }

        /// <summary>Attribute names</summary>
        List<KeyValuePair<string, AttributeValue>> AttributeValues { get; }
    }

    /// <summary>Key Condition expression for Query partition key</summary>
    public interface IQueryPartitionKeyConditionExpression : IExpression { }
    /// <summary>Key Condition expression for Query sort key</summary>
    public interface IQuerySortKeyConditionExpression : IExpression { }

    /// <summary>Filter expression</summary>
    /// <remarks>See <see href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.OperatorsAndFunctions.html"/></remarks>
    public interface IFilterExpression : IExpression { }

    /// <summary>Update expression</summary>
    public interface IUpdateExpression : IExpression { }

    /// <summary>Value that can be used to set an item in an <c>UpdateItem</c> operation</summary>
    public interface ISetValue
    {
        /// <summary>Build expression</summary>
        string Build();
    }
}
