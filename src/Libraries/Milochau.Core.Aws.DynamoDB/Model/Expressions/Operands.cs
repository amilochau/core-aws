using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Model.Expressions
{
    /// <summary>Operand, wrapping an <see cref="AttributeValue"/></summary>
    public class AttributeValueOperand(AttributeValue value) : IOperand, IUpdateValueOperand
    {
        /// <summary>Implicit convertor</summary>
        public static implicit operator AttributeValueOperand(AttributeValue value) => new(value);

        private readonly string id = Guid.NewGuid().ToString("N");
        private readonly AttributeValue value = value;

        /// <summary>Expression, typically <c>:v_guid</c></summary>
        public string Expression => $":v_{id}";

        /// <summary>Attribute names, typically empty for <see cref="AttributeValueOperand"/></summary>
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => [];

        /// <summary>Attribute values, typically <c>{ :v_guid, value }</c></summary>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [new($":v_{id}", value)];
    }

    /// <summary>Operand, wrapping a <see cref="string"/> as a path</summary>
    public class AttributePathOperand(string path) : IOperand, IUpdateValueOperand
    {
        /// <summary>Implicit convertor</summary>
        public static implicit operator AttributePathOperand(string path) => new(path);

        /// <summary>Add an <see cref="IOperand"/> to an <see cref="AttributePathOperand"/> to create an <see cref="AdditionOperand"/></summary>
        public static AdditionOperand operator +(AttributePathOperand operand1, IOperand operand2) => new(operand1, operand2);

        /// <summary>Reduce an <see cref="IOperand"/> from an <see cref="AttributePathOperand"/> to create an <see cref="AdditionOperand"/></summary>
        public static AdditionOperand operator -(AttributePathOperand operand1, IOperand operand2) => new(operand1, operand2);

        private readonly string path = path;

        /// <summary>Expression, typically <c>#prop1.#prop2[1]</c></summary>
        // @todo Make it work with index as [1]
        public string Expression => new StringBuilder().AppendJoin(".", path.Split('.').Select(x => $"#{x}")).ToString();

        /// <summary>Attribute names, typically <c>{ #prop1, prop1 }</c></summary>
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => path.Split('.').Select(x => new KeyValuePair<string, string>($"#{x}", x)).ToList();

        /// <summary>Attribute values, typically empty for <see cref="AttributePathOperand"/></summary>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [];
    }

    /// <summary>Operand (containing a pair of <see cref="IOperand"/>)</summary>
    public class AdditionOperand(IOperand operand1, IOperand operand2) : IUpdateValueOperand
    {
        private readonly IOperand operand1 = operand1;
        private readonly IOperand operand2 = operand2;

        /// <summary>Expression, typically <c>#prop1.#prop2[1] + #prop1.#prop2[1]</c>, or <c>#prop1.#prop2[1] + :v_guid</c></summary>
        public string Expression => $"{operand1.Expression} + {operand2.Expression}";

        /// <summary>Attribute names, typically <c>{ #prop1, prop1 }</c></summary>
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => [.. operand1.AttributeNames, .. operand2.AttributeNames];

        /// <summary>Attribute values, typically <c>{ :v_guid, value }</c></summary>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. operand1.AttributeValues, .. operand2.AttributeValues];
    }

    /// <summary>Operand (containing a pair of <see cref="IOperand"/>)</summary>
    public class ReductionOperand(IOperand operand1, IOperand operand2) : IUpdateValueOperand
    {
        private readonly IOperand operand1 = operand1;
        private readonly IOperand operand2 = operand2;

        /// <summary>Expression, typically <c>#prop1.#prop2[1] - #prop1.#prop2[1]</c>, or <c>#prop1.#prop2[1] - :v_guid</c></summary>
        public string Expression => $"{operand1.Expression} - {operand2.Expression}";

        /// <summary>Attribute names, typically <c>{ #prop1, prop1 }</c></summary>
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => [.. operand1.AttributeNames, .. operand2.AttributeNames];

        /// <summary>Attribute values, typically <c>{ :v_guid, value }</c></summary>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. operand1.AttributeValues, .. operand2.AttributeValues];
    }

    /// <summary>Operand (containing a pair of <see cref="IOperand"/>)</summary>
    public class ListAppendOperand(IOperand operand1, IOperand operand2) : IUpdateValueOperand
    {
        private readonly IOperand operand1 = operand1;
        private readonly IOperand operand2 = operand2;

        /// <summary>Expression, typically <c>list_append(#prop1.#prop2[1], :v_guid)</c></summary>
        public string Expression => $"list_append({operand1.Expression}, {operand2.Expression})";

        /// <summary>Attribute names, typically <c>{ #prop1, prop1 }</c></summary>
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => [.. operand1.AttributeNames, .. operand2.AttributeNames];

        /// <summary>Attribute values, typically <c>{ :v_guid, value }</c></summary>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. operand1.AttributeValues, .. operand2.AttributeValues];
    }

    /// <summary>Operand (containing a pair of <see cref="IOperand"/>)</summary>
    public class IfNotExistsOperand(IOperand operand1, IOperand operand2) : IUpdateValueOperand
    {
        private readonly IOperand operand1 = operand1;
        private readonly IOperand operand2 = operand2;

        /// <summary>Expression, typically <c>if_not_exists(#prop1.#prop2[1], :v_guid)</c></summary>
        public string Expression => $"if_not_exists({operand1.Expression}, {operand2.Expression})";

        /// <summary>Attribute names, typically <c>{ #prop1, prop1 }</c></summary>
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => [.. operand1.AttributeNames, .. operand2.AttributeNames];

        /// <summary>Attribute values, typically <c>{ :v_guid, value }</c></summary>
        public List<KeyValuePair<string, AttributeValue>> AttributeValues => [.. operand1.AttributeValues, .. operand2.AttributeValues];
    }
}
