using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Milochau.Core.Aws.DynamoDB;
using Milochau.Core.Aws.DynamoDB.Model;
using Milochau.Core.Aws.DynamoDB.Helpers;
using System;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System.ComponentModel.DataAnnotations;
using Milochau.Core.Aws.DynamoDB.Abstractions;
//using Milochau.Core.Aws.DynamoDB.Abstractions;

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
    public partial class Map : IDynamoDbEntity<Map>, IDynamoDbGettableEntity<Map>, IDynamoDbQueryableEntity<Map>, IDynamoDbPutableEntity<Map>, IDynamoDbDeletableEntity<Map>, IDynamoDbUpdatableEntity<Map>
    {
        [DynamoDbPartitionKeyAttribute("id")]
        public required Guid Id { get; set; }

        [DynamoDbAttribute("cd")]
        public required DateTimeOffset Creation { get; set; }

        [DynamoDbAttribute("if")]
        public required MapInformationSettings Information { get; set; }
    }

    [DynamoDbNested]
    public partial class MapInformationSettings : IDynamoDbParsableEntity<MapInformationSettings>
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
