using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Milochau.Core.Aws.Core.ValidationAttributes
{
    /// <summary>
    /// Validation attribute to assert a string property, field or parameter is a date
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class StringTypeDateAttribute : ValidationAttribute
    {
        public string DateFormat { get; set; } = "yyyy-MM-dd";

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

            return DateTime.TryParseExact(stringValue, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}
