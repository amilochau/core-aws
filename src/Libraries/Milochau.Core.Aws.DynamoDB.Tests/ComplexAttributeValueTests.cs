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
                    MoreTags = [],
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
                    MoreTags = new Dictionary<string, MapTag>
                    {
                        ["aa"] = new MapTag(),
                    },
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
                    MoreTags = new Dictionary<string, MapTag>
                    {
                        ["aa"] = new MapTag
                        {
                            Value = "  ",
                        },
                    },
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
                    MoreTags = new Dictionary<string, MapTag>
                    {
                        ["aa"] = new MapTag
                        {
                            Value = "tag",
                        },
                    },
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

        [DynamoDbAttribute("empty")]
        public Dictionary<string, MapTag>? MoreTags { get; set; }
    }

    [DynamoDbNested]
    public partial class MapSettings
    {
        [DynamoDbAttribute("tags")]
        public List<MapTag>? Tags { get; set; }

        [DynamoDbAttribute("moretags")]
        public Dictionary<string, MapTag>? MoreTags { get; set; }
    }

    [DynamoDbNested]
    public partial class MapTag
    {
        [DynamoDbAttribute("value")]
        public string? Value { get; set; }
    }
}
