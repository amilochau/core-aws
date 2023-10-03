using System.Collections.Generic;
using System.IO;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/AttributeValue.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
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
    public partial class AttributeValue
    {
        /// <summary>
        /// Empty constructor used to set  properties independently even when a simple constructor is available
        /// </summary>
        public AttributeValue() { }

        /// <summary>
        /// Gets and sets the property B. 
        /// <para>
        /// An attribute of type Binary. For example:
        /// </para>
        ///  
        /// <para>
        ///  <code>"B": "dGhpcyB0ZXh0IGlzIGJhc2U2NC1lbmNvZGVk"</code> 
        /// </para>
        /// </summary>
        public MemoryStream? B { get; set; }

        // Check to see if B property is set
        internal bool IsSetB()
        {
            return B != null;
        }

        /// <summary>
        /// Gets and sets the property BOOL. 
        /// <para>
        /// An attribute of type Boolean. For example:
        /// </para>
        ///  
        /// <para>
        ///  <code>"BOOL": true</code> 
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
        ///  <code>"BS": ["U3Vubnk=", "UmFpbnk=", "U25vd3k="]</code> 
        /// </para>
        /// </summary>
        public List<MemoryStream>? BS { get; set; }

        // Check to see if BS property is set
        internal bool IsSetBS()
        {
            return BS != null && BS.Count > 0;
        }

        /// <summary>
        /// Gets and sets the property L. 
        /// <para>
        /// An attribute of type List. For example:
        /// </para>
        ///  
        /// <para>
        ///  <code>"L": [ {"S": "Cookies"} , {"S": "Coffee"}, {"N": "3.14159"}]</code> 
        /// </para>
        /// </summary>
        public List<AttributeValue>? L { get; set; }

        // Check to see if L property is set
        internal bool IsSetL()
        {
            return L != null && L.Count > 0;
        }

        /// <summary>
        /// Gets and sets the property M. 
        /// <para>
        /// An attribute of type Map. For example:
        /// </para>
        ///  
        /// <para>
        ///  <code>"M": {"Name": {"S": "Joe"}, "Age": {"N": "35"}}</code> 
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? M { get; set; }

        // Check to see if M property is set
        internal bool IsSetM()
        {
            return M != null && M.Count > 0;
        }

        /// <summary>
        /// Gets and sets the property N. 
        /// <para>
        /// An attribute of type Number. For example:
        /// </para>
        ///  
        /// <para>
        ///  <code>"N": "123.45"</code> 
        /// </para>
        ///  
        /// <para>
        /// Numbers are sent across the network to DynamoDB as strings, to maximize compatibility
        /// across languages and libraries. However, DynamoDB treats them as number type attributes
        /// for mathematical operations.
        /// </para>
        /// </summary>
        public string? N { get; set; }

        // Check to see if N property is set
        internal bool IsSetN()
        {
            return N != null;
        }

        /// <summary>
        /// Gets and sets the property NS. 
        /// <para>
        /// An attribute of type Number Set. For example:
        /// </para>
        ///  
        /// <para>
        ///  <code>"NS": ["42.2", "-19", "7.5", "3.14"]</code> 
        /// </para>
        ///  
        /// <para>
        /// Numbers are sent across the network to DynamoDB as strings, to maximize compatibility
        /// across languages and libraries. However, DynamoDB treats them as number type attributes
        /// for mathematical operations.
        /// </para>
        /// </summary>
        public List<string>? NS { get; set; }

        // Check to see if NS property is set
        internal bool IsSetNS()
        {
            return NS != null && NS.Count > 0;
        }

        /// <summary>
        /// Gets and sets the property NULL. 
        /// <para>
        /// An attribute of type Null. For example:
        /// </para>
        ///  
        /// <para>
        ///  <code>"NULL": true</code> 
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
        ///  <code>"S": "Hello"</code> 
        /// </para>
        /// </summary>
        public string? S { get; set; }

        // Check to see if S property is set
        internal bool IsSetS()
        {
            return S != null;
        }

        /// <summary>
        /// Gets and sets the property SS. 
        /// <para>
        /// An attribute of type String Set. For example:
        /// </para>
        ///  
        /// <para>
        ///  <code>"SS": ["Giraffe", "Hippo" ,"Zebra"]</code> 
        /// </para>
        /// </summary>
        public List<string>? SS { get; set; }

        // Check to see if SS property is set
        internal bool IsSetSS()
        {
            return SS != null && SS.Count > 0;
        }
    }
}
