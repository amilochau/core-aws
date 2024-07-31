using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Milochau.Core.Aws.Core.ValidationExtensions
{
    /// <summary>
    /// Validation attribute to assert an enumerable property, field or parameter has all its values as part of a list of allowed values
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EnumerableAllowedValuesAttribute<TValue>(params object[] allowedValues) : ValidationAttribute
    {
        public object[] AllowedValues { get; } = allowedValues;

        public override bool IsValid(object? value)
        {
            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value == null)
            {
                return true;
            }

            return ((IEnumerable<TValue>)value).All(x => x is null || AllowedValues.Contains(x));
        }
    }
}
