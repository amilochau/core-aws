using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Milochau.Core.Aws.DynamoDB;
using Milochau.Core.Aws.DynamoDB.Model;
using System;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System.ComponentModel.DataAnnotations;
using Milochau.Core.Aws.DynamoDB.Abstractions;
using System.Diagnostics;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess.Nested;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface IDynamoDbDataAccess
    {
        Task ScanAsync(CancellationToken cancellationToken);
        Task UpdateAsync(CancellationToken cancellationToken);
        //Task GetTestItemAsync(CancellationToken cancellationToken);
    }

    public class DynamoDbDataAccess : IDynamoDbDataAccess
    {
        protected readonly IAmazonDynamoDB amazonDynamoDB;

        public DynamoDbDataAccess(IAmazonDynamoDB amazonDynamoDB)
        {
            this.amazonDynamoDB = amazonDynamoDB;
        }

        public async Task ScanAsync(CancellationToken cancellationToken)
        {
            var response = await amazonDynamoDB.ScanAsync(new ScanRequest<Map>
            {
                UserId = null
            }, cancellationToken);
        }


        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            List<Guid> listIds =
            [
                Guid.Parse("a6e60c59204947fbaba7d7305222cfe1"),
                Guid.Parse("a5e7d105032f40d08d2f2f0f000beb10"),
            ];

            var tableName = Item.TableName;

            foreach (var listId in listIds)
            {
                var response = await amazonDynamoDB.QueryAsync(new QueryRequest<Item>
                {
                    UserId = null,
                    PartitionKeyCondition = new EqualValueExpression(Item.PartitionKey, listId),
                }, cancellationToken);

                if (response.Items == null)
                {
                    Debugger.Break();
                    continue;
                }

                foreach (var item in response.Items)
                {
                    var rawSourceUserId = item.GetValueOrDefault(Item.Keys.SourceUserId)?.S;
                    var itemId = item.GetValueOrDefault(Item.Keys.Id)?.S?.ApplyOrDefault(Guid.Parse);
                    if (rawSourceUserId == null || itemId == null)
                    {
                        Debugger.Break();
                        continue;
                    }
                    var sourceUserId = Guid.Parse(rawSourceUserId);
                    var formattedSourceUserId = sourceUserId.ToString("N");
                    if (rawSourceUserId != formattedSourceUserId)
                    {
                        var updateResponse = await amazonDynamoDB.UpdateItemAsync(new UpdateItemRequest<Item>
                        {
                            UserId = null,
                            PartitionKey = listId,
                            SortKey = itemId,
                            UpdateExpression = new UpdateExpression
                            {
                                SetExpressions = [
                                    new SetUpdateExpression(Item.Keys.SourceUserId, new AttributeValueOperand(new(sourceUserId))),
                            ],
                            },
                        }, cancellationToken);

                        if (updateResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Debugger.Break();
                        }
                    }
                }
            }
        }

        public async Task GetTestItemAsync(CancellationToken cancellationToken)
        {
            var newMapId = Guid.NewGuid();

            var entity = new Map
            {
                Id = newMapId,
                /*
                Creation = DateTimeOffset.Now,
                Information = new MapInformationSettings
                {
                    Name = "TO DELETE - map from test in Milochau.Core.Aws",
                }
                */
            };

            var putResponse = await amazonDynamoDB.PutItemAsync(new PutItemRequest<Map>
            {
                UserId = null,
                Entity = entity,
            }, cancellationToken);

            var getItemResponse = await amazonDynamoDB.GetItemAsync(new GetItemRequest<Map>
            {
                UserId = null,
                PartitionKey = newMapId,
                SortKey = null,
            }, cancellationToken);

            var queryResponse = await amazonDynamoDB.QueryAsync(new QueryRequest<Map>
            {
                UserId = null,
                PartitionKeyCondition = new EqualValueExpression(Map.PartitionKey, newMapId),
                //Filters = new EqualExpression($"{Map.K_Information}.n", new AttributePath($"{Map.K_Information}.c")),
                //Filters = new InExpression($"{Map.K_Information}.n", new AttributePath($"{Map.K_Information}.d"), new AttributePath($"{Map.K_Information}.c")),
                /*Filters = new AndExpression(
                    new ContainsExpression($"{Map.K_Information}.n", new AttributeValueOperand("Map 1.6")),
                    new ContainsExpression($"{Map.K_Information}.n", new AttributePathOperand($"{Map.K_Information}.c")),
                    new InExpression($"{Map.K_Information}.c", new AttributeValueOperand("#0F9D58")),
                    new NotEqualExpression($"{Map.K_Information}.n", new AttributePathOperand($"{Map.K_Information}.c"))
                ),*/
                Limit = 1,
            }, cancellationToken);


            await amazonDynamoDB.QueryAndUpdateAllAsync<Access__Gsi_By_MapId_ThenBy_Creation, Access>(
                new QueryAndUpdateAllRequest<Access__Gsi_By_MapId_ThenBy_Creation, Access>
                {
                    UserId = null,
                    PartitionKeyCondition = new EqualValueExpression(Access__Gsi_By_MapId_ThenBy_Creation.PartitionKey, "map_id"),
                    SortKeyCondition = null,
                    Filters = null,
                    UpdateItemRequestFunction = entity => new UpdateItemRequest<Access>
                    {
                        UserId = null,
                        PartitionKey = entity.UserId,
                        SortKey = entity.MapId,
                        UpdateExpression = new UpdateExpression
                        {
                            AddExpressions = [
                            new AddUpdateExpression("a", new(entity.Creation))
                        ],
                        },
                    },
                },
                cancellationToken: cancellationToken);


            /*
            try
            {
                var updateResponse = await amazonDynamoDB.UpdateItemAsync(new UpdateItemRequest<Map>
                {
                    UserId = null,
                    PartitionKey = newMapId,
                    SortKey = null,
                    UpdateExpression = new UpdateExpression
                    {
                        RemoveExpressions = [
                            new RemoveUpdateExpression($"{Map.Keys.Information}.c"),
                        ],
                        AddExpressions = [
                            new AddUpdateExpression("cd", new AttributeValueOperand(10)),
                        ],
                        SetExpressions = [
                            new SetUpdateExpression("ooo", new AttributeValueOperand("hey hey")),
                            new SetUpdateExpression("ooofromcd", new AttributePathOperand(Map.Keys.Creation)),
                            new SetUpdateExpression("sum", new AttributePathOperand(Map.Keys.Creation) + new AttributeValueOperand(100)),
                        ]
                    }


                }, cancellationToken);
            }
            catch (Exception)
            {
            }
            */

            var deleteResponse = await amazonDynamoDB.DeleteItemAsync(new DeleteItemRequest<Map>
            {
                UserId = null,
                PartitionKey = newMapId,
                SortKey = null,
                ReturnValues = ReturnValue.ALL_OLD,
            }, cancellationToken);

            /*
            var batchResponse = await amazonDynamoDB.BatchWriteItemAsync(new BatchWriteItemRequest<Map>
            {
                UserId = null,
                RequestEntities = [
                    new PutRequest<Map>
                    {
                        Entity = entity,
                    }
                ]
            }, cancellationToken);

            var batchResponse2 = await amazonDynamoDB.BatchWriteItemAsync(new BatchWriteItemRequest<Map>
            {
                UserId = null,
                RequestEntities = [
                    new DeleteRequest<Map>
                    {
                        PartitionKey = entity.Id,
                        SortKey = null,
                    }
                ]
            }, cancellationToken);
            */
        }
    }

    [DynamoDbTable("items")]
    public partial class Item
    {
        [DynamoDbPartitionKeyAttribute("list_id")]
        public Guid ListId { get; set; }

        [DynamoDbSortKeyAttribute("id")]
        public Guid Id { get; set; }

        [DynamoDbAttribute("sui")]
        public Guid SourceUserId { get; set; }
    }

    [DynamoDbTable("accesses")]
    public partial class Access
    {
        [DynamoDbPartitionKeyAttribute("user_id")]
        public required Guid UserId { get; set; }
        [DynamoDbSortKeyAttribute("map_id")]
        public required Guid MapId { get; set; }

        [DynamoDbAttribute("cd")]
        public DateTimeOffset Creation { get; set; }

        [DynamoDbAttribute("n_s")]
        public required string NSearch { get; set; }

        [DynamoDbAttribute("mp")]
        public required MapInformationSettings Map { get; set; }
    }

    [DynamoDbIndex("accesses", "by_map_id_thenby_cd")]
    public partial class Access__Gsi_By_MapId_ThenBy_Creation
    {
        [DynamoDbPartitionKeyAttribute("map_id")]
        public required Guid MapId { get; set; }
        [DynamoDbSortKeyAttribute("cd")]
        public DateTimeOffset Creation { get; set; }

        [DynamoDbAttribute("user_id")]
        public required Guid UserId { get; set; } // Automatically projected, as it is part of key attributes
    }


    [DynamoDbTable("maps")]
    public partial class Map
    {
        [DynamoDbPartitionKeyAttribute("id")]
        public required Guid Id { get; set; }

        [DynamoDbAttribute("ListKey")]
        public List<string> List { get; set; } = ["hey"];
        [DynamoDbAttribute("NullListKey")]
        public List<string>? NullList { get; set; }

        [DynamoDbAttribute("SetKey", UseSet = true)]
        public List<string> Set { get; set; } = ["a", "c", "b"];
        [DynamoDbAttribute("NullSetKey", UseSet = true)]
        public List<string>? NullSet { get; set; }

        [DynamoDbAttribute("DoubleSetKey", UseSet = true)]
        public List<double> DoubleSet { get; set; } = [1, 3, 2];
        [DynamoDbAttribute("NullDoubleSetKey", UseSet = true)]
        public List<double>? NullDoubleSet { get; set; }

        /*
        [DynamoDbAttribute("cd")]
        public required DateTimeOffset Creation { get; set; }

        [DynamoDbAttribute("if")]
        public required MapInformationSettings? Information { get; set; }

        [DynamoDbAttribute("GuidKey")]
        public Guid Guid { get; set; } = Guid.NewGuid();
        [DynamoDbAttribute("NullGuidKey")]
        public Guid? NullGuid { get; set; }

        [DynamoDbAttribute("IntKey")]
        public int Int { get; set; }
        [DynamoDbAttribute("NullIntKey")]
        public int? NullInt { get; set; }

        [DynamoDbAttribute("LongKey")]
        public long Long { get; set; } = 10;
        [DynamoDbAttribute("NullLongKey")]
        public long? NullLong { get; set; }

        [DynamoDbAttribute("DecimalKey")]
        public decimal Decimal { get; set; } = 10.45m;
        [DynamoDbAttribute("NullDecimalKey")]
        public decimal? NullDecimal { get; set; }

        [DynamoDbAttribute("DoubleKey")]
        public double Double { get; set; } = 10.45;
        [DynamoDbAttribute("NullDoubleKey")]
        public double? NullDouble { get; set; }

        [DynamoDbAttribute("StringKey")]
        public string String { get; set; } = "  ";
        [DynamoDbAttribute("NullStringKey")]
        public string? NullString { get; set; }

        [DynamoDbAttribute("ArrayKey")]
        public string[] Array { get; set; } = ["hey"];
        [DynamoDbAttribute("NullArrayKey")]
        public string[]? NullArray { get; set; }

        [DynamoDbAttribute("DateTimeOffsetKey")]
        public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.Now;
        [DynamoDbAttribute("NullDateTimeOffsetKey")]
        public DateTimeOffset? NullDateTimeOffset { get; set; }

        [DynamoDbAttribute("DateTimeKey")]
        public DateTime DateTime { get; set; } = DateTime.Now;
        [DynamoDbAttribute("NullDateTimeKey")]
        public DateTime? NullDateTime { get; set; }

        [DynamoDbAttribute("BoolKey")]
        public bool Bool { get; set; } = true;
        [DynamoDbAttribute("NullBoolKey")]
        public bool? NullBool { get; set; }

        [DynamoDbAttribute("EnumKey")]
        public MapType Enum { get; set; } = MapType.Smth;
        [DynamoDbAttribute("NullEnumKey")]
        public MapInformationSettings? NullEnum { get; set; }

        [DynamoDbAttribute("ListKey")]
        public List<string> List { get; set; } = ["hey"];
        [DynamoDbAttribute("NullListKey")]
        public List<string>? NullList { get; set; }

        [DynamoDbAttribute("IListKey")]
        public IList<string> IList { get; set; } = ["hey"];
        [DynamoDbAttribute("NullIListKey")]
        public IList<string>? NullIList { get; set; }

        [DynamoDbAttribute("IListDecimalKey")]
        public IList<decimal> IListDecimal { get; set; } = [10m];
        [DynamoDbAttribute("NullIListDecimalKey")]
        public IList<decimal>? NullIListDecimal { get; set; }

        [DynamoDbAttribute("NullIEnumerableMapInfoKey")]
        public IEnumerable<MapInformationSettings>? NullIEnumerableMapInfo { get; set; } = [new MapInformationSettings { Name = "aa" }];

        [DynamoDbAttribute("DictionaryKey")]
        public Dictionary<string, string> Dictionary { get; set; } = new()
        {
            ["first"] = "hey",
        };

        [DynamoDbAttribute("IDictionaryKey")]
        public IDictionary<string, string> IDictionary { get; set; } = new Dictionary<string, string>()
        {
            ["first"] = "hey",
        };

        [DynamoDbAttribute("NullIDictionaryMapInfoKey")]
        public IDictionary<string, MapInformationSettings>? NullIDictionaryMapInfo { get; set; } = new Dictionary<string, MapInformationSettings>()
        {
            ["first"] = new MapInformationSettings { Name = "aa" },
        };

        [DynamoDbAttribute("NullIDictionaryDecimalKey")]
        public IDictionary<string, decimal>? NullIDictionaryDecimal { get; set; } = new Dictionary<string, decimal>()
        {
            ["first"] = 10m,
        };
        */
    }
    public enum MapType
    {
        None = 0,
        Smth = 1,
    }
}

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess.Nested
{
    [DynamoDbNested]
    public partial class MapInformationSettings
    {
        [Required]
        [StringLength(100)]
        [DynamoDbAttribute("n")]
        public required string Name { get; set; }

        [StringLength(500)]
        [DynamoDbAttribute("d")]
        public string? Desc { get; set; }

        [StringLength(50)]
        [DynamoDbAttribute("c")]
        public string? Color { get; set; }
    }
}