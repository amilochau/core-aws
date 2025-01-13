﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal
{
    internal class ItemsDictionary : IDictionary<object, object?>
    {
        private readonly IDictionary<object, object?> _items = new Dictionary<object, object?>();

        public IDictionary<object, object?> Items => this;

        // Replace the indexer with one that returns null for missing values
        object? IDictionary<object, object?>.this[object key]
        {
            get
            {
                if (_items.TryGetValue(key, out var value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                _items[key] = value;
            }
        }

        void IDictionary<object, object?>.Add(object key, object? value)
        {
            _items.Add(key, value);
        }

        bool IDictionary<object, object?>.ContainsKey(object key) => _items.ContainsKey(key);

        ICollection<object> IDictionary<object, object?>.Keys => _items.Keys;

        bool IDictionary<object, object?>.Remove(object key) => _items.Remove(key);

        bool IDictionary<object, object?>.TryGetValue(object key, [NotNullWhen(true)] out object? value) => _items.TryGetValue(key, out value);

        ICollection<object?> IDictionary<object, object?>.Values => _items.Values;

        void ICollection<KeyValuePair<object, object?>>.Add(KeyValuePair<object, object?> item)
        {
            _items.Add(item);
        }

        void ICollection<KeyValuePair<object, object?>>.Clear() => _items?.Clear();

        bool ICollection<KeyValuePair<object, object?>>.Contains(KeyValuePair<object, object?> item) => _items.Contains(item);

        void ICollection<KeyValuePair<object, object?>>.CopyTo(KeyValuePair<object, object?>[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<object, object?>>.Count => _items?.Count ?? 0;

        bool ICollection<KeyValuePair<object, object?>>.IsReadOnly => _items?.IsReadOnly ?? false;

        bool ICollection<KeyValuePair<object, object?>>.Remove(KeyValuePair<object, object?> item)
        {
            if (_items.TryGetValue(item.Key, out var value) && Equals(item.Value, value))
            {
                return _items.Remove(item.Key);
            }
            return false;
        }

        IEnumerator<KeyValuePair<object, object?>> IEnumerable<KeyValuePair<object, object?>>.GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
