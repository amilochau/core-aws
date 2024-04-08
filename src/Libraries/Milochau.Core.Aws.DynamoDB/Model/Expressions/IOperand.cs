using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model.Expressions
{
    /// <summary>Attribute operand</summary>
    public interface IOperand
    {
        /// <summary>Expression, typically <c>#prop1.#prop2[1]</c> or <c>:v_guid</c></summary>
        string Expression { get; }

        /// <summary>Attribute name, typically <c>#prop1.#prop2[1]</c></summary>
        IEnumerable<KeyValuePair<string, string>> AttributeNames { get; }

        /// <summary>Attribute values, typically <c>{ :v_guid, value }</c></summary>
        List<KeyValuePair<string, AttributeValue>> AttributeValues { get; }
    }

    /// <summary>Attribute operand used for update operations</summary>
    public interface IUpdateValueOperand : IOperand { }
}
