using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
