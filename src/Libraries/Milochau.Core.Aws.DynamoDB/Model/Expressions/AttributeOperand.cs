using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model.Expressions
{
    /// <summary>Attribute operand (containing an <see cref="AttributePath"/> or an <see cref="AttributeValue"/>)</summary>
    public class AttributeOperand
    {
        /// <summary>Constructor</summary>
        public AttributeOperand(AttributePath path) => Path = path;
        /// <summary>Constructor</summary>
        public AttributeOperand(AttributeValue value) => Value = value;
        /// <summary>Constructor</summary>
        public AttributeOperand(string value) => Value = value;

        /// <summary>Implicit convertor</summary>
        public static implicit operator AttributeOperand(AttributePath path) => new(path);
        /// <summary>Implicit convertor</summary>
        public static implicit operator AttributeOperand(AttributeValue value) => new(value);
        /// <summary>Implicit convertor</summary>
        public static implicit operator AttributeOperand(string value) => new(value);

        /// <summary>Path, when the operand contains an <see cref="AttributePath"/></summary>
        public AttributePath? Path { get; set; }

        /// <summary>Value, when the operand contains an <see cref="AttributeValue"/></summary>
        public AttributeValue? Value { get; set; }

        /// <summary>Attribute name, typically <c>#prop1.#prop2[1]</c></summary>
        /// <remarks>Empty when the operand does not contain an <see cref="AttributePath"/></remarks>
        public string? AttributeName => Path?.AttributeName;

        /// <summary>Attribute names, typically <c>{ #prop1, prop1 }</c></summary>
        /// <remarks>Empty when the operand does not contain an <see cref="AttributePath"/></remarks>
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => Path?.AttributeNames ?? [];
    }
}
