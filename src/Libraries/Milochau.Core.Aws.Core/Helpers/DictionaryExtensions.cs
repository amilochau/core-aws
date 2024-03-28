using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.Helpers
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue>? NullIfEmpty<TKey, TValue>(this Dictionary<TKey, TValue>? dictionary)
            where TKey: notnull
        {
            return dictionary == null || dictionary.Count == 0 ? null : dictionary;
        }
    }
}
