﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            where TValue: IComparable<TValue>
        {
            if (value != null && value.CompareTo(minValue) < 0)
            {
                modelStateDictionary.Populate(key, $"Min {minValue} value");
            }
        }

        /// <summary>Validate a max value</summary>
        public static void ValidateMaxValue<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, TValue maxValue)
            where TValue : IComparable<TValue>
        {
            if (value != null && value.CompareTo(maxValue) > 0)
            {
                modelStateDictionary.Populate(key, $"Max {maxValue} value");
            }
        }

        /// <summary>Validate an equal value</summary>
        public static void ValidateEqualValue<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, TValue equalValue)
            where TValue : IComparable<TValue>
        {
            if (value != null && value.CompareTo(equalValue) != 0)
            {
                modelStateDictionary.Populate(key, $"Is {equalValue} value");
            }
        }

        /// <summary>Validate a range value</summary>
        public static void ValidateRangeValue<TValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, TValue minValue, TValue maxValue)
            where TValue : IComparable<TValue>
        {
            if (value != null)
            {
                if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0)
                modelStateDictionary.Populate(key, $"Between {minValue} and {maxValue} value");
            }
        }

        #endregion
        #region IEnumerable

        /// <summary>Validate a min length</summary>
        public static void ValidateMinLength<TValue, TItemValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, int minLength)
            where TValue : IEnumerable<TItemValue>
        {
            if (value != null && value.Count() < minLength)
            {
                modelStateDictionary.Populate(key, $"Min {minLength} length");
            }
        }

        /// <summary>Validate a max length</summary>
        public static void ValidateMaxLength<TValue, TItemValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, int maxLength)
            where TValue : IEnumerable<TItemValue>
        {
            if (value != null && value.Count() > maxLength)
            {
                modelStateDictionary.Populate(key, $"Min {maxLength} length");
            }
        }

        /// <summary>Validate an equal length</summary>
        public static void ValidateEqualLength<TValue, TItemValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, int equalLength)
            where TValue : IEnumerable<TItemValue>
        {
            if (value != null && value.Count() != equalLength)
            {
                modelStateDictionary.Populate(key, $"Is {equalLength} length");
            }
        }

        /// <summary>Validate a range length</summary>
        public static void ValidateRangeLength<TValue, TItemValue>(this Dictionary<string, Collection<string>> modelStateDictionary, string key, TValue? value, int minLength, int maxLength)
            where TValue : IEnumerable<TItemValue>
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
        #region Dictionary



        #endregion
        #region Misc

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
