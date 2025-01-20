using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    public class Annotations : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly ConcurrentDictionary<string, string> _annotations = new();

        /// <summary>
        /// Gets the annotation value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the annotation value to get</param>
        /// <returns>The value associated with the specified key</returns>
        public object this[string key]
        {
            get
            {
                return _annotations[key];
            }
        }

        /// <summary>
        /// Add the specified key and string value as annotation
        /// </summary>
        /// <param name="key">The key of the annotation to add</param>
        /// <param name="value">The string value of the annotation to add</param>
        public void Add(string key, string value)
        {
            _annotations[key] = value;
        }

        /// <summary>
        /// Returns an enumerator that iterates through annotations
        /// </summary>
        /// <returns>An enumerator structure for annotations.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _annotations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
