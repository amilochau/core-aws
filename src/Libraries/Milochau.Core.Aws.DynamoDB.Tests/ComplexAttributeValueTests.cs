using Microsoft.VisualStudio.TestTools.UnitTesting;
using Milochau.Core.Aws.DynamoDB.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Milochau.Core.Aws.DynamoDB.Helpers;

namespace Milochau.Core.Aws.DynamoDB.Tests
{
    [TestClass]
    public class ComplexAttributeValueTests
    {
        [TestMethod]
        public void ValidateComplexAttributeValue_Null()
        {
            var map = new Map();

            var attributes = map.FormatForDynamoDb().Where(x => x.Value.IsSet());

            Assert.AreEqual(1, attributes.Count()); // Partition key is always present
        }

        [TestMethod]
        public void ValidateComplexAttributeValue_Empty()
        {
            var map = new Map
            {
                Settings = new MapSettings
                {
                    Tags = [],
                },
            };

            var attributes = map.FormatForDynamoDb().Where(x => x.Value.IsSet());

            Assert.AreEqual(1, attributes.Count()); // Partition key is always present
        }

        [TestMethod]
        public void ValidateComplexAttributeValue_EmptyValue()
        {
            var map = new Map
            {
                Settings = new MapSettings
                {
                    Tags =
                    [
                        new MapTag()
                    ],
                },
            };

            var attributes = map.FormatForDynamoDb().Where(x => x.Value.IsSet());

            Assert.AreEqual(1, attributes.Count()); // Partition key is always present
        }

        [TestMethod]
        public void ValidateComplexAttributeValue_AvoidableValue()
        {
            var map = new Map
            {
                Settings = new MapSettings
                {
                    Tags =
                    [
                        new MapTag
                        {
                            Value = "  ",
                        },
                    ],
                },
            };

            var attributes = map.FormatForDynamoDb().Where(x => x.Value.IsSet());

            Assert.AreEqual(1, attributes.Count()); // Partition key is always present
        }

        [TestMethod]
        public void ValidateComplexAttributeValue_Value()
        {
            var map = new Map
            {
                Settings = new MapSettings
                {
                    Tags =
                    [
                        new MapTag
                        {
                            Value = "tag",
                        },
                    ],
                },
            };

            var attributes = map.FormatForDynamoDb().Where(x => x.Value.IsSet());

            Assert.AreEqual(2, attributes.Count()); // Partition key is always present
        }
    }

    [DynamoDbTable("maps")]
    public partial class Map
    {
        [DynamoDbPartitionKeyAttribute("id")]
        public Guid Id { get; set; }

        [DynamoDbAttribute("settings")]
        public MapSettings? Settings { get; set; }
    }

    [DynamoDbNested]
    public partial class MapSettings
    {
        [DynamoDbAttribute("tags")]
        public List<MapTag>? Tags { get; set; }
    }

    [DynamoDbNested]
    public partial class MapTag
    {
        [DynamoDbAttribute("value")]
        public string? Value { get; set; }
    }
}
