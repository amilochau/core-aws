using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>Extension methods for validation</summary>
    public static class ValidationExtensions
    {
        /// <summary>Validate a required string value</summary>
        public static void ValidateRequired(this Dictionary<string, Collection<string>> modelStateDictionary, string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                modelStateDictionary.Populate(key, "Required");
            }
        }

        /// <summary>Validate a required object value</summary>
        public static void ValidateRequired(this Dictionary<string, Collection<string>> modelStateDictionary, string key, object? value)
        {
            if (value == null)
            {
                modelStateDictionary.Populate(key, "Required");
            }
        }

        /// <summary>Validate an int value in a range</summary>
        public static void ValidateBetween(this Dictionary<string, Collection<string>> modelStateDictionary, string key, int? value, int min, int max)
        {
            if (value != null && (value < min || value > max))
            {
                modelStateDictionary.Populate(key, $"Between {min} and {max}");
            }
        }

        /// <summary>Validate the minimal length of a string value</summary>
        public static void ValidateMinLength(this Dictionary<string, Collection<string>> modelStateDictionary, string key, string? value, int minLength)
        {
            if (value != null && value.Length < minLength)
            {
                modelStateDictionary.Populate(key, $"Min {minLength} characters");
            }
        }

        /// <summary>Validate the minimal length of an enumerable value</summary>
        public static void ValidateMinLength(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IEnumerable<string>? value, int minLength)
        {
            if (value != null && value.Count() < minLength)
            {
                modelStateDictionary.Populate(key, $"Min {minLength} items");
            }
        }

        /// <summary>Validate the minimal length of a dictionary value</summary>
        public static void ValidateMinLength<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IDictionary<string, TValue>? value, int minLength)
        {
            if (value != null && value.Count < minLength)
            {
                modelStateDictionary.Populate(key, $"Min {minLength} items");
            }
        }

        /// <summary>Validate the maximal length of a string value</summary>
        public static void ValidateMaxLength(this Dictionary<string, Collection<string>> modelStateDictionary, string key, string? value, int maxLength)
        {
            if (value != null && value.Length > maxLength)
            {
                modelStateDictionary.Populate(key, $"Max {maxLength} characters");
            }
        }

        /// <summary>Validate the maximal length of an enumerable value</summary>
        public static void ValidateMaxLength(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IEnumerable<string>? value, int maxLength)
        {
            if (value != null && value.Count() > maxLength)
            {
                modelStateDictionary.Populate(key, $"Max {maxLength} items");
            }
        }

        /// <summary>Validate the maximal length of a dictionary value</summary>
        public static void ValidateMaxLength<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IDictionary<string, TValue>? value, int maxLength)
        {
            if (value != null && value.Count > maxLength)
            {
                modelStateDictionary.Populate(key, $"Max {maxLength} items");
            }
        }

        /// <summary>Validate the maximal length of each item of an enumerable value</summary>
        public static void ValidateMaxLengthItem(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IEnumerable<string>? value, int maxLength)
        {
            if (value != null)
            {
                foreach (var item in value)
                {
                    if (item.Length > maxLength)
                    {
                        modelStateDictionary.Populate(key, $"Max {maxLength} characters");
                        break;
                    }
                }
            }
        }
        /// <summary>Validate the maximal length of each item of a dictionary value</summary>
        public static void ValidateMaxLengthItem(this Dictionary<string, Collection<string>> modelStateDictionary, string key, IDictionary<string, string>? value, int maxLength)
        {
            if (value != null)
            {
                foreach (var item in value)
                {
                    if (item.Value.Length > maxLength)
                    {
                        modelStateDictionary.Populate(key, $"Max {maxLength} characters");
                        break;
                    }
                }
            }
        }

        /// <summary>Validate a string value as an email address</summary>
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
    }
}
