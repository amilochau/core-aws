using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Milochau.Core.Aws.Core.ValidationAttributes
{
    /// <summary>
    /// Validation attribute to assert an enumerable property, field or parameter does not exceed a maximum length
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EnumerableLengthAttribute<TValue>() : ValidationAttribute
    {
        /// <summary>Gets the maximum acceptable length of the enumerable</summary>
        public int MaximumLength { get; set; } = int.MaxValue;

        /// <summary>Gets or sets the minimum acceptable length of the enumerable</summary>
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

            int length = ((IEnumerable<TValue>)value).Count();
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
