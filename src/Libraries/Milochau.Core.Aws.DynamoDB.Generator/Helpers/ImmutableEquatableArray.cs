using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Generator.Helpers
{
    /// <summary>
    /// An immutable, equatable array. This is equivalent to <see cref="Array{T}"/> but with value equality support.
    /// </summary>
    /// <typeparam name="T">The type of values in the array.</typeparam>
    public sealed class ImmutableEquatableArray<T> : IEquatable<ImmutableEquatableArray<T>>, IReadOnlyList<T>
        where T : IEquatable<T>
    {
        public static ImmutableEquatableArray<T> Empty { get; } = new(Array.Empty<T>());

        private readonly T[] _values;
        public T this[int index] => _values[index];
        public int Count => _values.Length;

        /// <summary>Constructor</summary>
        public ImmutableEquatableArray(IEnumerable<T> values) => _values = values.ToArray();

        public bool Equals(ImmutableEquatableArray<T> other)
        {
            return ((ReadOnlySpan<T>)_values).SequenceEqual(other._values);
        }

        public override bool Equals(object? obj)
        {
            return obj is ImmutableEquatableArray<T> other && Equals(this, other);
        }

        public override int GetHashCode()
        {
            HashCode hashCode = default;
            foreach (T value in _values)
            {
                hashCode.Add(value);
            }
            return hashCode.ToHashCode();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

        /// <summary>
        /// Checks whether two <see cref="EquatableArray{T}"/> values are the same.
        /// </summary>
        /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
        /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
        /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
        public static bool operator ==(ImmutableEquatableArray<T> left, ImmutableEquatableArray<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="EquatableArray{T}"/> values are not the same.
        /// </summary>
        /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
        /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
        /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
        public static bool operator !=(ImmutableEquatableArray<T> left, ImmutableEquatableArray<T> right)
        {
            return !left.Equals(right);
        }
    }
}
