using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Milochau.Core.Aws.DynamoDB;
using Milochau.Core.Aws.DynamoDB.Model;
using System;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System.ComponentModel.DataAnnotations;
using Milochau.Core.Aws.DynamoDB.Abstractions;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess.Nested;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface IDynamoDbDataAccess
    {
        Task GetTestItemAsync(CancellationToken cancellationToken);
    }

    public class DynamoDbDataAccess : IDynamoDbDataAccess
    {
        protected readonly IAmazonDynamoDB amazonDynamoDB;

        public DynamoDbDataAccess(IAmazonDynamoDB amazonDynamoDB)
        {
            this.amazonDynamoDB = amazonDynamoDB;
        }

        public async Task GetTestItemAsync(CancellationToken cancellationToken)
        {

            var newMapId = Guid.NewGuid();
            var putResponse = await amazonDynamoDB.PutItemAsync(new PutItemRequest<Map>
            {
                UserId = null,
                Entity = new Map
                {
                    Id = newMapId,
                    Creation = DateTimeOffset.Now,
                    Information = new MapInformationSettings
                    {
                        Name = "TO DELETE - map from test in Milochau.Core.Aws",
                    }
                },
            }, cancellationToken);

            var getItemResponse = await amazonDynamoDB.GetItemAsync(new GetItemRequest<Map>
            {
                UserId = null,
                PartitionKey = new(Map.K_Id, newMapId),
                SortKey = null,
            }, cancellationToken);

            var queryResponse = await amazonDynamoDB.QueryAsync(new QueryRequest<Map>
            {
                UserId = null,
                PartitionKeyCondition = new EqualValueExpression(Map.K_Id, newMapId),
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

            try
            {
                var updateResponse = await amazonDynamoDB.UpdateItemAsync(new UpdateItemRequest<Map>
                {
                    UserId = null,
                    PartitionKey = new(Map.K_Id, newMapId),
                    SortKey = null,
                    UpdateExpression = new UpdateExpression
                    {
                        RemoveExpressions = [
                            new RemoveUpdateExpression($"{Map.K_Information}.c"),
                        ],
                        AddExpressions = [
                            new AddUpdateExpression("cd", new AttributeValueOperand(10)),
                        ],
                        SetExpressions = [
                            new SetUpdateExpression("ooo", new AttributeValueOperand("hey hey")),
                            new SetUpdateExpression("ooofromcd", new AttributePathOperand(Map.K_Creation)),
                            new SetUpdateExpression("sum", new AttributePathOperand(Map.K_Creation) + new AttributeValueOperand(100)),
                        ]
                    }


                }, cancellationToken);
            }
            catch (Exception)
            {
            }

            var deleteResponse = await amazonDynamoDB.DeleteItemAsync(new DeleteItemRequest<Map>
            {
                UserId = null,
                PartitionKey = new(Map.K_Id, newMapId),
                SortKey = null,
                ReturnValues = ReturnValue.ALL_OLD,
            }, cancellationToken);
        }
    }

    [DynamoDbTable("maps")]
    public partial class Map
    {
        [DynamoDbPartitionKeyAttribute("id")]
        public required Guid Id { get; set; }

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