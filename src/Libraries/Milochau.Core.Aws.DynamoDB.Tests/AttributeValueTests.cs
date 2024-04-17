using Microsoft.VisualStudio.TestTools.UnitTesting;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Tests
{
    [TestClass]
    public class AttributeValueTests
    {
        [TestMethod]
        [DataRow(null, false, null)]
        [DataRow("", false, null)]
        [DataRow("  ", false, null)]
        [DataRow(" a ", true, "a")]
        public void ValidateAttributeValue_String(string? value, bool expectedIsSet, string? expectedValue)
        {
            var attributeValue = new AttributeValue(value);

            Assert.AreEqual(expectedIsSet, attributeValue.IsSet());
            Assert.AreEqual(expectedValue, attributeValue.S);
        }

        [TestMethod]
        [DataRow(null, false, null)]
        [DataRow(false, false, null)]
        [DataRow(true, true, true)]
        public void ValidateAttributeValue_Bool(bool? value, bool expectedIsSet, bool? expectedValue)
        {
            var attributeValue = new AttributeValue(value);

            Assert.AreEqual(expectedIsSet, attributeValue.IsSet());
            Assert.AreEqual(expectedValue, attributeValue.BOOL);
        }

        [TestMethod]
        [DataRow(null, false, null)]
        [DataRow(0d, true, "0")]
        [DataRow(10.2d, true, "10.2")]
        public void ValidateAttributeValue_Double(double? value, bool expectedIsSet, string? expectedValue)
        {
            var attributeValue = new AttributeValue(value);

            Assert.AreEqual(expectedIsSet, attributeValue.IsSet());
            Assert.AreEqual(expectedValue, attributeValue.N);
        }

        [TestMethod]
        [DataRow(null, false, null)]
        [DataRow("F61E9B53-D197-4191-8755-2EAC72237C32", true, "f61e9b53d197419187552eac72237c32")]
        public void ValidateAttributeValue_Guid(string? rawValue, bool expectedIsSet, string? expectedValue)
        {
            Guid? value = rawValue != null ? Guid.Parse(rawValue) : null;

            var attributeValue = new AttributeValue(value);

            Assert.AreEqual(expectedIsSet, attributeValue.IsSet());
            Assert.AreEqual(expectedValue, attributeValue.S);
        }

        [TestMethod]
        public void ValidateAttributeValue_Formattable_EmptyValue()
        {
            IDynamoDbFormattableEntity? entity = new FormattableClass_EmptyValue();

            var attributeValue = new AttributeValue(entity);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }

        [TestMethod]
        public void ValidateAttributeValue_ListString_Null()
        {
            List<string>? list = null;

            var attributeValue = new AttributeValue(list, useSet: false);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
            Assert.AreEqual(null, attributeValue.SS);
        }

        [TestMethod]
        public void ValidateAttributeValue_ListString_Empty()
        {
            List<string>? list = [];

            var attributeValue = new AttributeValue(list, useSet: false);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
            Assert.AreEqual(null, attributeValue.SS);
        }

        [TestMethod]
        public void ValidateAttributeValue_ListString_EmptyValue()
        {
            List<string>? list = [""];

            var attributeValue = new AttributeValue(list, useSet: false);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
            Assert.AreEqual(null, attributeValue.SS);
        }

        [TestMethod]
        public void ValidateAttributeValue_ListAttributeValue_Null()
        {
            List<AttributeValue>? list = null;

            var attributeValue = new AttributeValue(list);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }

        [TestMethod]
        public void ValidateAttributeValue_ListAttributeValue_Empty()
        {
            List<AttributeValue>? list = [];

            var attributeValue = new AttributeValue(list);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }

        [TestMethod]
        public void ValidateAttributeValue_ListAttributeValue_EmptyValue()
        {
            List<AttributeValue>? list = [new()];

            var attributeValue = new AttributeValue(list);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }

        [TestMethod]
        public void ValidateAttributeValue_ListFormattable_EmptyValue()
        {
            List<IDynamoDbFormattableEntity>? list = [new FormattableClass_EmptyValue()];

            var attributeValue = new AttributeValue(list);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }

        [TestMethod]
        public void ValidateAttributeValue_ListDictionary_EmptyValue()
        {
            List<Dictionary<string, AttributeValue>>? list =
            [
                new Dictionary<string, AttributeValue>
                {
                    ["1"] = new(),
                }
            ];

            var attributeValue = new AttributeValue(list);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }

        [TestMethod]
        public void ValidateAttributeValue_Dictionary_Null()
        {
            Dictionary<string, AttributeValue>? dictionary = null;

            var attributeValue = new AttributeValue(dictionary);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }

        [TestMethod]
        public void ValidateAttributeValue_Dictionary_Empty()
        {
            Dictionary<string, AttributeValue>? dictionary = [];

            var attributeValue = new AttributeValue(dictionary);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }

        [TestMethod]
        public void ValidateAttributeValue_Dictionary_EmptyValue()
        {
            Dictionary<string, AttributeValue>? dictionary = new Dictionary<string, AttributeValue>
            {
                ["1"] = new(),
            };

            var attributeValue = new AttributeValue(dictionary);

            Assert.AreEqual(false, attributeValue.IsSet());
            Assert.AreEqual(null, attributeValue.L);
        }
    }

    public class FormattableClass_EmptyValue : IDynamoDbFormattableEntity
    {
        public Dictionary<string, AttributeValue> FormatForDynamoDb()
        {
            return new Dictionary<string, AttributeValue>
            {
                ["1"] = new(),
            };
        }
    }
}
