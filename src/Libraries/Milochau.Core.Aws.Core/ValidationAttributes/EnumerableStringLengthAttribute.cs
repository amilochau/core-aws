using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Milochau.Core.Aws.Core.ValidationAttributes
{
    /// <summary>
    /// Validation attribute to assert enumerable values property, field or parameter does not exceed a maximum length
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EnumerableStringLengthAttribute(int maximumLength) : ValidationAttribute
    {
        /// <summary>Gets the maximum acceptable length of the enumerable values</summary>
        public int MaximumLength { get; } = maximumLength;

        /// <summary>Gets or sets the minimum acceptable length of the enumerable values</summary>
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

            var enumerable = (IEnumerable<string>)value;
            foreach (var item in enumerable)
            {
                if (item is null)
                {
                    continue;
                }

                if (item.Length < MinimumLength || item.Length > MaximumLength)
                {
                    return false;
                }
            }

            return true;
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
