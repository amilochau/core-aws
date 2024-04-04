using System;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Extension methods for attribute value dictionary</summary>
    public static partial class AttributesExtensions
    {
        /// <summary>Append a value under condition</summary>
        public static IEnumerable<TSource> AppendIf<TSource>(this IEnumerable<TSource> source, TSource element, bool condition)
        {
            return condition ? source.Append(element) : source;
        }
        /// <summary>Append a value under condition</summary>
        public static IEnumerable<TSource> AppendIf<TSource>(this IEnumerable<TSource> source, TSource element, Func<TSource, bool> condition)
        {
            return condition(element) ? source.Append(element) : source;
        }
        /// <summary>Apply a function when <paramref name="origin"/> is not <c>null</c></summary>
        public static TValue? ApplyOrDefault<TOrigin, TValue>(this TOrigin? origin, Func<TOrigin, TValue> transform, TValue? defaultValue = default)
        {
            return origin != null ? transform(origin) : defaultValue;
        }
    }
}
