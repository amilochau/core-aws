using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// B
        /// <para>
        /// An attribute of type Binary. For example:
        /// </para>
        /// <code>"B": "dGhpcyB0ZXh0IGlzIGJhc2U2NC1lbmNvZGVk"</code>
        /// </summary>
        public MemoryStream? B { get; set; }

        /// <summary>
        /// BOOL
        /// <para>
        /// An attribute of type Boolean. For example:
        /// </para>
        /// <code>"BOOL": true</code>
        /// </summary>
        public bool? BOOL { get; set; }

        /// <summary>
        /// BS
        /// <para>
        /// An attribute of type Binary Set. For example:
        /// </para>
        /// <code>"BS": ["U3Vubnk=", "UmFpbnk=", "U25vd3k="]</code>
        /// </summary>
        public List<MemoryStream>? BS { get; set; }

        /// <summary>
        /// L
        /// <para>
        /// An attribute of type List. For example:
        /// </para>
        /// <code>"L": [ {"S": "Cookies"} , {"S": "Coffee"}, {"N": "3.14159"}]</code> 
        /// </summary>
        public List<AttributeValue>? L { get; set; }

        /// <summary>
        /// M
        /// <para>
        /// An attribute of type Map. For example:
        /// </para>
        /// <code>"M": {"Name": {"S": "Joe"}, "Age": {"N": "35"}}</code> 
        /// </summary>
        public Dictionary<string, AttributeValue>? M { get; set; }

        /// <summary>
        /// N
        /// <para>
        /// An attribute of type Number. For example:
        /// </para>
        /// <code>"N": "123.45"</code> 
        /// <para>
        /// Numbers are sent across the network to DynamoDB as strings, to maximize compatibility
        /// across languages and libraries. However, DynamoDB treats them as number type attributes
        /// for mathematical operations.
        /// </para>
        /// </summary>
        public string? N { get; set; }

        /// <summary>
        /// NS
        /// <para>
        /// An attribute of type Number Set. For example:
        /// </para>
        /// <code>"NS": ["42.2", "-19", "7.5", "3.14"]</code> 
        /// <para>
        /// Numbers are sent across the network to DynamoDB as strings, to maximize compatibility
        /// across languages and libraries. However, DynamoDB treats them as number type attributes
        /// for mathematical operations.
        /// </para>
        /// </summary>
        public List<string>? NS { get; set; }

        /// <summary>
        /// Gets and sets the property NULL. 
        /// <para>
        /// An attribute of type Null. For example:
        /// </para>
        /// <code>"NULL": true</code> 
        /// </summary>
        public bool? NULL { get; set; }

        /// <summary>
        /// S
        /// <para>
        /// An attribute of type String. For example:
        /// </para>
        /// <code>"S": "Hello"</code>
        /// </summary>
        public string? S { get; set; }

        /// <summary>
        /// SS
        /// <para>
        /// An attribute of type String Set. For example:
        /// </para>
        /// <code>"SS": ["Giraffe", "Hippo" ,"Zebra"]</code>
        /// </summary>
        public List<string>? SS { get; set; }


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

        /// <summary>Implicit conversion within <see cref="M"/></summary>
        public static implicit operator AttributeValue(List<DynamoDbAttribute>? value) => new(value);

        /// <summary>Implicit conversion within <see cref="L"/></summary>
        public static implicit operator AttributeValue(List<Dictionary<string, AttributeValue>>? value) => new(value);

        // @todo Add more implicit operators here


        /// <summary>Constructor</summary>
        public AttributeValue() { }

        /// <summary>Constructor</summary>
        public AttributeValue(string? value) => S = value?.Trim();

        /// <summary>Constructor</summary>
        public AttributeValue(Guid? value) => S = value?.ToString("N");

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<string>? value) => L = value?.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new AttributeValue(x)).ToList();

        /// <summary>Constructor</summary>
        public AttributeValue(bool? value) => BOOL = value.HasValue && value.Value ? true : null;

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
        public AttributeValue(Dictionary<string, AttributeValue>? value) => M = value == null || value.Count == 0 ? null : value;

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<DynamoDbAttribute>? value) => M = value == null || !value.Any() ? null : value.ToDictionary(x => x.Key, x => x.Value);

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<IDynamoDbFormattableEntity>? value) => L = value == null || !value.Any() ? null : value.Select(x => new AttributeValue(x.FormatForDynamoDb().ToDictionary(a => a.Key, a => a.Value))).ToList();

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<Dictionary<string, AttributeValue>>? value) => L = value == null || !value.Any() ? null : value.Select(x => new AttributeValue(x)).ToList();

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<IEnumerable<DynamoDbAttribute>>? value) => L = value == null || !value.Any() ? null : value.Select(x => new AttributeValue(x)).ToList();

        /// <summary>Constructor</summary>
        public AttributeValue(IEnumerable<AttributeValue>? value) => L = value == null || !value.Any() ? null : value.ToList();
    }
}
