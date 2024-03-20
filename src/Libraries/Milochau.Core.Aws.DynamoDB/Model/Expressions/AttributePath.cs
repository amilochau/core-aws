using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Model.Expressions
{
    /// <summary>Attribute path</summary>
    public class AttributePath(string path)
    {
        /// <summary>Implicit convertor</summary>
        public static implicit operator AttributePath(string path) => new(path);

        /// <summary>Path</summary>
        public string Path { get; set; } = path;

        /// <summary>Attribute name, typically <c>#prop1.#prop2[1]</c></summary>
        public string AttributeName => new StringBuilder().AppendJoin(".", Path.Split('.').Select(x => $"#{x}")).ToString();

        /// <summary>Attribute value name, typically <c>:v_prop1_prop2</c></summary>
        public string AttributeValueName => $":v_{Path.Replace(".", "_")}";

        /// <summary>Attribute names, typically <c>{ #prop1, prop1 }</c></summary>
        public IEnumerable<KeyValuePair<string, string>> AttributeNames => Path.Split('.').Select(x => new KeyValuePair<string, string>($"#{x}", x)).ToList();
    }
}
