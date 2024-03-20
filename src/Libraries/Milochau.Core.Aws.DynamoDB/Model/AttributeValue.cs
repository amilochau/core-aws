using System;
using System.Collections.Generic;
using System.IO;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the data for an attribute.
    /// <para>
    /// Each attribute value is described as a name-value pair. The name is the data type,
    /// and the value is the data itself.
    /// </para>    /// <para>
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

        /// <summary>Whether a value is set</summary>
        public bool IsSet => B != null || BOOL != null || BS != null || L != null || M != null || N != null || NS != null || NULL != null || S != null || SS != null;

        /// <summary>Implicit conversion</summary>
        public static implicit operator AttributeValue(Guid? value) => new() { S = value?.ToString("N") };
        /// <summary>Implicit conversion</summary>
        public static implicit operator AttributeValue(string? value) => new() { S = value?.Trim() };
        /// <summary>Implicit conversion</summary>
        public static implicit operator AttributeValue(long? value) => new() { N = $"{value}" };
        
        // @todo Add more implicit operators here
    }
}
