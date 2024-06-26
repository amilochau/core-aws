﻿using Milochau.Core.Aws.DynamoDB.Model;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    internal static class AttributeValueHelpers
    {
        public static bool IsSet(this AttributeValue attributeValue)
        {
            if (attributeValue.BOOL != null && attributeValue.BOOL.Value)
            {
                return true;
            }

            if (attributeValue.L != null && attributeValue.L.Count > 0 && attributeValue.L.Any(x => x.IsSet()))
            {
                return true;
            }

            if (attributeValue.M != null && attributeValue.M.Count > 0 && attributeValue.M.Any(x => x.Value.IsSet()))
            {
                return true;
            }

            if (attributeValue.N != null && !string.IsNullOrWhiteSpace(attributeValue.N))
            {
                return true;
            }

            if (attributeValue.NS != null && attributeValue.NS.Count > 0 && attributeValue.NS.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                return true;
            }

            if (attributeValue.S != null && !string.IsNullOrWhiteSpace(attributeValue.S))
            {
                return true;
            }

            if (attributeValue.SS != null && attributeValue.SS.Count > 0 && attributeValue.SS.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                return true;
            }

            return false;
        }

        public static Dictionary<string, AttributeValue> Sanitize(this Dictionary<string, AttributeValue> dictionary)
        {
            return dictionary.Where(x => x.Value.IsSet()).ToDictionary();
        }

        public static List<AttributeValue> Sanitize(this List<AttributeValue> list)
        {
            return list.Where(x => x.IsSet()).ToList();
        }
    }
}
