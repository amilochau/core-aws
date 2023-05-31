using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ApiGateway.Tests
{
    [TestClass]
    public class ValidationExtensionsTests
    {
        [TestMethod]
        [DataRow(null, 1)]
        [DataRow("", 4)]
        [DataRow("        ", 1)]
        [DataRow("aaa", 3)]
        [DataRow("aaaaabbbbbccccc", 3)]
        [DataRow("aaaaabb", 1)]
        [DataRow("aaaaabbb", 0)]
        public void ValidateString(string value, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateNotWhitespace("key", value);
            modelStateDictionary.ValidateMinLength("key", value, 5);
            modelStateDictionary.ValidateMaxLength("key", value, 10);
            modelStateDictionary.ValidateRangeLength("key", value, 7, 9);
            modelStateDictionary.ValidateEqualLength("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(null, 1)]
        [DataRow("", 4)]
        [DataRow("        ", 1)]
        [DataRow("aaa", 3)]
        [DataRow("aaaaabbbbbccccc", 3)]
        [DataRow("aaaaabb", 1)]
        [DataRow("aaaaabbb", 0)]
        public void ValidateStringNullable(string? value, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateNotWhitespace("key", value);
            modelStateDictionary.ValidateMinLength("key", value, 5);
            modelStateDictionary.ValidateMaxLength("key", value, 10);
            modelStateDictionary.ValidateRangeLength("key", value, 7, 9);
            modelStateDictionary.ValidateEqualLength("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(0, 3)]
        [DataRow(3, 3)]
        [DataRow(15, 3)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void ValidateInt(int value, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinValue("key", value, 5);
            modelStateDictionary.ValidateMaxValue("key", value, 10);
            modelStateDictionary.ValidateRangeValue("key", value, 7, 9);
            modelStateDictionary.ValidateEqualValue("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(null, 1)]
        [DataRow(0, 3)]
        [DataRow(3, 3)]
        [DataRow(15, 3)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void ValidateIntNullable(int? value, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinValue("key", value, 5);
            modelStateDictionary.ValidateMaxValue("key", value, 10);
            modelStateDictionary.ValidateRangeValue("key", value, 7, 9);
            modelStateDictionary.ValidateEqualValue("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(0d, 3)]
        [DataRow(3d, 3)]
        [DataRow(15d, 3)]
        [DataRow(7d, 1)]
        [DataRow(8d, 0)]
        public void ValidateDouble(double value, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinValue("key", value, 5d);
            modelStateDictionary.ValidateMaxValue("key", value, 10d);
            modelStateDictionary.ValidateRangeValue("key", value, 7d, 9d);
            modelStateDictionary.ValidateEqualValue("key", value, 8d);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(null, 1)]
        [DataRow(0d, 3)]
        [DataRow(3d, 3)]
        [DataRow(15d, 3)]
        [DataRow(7d, 1)]
        [DataRow(8d, 0)]
        public void ValidateDoubleNullable(double? value, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinValue("key", value, 5d);
            modelStateDictionary.ValidateMaxValue("key", value, 10d);
            modelStateDictionary.ValidateRangeValue("key", value, 7d, 9d);
            modelStateDictionary.ValidateEqualValue("key", value, 8d);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(0, 3)]
        [DataRow(3, 3)]
        [DataRow(15, 3)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void ValidateList(int itemsCount, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            List<object>? value = new List<object>(itemsCount);
            for (int i = 0; i < itemsCount; i++)
            {
                value.Add(new object());
            }

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinLength("key", value, 5);
            modelStateDictionary.ValidateMaxLength("key", value, 10);
            modelStateDictionary.ValidateRangeLength("key", value, 7, 9);
            modelStateDictionary.ValidateEqualLength("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(-1, 1)]
        [DataRow(0, 3)]
        [DataRow(3, 3)]
        [DataRow(15, 3)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void ValidateListNullable(int itemsCount, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            List<object>? value = null;
            if (itemsCount >= 0)
            {
                value = new List<object>(itemsCount);
                for (int i = 0; i < itemsCount; i++)
                {
                    value.Add(new object());
                }
            }

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinLength("key", value, 5);
            modelStateDictionary.ValidateMaxLength("key", value, 10);
            modelStateDictionary.ValidateRangeLength("key", value, 7, 9);
            modelStateDictionary.ValidateEqualLength("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(0, 3)]
        [DataRow(3, 3)]
        [DataRow(15, 3)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void ValidateDictionaryObject(int itemsCount, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            Dictionary<object, object>? value = new Dictionary<object, object>(itemsCount);
            for (int i = 0; i < itemsCount; i++)
            {
                value.Add(new object(), new object());
            }

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinLength("key", value, 5);
            modelStateDictionary.ValidateMaxLength("key", value, 10);
            modelStateDictionary.ValidateRangeLength("key", value, 7, 9);
            modelStateDictionary.ValidateEqualLength("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(-1, 1)]
        [DataRow(0, 3)]
        [DataRow(3, 3)]
        [DataRow(15, 3)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void ValidateDictionaryObjectNullable(int itemsCount, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            Dictionary<object, object>? value = null;
            if (itemsCount >= 0)
            {
                value = new Dictionary<object, object>(itemsCount);
                for (int i = 0; i < itemsCount; i++)
                {
                    value.Add(new object(), new object());
                }
            }

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinLength("key", value, 5);
            modelStateDictionary.ValidateMaxLength("key", value, 10);
            modelStateDictionary.ValidateRangeLength("key", value, 7, 9);
            modelStateDictionary.ValidateEqualLength("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(0, 3)]
        [DataRow(3, 3)]
        [DataRow(15, 3)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void ValidateDictionaryString(int itemsCount, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            Dictionary<string, object>? value = new Dictionary<string, object>(itemsCount);
            for (int i = 0; i < itemsCount; i++)
            {
                value.Add($"{i}", new object());
            }

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinLength("key", value, 5);
            modelStateDictionary.ValidateMaxLength("key", value, 10);
            modelStateDictionary.ValidateRangeLength("key", value, 7, 9);
            modelStateDictionary.ValidateEqualLength("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(-1, 1)]
        [DataRow(0, 3)]
        [DataRow(3, 3)]
        [DataRow(15, 3)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void ValidateDictionaryStringNullable(int itemsCount, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            Dictionary<string, object>? value = null;
            if (itemsCount >= 0)
            {
                value = new Dictionary<string, object>(itemsCount);
                for (int i = 0; i < itemsCount; i++)
                {
                    value.Add($"{i}", new object());
                }
            }

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateMinLength("key", value, 5);
            modelStateDictionary.ValidateMaxLength("key", value, 10);
            modelStateDictionary.ValidateRangeLength("key", value, 7, 9);
            modelStateDictionary.ValidateEqualLength("key", value, 8);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(null, 1)]
        [DataRow("2022", 1)]
        [DataRow("2022-01", 1)]
        [DataRow("2022-01-01", 0)]
        public void ValidateDate(string value, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateDate("key", value);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }

        [TestMethod]
        [DataRow(null, 1)]
        [DataRow("https://google.com", 0)]
        [DataRow("google.com", 1)]
        [DataRow("/maps", 1)]
        public void ValidateUri(string value, int messagesCount)
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            modelStateDictionary.ValidateRequired("key", value);
            modelStateDictionary.ValidateUri("key", value);

            Assert.AreEqual(messagesCount > 0 ? 1 : 0, modelStateDictionary.Count);
            if (messagesCount > 0)
            {
                Assert.AreEqual(messagesCount, modelStateDictionary.First().Value.Count);
            }
        }
    }
}
