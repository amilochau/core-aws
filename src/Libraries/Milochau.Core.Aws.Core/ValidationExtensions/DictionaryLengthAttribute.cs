using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Milochau.Core.Aws.Core.ValidationExtensions
{
    /// <summary>
    /// Validation attribute to assert a dictionary property, field or parameter does not exceed a maximum length
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DictionaryLengthAttribute<TKey, TValue> : ValidationAttribute
    {
        public DictionaryLengthAttribute(int maximumLength)
        {
            MaximumLength = maximumLength;
        }

        /// <summary>Gets the maximum acceptable length of the dictionary</summary>
        public int MaximumLength { get; }

        /// <summary>Gets or sets the minimum acceptable length of the dictionary</summary>
        public int MinimumLength { get; set; }

        public override bool IsValid(object? value)
        {
            // Check the lengths for legality
            EnsureLegalLengths();

            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value == null)
            {
                return true;
            }

            int length = ((IDictionary<TKey, TValue>)value).Count;
            return length >= MinimumLength && length <= MaximumLength;
        }

        /// <summary>
        ///     Checks that MinimumLength and MaximumLength have legal values.  Throws InvalidOperationException if not.
        /// </summary>
        private void EnsureLegalLengths()
        {
            if (MaximumLength < 0)
            {
                throw new InvalidOperationException();
            }

            if (MaximumLength < MinimumLength)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
