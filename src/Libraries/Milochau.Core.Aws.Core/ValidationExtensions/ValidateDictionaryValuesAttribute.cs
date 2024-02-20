using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Milochau.Core.Aws.Core.ValidationExtensions
{
    /// <summary>
    /// Validation attribute to assert a dictionary property, field or parameter has valid values
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidateDictionaryValuesAttribute<TKey, TValue, TValidator> : ValidationAttribute
        where TValidator : IValidateOptions<TValue>, new()
        where TValue : class
    {
        public override bool IsValid(object? value)
        {
            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value is null)
            {
                return true;
            }

            var dictionary = (IDictionary<TKey, TValue>)value;
            var validator = new TValidator();

            foreach (var keyValue in dictionary)
            {
                if (keyValue.Value is null)
                {
                    continue;
                }

                var validationResult = validator.Validate(null, keyValue.Value);

                if (validationResult.Failed)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
