using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Milochau.Core.Aws.DynamoDB;
using Milochau.Core.Aws.DynamoDB.Model;
using System.Linq;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.Core.References;

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
            var response = await amazonDynamoDB.GetItemAsync(new GetItemRequest(null)
            {
                TableName = $"{EnvironmentVariables.ConventionPrefix}-table-maps",
                Key = new Dictionary<string, AttributeValue>()
                    .Append("id", "0dc388584487498c98c98a4b9d2cad3c")
                    .ToDictionary(),
            }, cancellationToken);
        }
    }

    public class Map : IDynamoDbEntity<Map>
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

        public static Map ParseFromDynamoDb(Dictionary<string, AttributeValue> attributes)
        {
            return new Map
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
