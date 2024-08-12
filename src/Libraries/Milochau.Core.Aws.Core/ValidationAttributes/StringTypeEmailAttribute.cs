using System;
using System.ComponentModel.DataAnnotations;

namespace Milochau.Core.Aws.Core.ValidationAttributes
{
    /// <summary>
    /// Validation attribute to assert a string property, field or parameter is an email
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class StringTypeEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value == null)
            {
                return true;
            }

            if (value is not string stringValue)
            {
                return false;
            }

            var index = stringValue.IndexOf('@');

            return index > 0 && index < stringValue.Length - 1 && index == stringValue.LastIndexOf('@');
        }
    }
}
