using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Milochau.Core.Aws.DynamoDB;
using Milochau.Core.Aws.DynamoDB.Model;
using System.Linq;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.Core.References;
using System;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System.ComponentModel.DataAnnotations;

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
                    new ContainsExpression($"{Map.K_Information}.n", "Map 1.6"),
                    new ContainsExpression($"{Map.K_Information}.n", new AttributePath($"{Map.K_Information}.c")),
                    new InExpression($"{Map.K_Information}.c", "#0F9D58"),
                    new NotEqualExpression($"{Map.K_Information}.n", new AttributePath($"{Map.K_Information}.c"))
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
                            new AddUpdateExpression("cd", 10),
                        ],
                        SetExpressions = [
                            new SetUpdateExpression("ooo", "hey hey"),
                            new SetUpdateExpression("ooofromcd", new AttributePath(Map.K_Creation)),
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

    public class Map : IDynamoDbEntity<Map>, IDynamoDbGettableEntity<Map>, IDynamoDbQueryableEntity<Map>, IDynamoDbPutableEntity<Map>, IDynamoDbDeletableEntity<Map>, IDynamoDbUpdatableEntity<Map>
    {
        public const string TableNameSuffix = "maps";

        public const string K_Id = "id";
        public const string K_Creation = "cd";
        public const string K_Information = "if";

        public required Guid Id { get; set; }
        public required DateTimeOffset Creation { get; set; }
        public required MapInformationSettings Information { get; set; }

        public static string TableName => $"{EnvironmentVariables.ConventionPrefix}-table-maps";
        //public static IEnumerable<string>? ProjectedAttributes => [K_Id, K_Creation, K_Information];

        public Dictionary<string, AttributeValue> FormatForDynamoDb()
        {
            return new Dictionary<string, AttributeValue>()
                .Append(K_Id, Id)
                .Append(K_Creation, Creation)
                .Append(K_Information, Information)
                .ToDictionary();
        }

        public static Map ParseFromDynamoDb(Dictionary<string, AttributeValue> attributes)
        {
            return new Map
            {
                Id = attributes.ReadGuid(K_Id),
                Creation = attributes.ReadDateTimeOffset(K_Creation),
                Information = attributes.ReadObject<MapInformationSettings>(K_Information),
            };
        }
    }

    public class MapInformationSettings : IDynamoDbEntity<MapInformationSettings>
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Desc { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        public Dictionary<string, AttributeValue> FormatForDynamoDb()
        {
            return new Dictionary<string, AttributeValue>()
                .Append("n", Name)
                .Append("d", Desc)
                .Append("c", Color)
                .ToDictionary();
        }

        public static MapInformationSettings ParseFromDynamoDb(Dictionary<string, AttributeValue> attributes)
        {
            return new MapInformationSettings
            {
                Name = attributes.ReadString("n"),
                Desc = attributes.ReadStringOptional("d"),
                Color = attributes.ReadStringOptional("c"),
            };
        }
    }

    public class FakeMap : IDynamoDbEntity<FakeMap>
    {
        public required string Title { get; set; }
        public required MapIcon FirstIcon { get; set; }
        public MapIcon? SecondIcon { get; set; }
        public required List<MapIcon> MoreIcons { get; set; }
        public List<MapIcon>? MoreOptionalIcons { get; set; }

        public Dictionary<string, AttributeValue> FormatForDynamoDb()
        {
            return new Dictionary<string, AttributeValue>()
                .Append("title", Title)
                .Append("first_icon", FirstIcon)
                .Append("second_icon", SecondIcon)
                .Append("more_icons", MoreIcons)
                .Append("more_optional_icons", MoreOptionalIcons)
                .ToDictionary();
        }

        public static FakeMap ParseFromDynamoDb(Dictionary<string, AttributeValue> attributes)
        {
            return new FakeMap
            {
                Title = attributes.ReadString("title"),
                FirstIcon = attributes.ReadObject<MapIcon>("first_icon"),
                SecondIcon = attributes.ReadObjectOptional<MapIcon>("second_icon"),
                MoreIcons = attributes.ReadList<MapIcon>("more_icons"),
                MoreOptionalIcons = attributes.ReadListOptional<MapIcon>("more_icons"),
            };
        }
    }

    public class MapIcon : IDynamoDbEntity<MapIcon>
    {
        public required string Mdi { get; set; }

        public Dictionary<string, AttributeValue> FormatForDynamoDb()
        {
            return new Dictionary<string, AttributeValue>()
                .Append("mdi", Mdi)
                .ToDictionary();
        }

        public static MapIcon ParseFromDynamoDb(Dictionary<string, AttributeValue> attributes)
        {
            return new MapIcon
            {
                Mdi = attributes.ReadString("mdi"),
            };
        }
    }
}
