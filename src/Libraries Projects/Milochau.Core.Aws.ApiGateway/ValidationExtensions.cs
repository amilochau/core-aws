using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>Extension methods for validation</summary>
    public static class ValidationExtensions
    {
        #region General

        /// <summary>Validate a required value</summary>
        public static void ValidateRequired<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value)
        {
            if (value == null)
            {
                modelStateDictionary.Populate(key, "Required value");
            }
        }

        #endregion
        #region IComparable

        /// <summary>Validate a min value</summary>
        public static void ValidateMinValue<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, TValue minValue)
            where TValue: struct, IComparable<TValue>
        {
            if (value != null && value.Value.CompareTo(minValue) < 0)
            {
                modelStateDictionary.Populate(key, $"Min {minValue} value");
            }
        }

        /// <summary>Validate a max value</summary>
        public static void ValidateMaxValue<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, TValue maxValue)
            where TValue : struct, IComparable<TValue>
        {
            if (value != null && value.Value.CompareTo(maxValue) > 0)
            {
                modelStateDictionary.Populate(key, $"Max {maxValue} value");
            }
        }

        /// <summary>Validate an equal value</summary>
        public static void ValidateEqualValue<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, TValue equalValue)
            where TValue : struct, IComparable<TValue>
        {
            if (value != null && value.Value.CompareTo(equalValue) != 0)
            {
                modelStateDictionary.Populate(key, $"Is {equalValue} value");
            }
        }

        /// <summary>Validate a range value</summary>
        public static void ValidateRangeValue<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, TValue minValue, TValue maxValue)
            where TValue : struct, IComparable<TValue>
        {
            if (value != null)
            {
                if (value.Value.CompareTo(minValue) < 0 || value.Value.CompareTo(maxValue) > 0)
                modelStateDictionary.Populate(key, $"Between {minValue} and {maxValue} value");
            }
        }

        #endregion
        #region IEnumerable

        /// <summary>Validate a min length</summary>
        public static void ValidateMinLength<TItemValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IEnumerable<TItemValue>? value, int minLength)
        {
            if (value != null && value.Count() < minLength)
            {
                modelStateDictionary.Populate(key, $"Min {minLength} length");
            }
        }

        /// <summary>Validate a max length</summary>
        public static void ValidateMaxLength<TItemValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IEnumerable<TItemValue>? value, int maxLength)
        {
            if (value != null && value.Count() > maxLength)
            {
                modelStateDictionary.Populate(key, $"Min {maxLength} length");
            }
        }

        /// <summary>Validate an equal length</summary>
        public static void ValidateEqualLength<TItemValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IEnumerable<TItemValue>? value, int equalLength)
        {
            if (value != null && value.Count() != equalLength)
            {
                modelStateDictionary.Populate(key, $"Is {equalLength} length");
            }
        }

        /// <summary>Validate a range length</summary>
        public static void ValidateRangeLength<TItemValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IEnumerable<TItemValue>? value, int minLength, int maxLength)
        {
            if (value != null)
            {
                var count = value.Count();
                if (count < minLength || count > maxLength)
                {
                    modelStateDictionary.Populate(key, $"Between {minLength} and {maxLength} length");
                }
            }
        }

        #endregion
        #region Misc

        /// <summary>Validate a not whitespace string</summary>
        public static void ValidateNotWhitespace(this Dictionary<string, Collection<string>> modelStateDictionary, string key, string? value)
        {
            if (value != null && string.IsNullOrWhiteSpace(value))
            {
                modelStateDictionary.Populate(key, "Not empty value");
            }
        }

        /// <summary>Validate a GUID</summary>
        public static void ValidateGuid(this Dictionary<string, Collection<string>> modelStateDictionary, string key, string? value, bool useDashes = false)
        {
            if (value != null)
            {
                var format = useDashes ? "D" : "N";
                if (!Guid.TryParseExact(value, format, out _))
                {
                    modelStateDictionary.Populate(key, "GUID required");
                }
            }
        }

        /// <summary>Validate a date from the date part of ISO 8601 (yyyy-MM-dd)</summary>
        public static void ValidateDate(this Dictionary<string, Collection<string>> modelStateDictionary, string key, string? value)
        {
            if (value != null)
            {
                if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    modelStateDictionary.Populate(key, "Date required");
                }
            }
        }

        /// <summary>Validate an email address</summary>
        public static void ValidateEmail(this Dictionary<string, Collection<string>> modelStateDictionary, string key, string? value)
        {
            if (value != null)
            {
                var index = value.IndexOf('@');
                if (index <= 0 || index == value.Length - 1 || index != value.LastIndexOf('@'))
                {
                    // If '@' is not found or the first char, of '@' is the last char, or more than one '@' is found
                    modelStateDictionary.Populate(key, "Email address required");
                }
            }
        }

        #endregion
    }
}
