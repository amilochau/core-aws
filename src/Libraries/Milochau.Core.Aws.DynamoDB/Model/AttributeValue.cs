using System.Collections.Generic;
using System.IO;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the data for an attribute.
    /// 
    ///  
    /// <para>
    /// Each attribute value is described as a name-value pair. The name is the data type,
    /// and the value is the data itself.
    /// </para>
    ///  
    /// <para>
    /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.NamingRulesDataTypes.html#HowItWorks.DataTypes">Data
    /// Types</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </para>
    /// </summary>
    public class AttributeValue
    {
        /// <summary>
        /// Gets and sets the property B. 
        /// <para>
        /// An attribute of type Binary. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"B": "dGhpcyB0ZXh0IGlzIGJhc2U2NC1lbmNvZGVk"</c> 
        /// </para>
        /// </summary>
        public MemoryStream? B { get; set; }

        /// <summary>
        /// Gets and sets the property BOOL. 
        /// <para>
        /// An attribute of type Boolean. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"BOOL": true</c> 
        /// </para>
        /// </summary>
        public bool? BOOL { get; set; }

        /// <summary>
        /// Gets and sets the property BS. 
        /// <para>
        /// An attribute of type Binary Set. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"BS": ["U3Vubnk=", "UmFpbnk=", "U25vd3k="]</c> 
        /// </para>
        /// </summary>
        public List<MemoryStream>? BS { get; set; }

        /// <summary>
        /// Gets and sets the property L. 
        /// <para>
        /// An attribute of type List. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"L": [ {"S": "Cookies"} , {"S": "Coffee"}, {"N": "3.14159"}]</c> 
        /// </para>
        /// </summary>
        public List<AttributeValue>? L { get; set; }

        /// <summary>
        /// Gets and sets the property M. 
        /// <para>
        /// An attribute of type Map. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"M": {"Name": {"S": "Joe"}, "Age": {"N": "35"}}</c> 
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? M { get; set; }

        /// <summary>
        /// Gets and sets the property N. 
        /// <para>
        /// An attribute of type Number. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"N": "123.45"</c> 
        /// </para>
        ///  
        /// <para>
        /// Numbers are sent across the network to DynamoDB as strings, to maximize compatibility
        /// across languages and libraries. However, DynamoDB treats them as number type attributes
        /// for mathematical operations.
        /// </para>
        /// </summary>
        public string? N { get; set; }

        /// <summary>
        /// Gets and sets the property NS. 
        /// <para>
        /// An attribute of type Number Set. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"NS": ["42.2", "-19", "7.5", "3.14"]</c> 
        /// </para>
        ///  
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
        ///  
        /// <para>
        ///  <c>"NULL": true</c> 
        /// </para>
        /// </summary>
        public bool? NULL { get; set; }

        /// <summary>
        /// Gets and sets the property S. 
        /// <para>
        /// An attribute of type String. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"S": "Hello"</c> 
        /// </para>
        /// </summary>
        public string? S { get; set; }

        /// <summary>
        /// Gets and sets the property SS. 
        /// <para>
        /// An attribute of type String Set. For example:
        /// </para>
        ///  
        /// <para>
        ///  <c>"SS": ["Giraffe", "Hippo" ,"Zebra"]</c> 
        /// </para>
        /// </summary>
        public List<string>? SS { get; set; }
    }
}
