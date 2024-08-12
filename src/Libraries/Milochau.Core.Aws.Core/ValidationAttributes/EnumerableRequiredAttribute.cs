using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Milochau.Core.Aws.Core.ValidationAttributes
{
    /// <summary>
    /// Validation attribute to assert an enumerable property, field or parameter does not include null or whitespace value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    public class EnumerableRequiredAttribute<TValue> : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value == null)
            {
                return true;
            }

            var dictionary = (IEnumerable<TValue>)value;

            return dictionary.All(x => x is not null && (x is not string value || !string.IsNullOrWhiteSpace(value)));
        }
    }
}
