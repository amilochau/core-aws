using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.Core.Helpers
{
    public static class EnumerableExtensions
    {
        public static List<TValue>? NullIfEmpty<TValue>(this List<TValue>? list)
        {
            return list == null || list.Count == 0 ? null : list;
        }
        public static IEnumerable<TValue>? NullIfEmpty<TValue>(this IEnumerable<TValue>? list)
        {
            return list == null || !list.Any() ? null : list;
        }
    }
}
