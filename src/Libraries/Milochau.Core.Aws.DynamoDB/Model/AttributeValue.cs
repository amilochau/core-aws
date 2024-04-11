using Milochau.Core.Aws.Core.Helpers;
using Milochau.Core.Aws.DynamoDB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the data for an attribute.
    /// <para>
    /// Each attribute value is described as a name-value pair. The name is the data type,
    /// and the value is the data itself.
    /// </para>
    /// <para>
    /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.NamingRulesDataTypes.html#HowItWorks.DataTypes">Data
    /// Types</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </para>
    /// </summary>
    public class AttributeValue
    {
        private bool? @bool;
        private List<AttributeValue>? l;
        private Dictionary<string, AttributeValue>? m;
        private List<string>? ns;
        private string? s;
        private List<string>? ss;

        /// <summary>
        /// <para>An attribute of type Boolean. For example: <c>"BOOL": true</c></para>
        /// </summary>
        public bool? BOOL { get => @bool; set => @bool = value.HasValue && value.Value ? value : null; }

        /// <summary>
        /// <para>An attribute of type List. For example: <c>"L": [ {"S": "Cookies"} , {"S": "Coffee"}, {"N": "3.14159"}]</c></para>
        /// </summary>
        public List<AttributeValue>? L { get => l; set => l = value?.Sanitize()?.NullIfEmpty()?.ToList(); }

        /// <summary>
        /// <para>An attribute of type Map. For example: <c>"M": {"Name": {"S": "Joe"}, "Age": {"N": "35"}}</c></para>
        /// </summary>
        public Dictionary<string, AttributeValue>? M { get => m; set => m = value?.Sanitize().NullIfEmpty(); }

        /// <summary>
        /// <para>An attribute of type Number. For example: <c>"N": "123.45"</c></para>
        /// <para>
        /// Numbers are sent across the network to DynamoDB as strings, to maximize compatibility
        /// across languages and libraries. However, DynamoDB treats them as number type attributes
        /// for mathematical operations.
        /// </para>
        /// </summary>
        public string? N { get; set; }

        /// <summary>
        /// <para>An attribute of type Number Set. For example: <c>"NS": ["42.2", "-19", "7.5", "3.14"]</c></para>
        /// <para>
        /// Numbers are sent across the network to DynamoDB as strings, to maximize compatibility
        /// across languages and libraries. However, DynamoDB treats them as number type attributes
        /// for mathematical operations.
        /// </para>
        /// </summary>
        public List<string>? NS { get => ns; set => ns = value?.NullIfEmpty(); }

        /// <summary>
        /// <para>An attribute of type String. For example: <c>"S": "Hello"</c></para>
        /// </summary>
        public string? S { get => s; set => s = !string.IsNullOrWhiteSpace(value) ? value.Trim() : null; }

        /// <summary>
        /// <para>An attribute of type String Set. For example: <c>"SS": ["Giraffe", "Hippo" ,"Zebra"]</c></para>
        /// </summary>
        public List<string>? SS { get => ss; set => ss = value.NullIfEmpty(); }


        /// <summary>Implicit conversion within <see cref="S"/></summary>
        public static implicit operator AttributeValue(string? value) => new(value);

        /// <summary>Implicit conversion within <see cref="S"/></summary>
        public static implicit operator AttributeValue(Guid? value) => new(value);

        /// <summary>Implicit conversion within <see cref="L"/></summary>
        public static implicit operator AttributeValue(List<string>? value) => new(value);

        /// <summary>Implicit conversion within <see cref="BOOL"/></summary>
        public static implicit operator AttributeValue(bool? value) => new(value);

        /// <summary>Implicit conversion within <see cref="N"/></summary>
        public static implicit operator AttributeValue(double? value) => new(value);

        /// <summary>Implicit conversion within <see cref="N"/></summary>
        public static implicit operator AttributeValue(long? value) => new(value);

        /// <summary>Implicit conversion within <see cref="N"/></summary>
        public static implicit operator AttributeValue(decimal? value) => new(value);

        /// <summary>Implicit conversion within <see cref="NS"/></summary>
        public static implicit operator AttributeValue(List<double?>? value) => new(value);

        /// <summary>Implicit conversion within <see cref="N"/></summary>
        public static implicit operator AttributeValue(Enum? value) => new(value);

        // @todo Enum - not null ?

        /// <summary>Implicit conversion within <see cref="N"/></summary>
        public static implicit operator AttributeValue(DateTimeOffset? value) => new(value);

        /// <summary>Implicit conversion within <see cref="M"/></summary>
        public static implicit operator AttributeValue(Dictionary<string, AttributeValue>? value) => new(value);

        /// <summary>Implicit conversion within <see cref="L"/></summary>
        public static implicit operator AttributeValue(List<Dictionary<string, AttributeValue>>? value) => new(value);

        // @todo Add more implicit operators here


        /// <summary>Constructor</summary>
        public AttributeValue() { }

        /// <summary>Constructor</summary>
        public AttributeValue(string? value) => S = value;

        /// <summary>Constructor</summary>
        public AttributeValue(Guid? value) => S = value?.ToString("N");

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<string>? value) => L = value?.Select(x => new AttributeValue(x)).ToList();

        /// <summary>Constructor</summary>
        public AttributeValue(bool? value) => BOOL = value;

        /// <summary>Constructor</summary>
        public AttributeValue(double? value) => N = value != null ? $"{value.Value}" : null;

        /// <summary>Constructor</summary>
        public AttributeValue(long? value) => N = value != null ? $"{value.Value}" : null;

        /// <summary>Constructor</summary>
        public AttributeValue(decimal? value) => N = value != null ? $"{value.Value}" : null;

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<double?>? value) => NS = value?.Where(x => x != null).Select(x => $"{x}").ToList();

        /// <summary>Constructor</summary>
        public AttributeValue(Enum? value) => N = value != null ? $"{Convert.ToInt32(value)}" : null;

        /// <summary>Constructor</summary>
        public AttributeValue(DateTimeOffset? value) => N = value != null ? $"{value.Value.ToUnixTimeSeconds()}" : null;

        /// <summary>Constructor</summary>
        public AttributeValue(IDynamoDbFormattableEntity? value) : this(value?.FormatForDynamoDb()) { }

        /// <summary>Constructor</summary>
        public AttributeValue(Dictionary<string, AttributeValue>? value) => M = value;

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<IDynamoDbFormattableEntity>? value) => L = value?.Select(x => new AttributeValue(x.FormatForDynamoDb()))?.ToList();

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<Dictionary<string, AttributeValue>>? value) => L = value?.Select(x => new AttributeValue(x))?.ToList();

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<AttributeValue>? value) => L = value?.ToList();
    }
}
