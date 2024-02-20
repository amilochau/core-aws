using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Milochau.Core.Aws.Core.ValidationExtensions
{
    /// <summary>
    /// Validation attribute to assert a dictionary property, field or parameter does not include null or whitespace key or value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    public class DictionaryRequiredAttribute<TKey, TValue> : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value == null)
            {
                return true;
            }

            var dictionary = (IDictionary<TKey, TValue>)value;

            return dictionary.All(x => x.Key is not null && (x.Key is not string key || !string.IsNullOrWhiteSpace(key)) && x.Value is not null && (x.Value is not string value || !string.IsNullOrWhiteSpace(value)));
        }
    }
}
