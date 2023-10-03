using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Milochau.Core.Aws.DynamoDB.DynamoDBv2;
using Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model;
using Milochau.Core.Aws.DynamoDB;
using System.Linq;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface IDynamoDbDataAccess
    {
        Task GetMessageAsync(CancellationToken cancellationToken);
    }

    public class DynamoDbDataAccess : IDynamoDbDataAccess
    {
        public static string ConventionsPrefix { get; set; } = Environment.GetEnvironmentVariable("CONVENTION__PREFIX")!;

        protected readonly IAmazonDynamoDB amazonDynamoDB;

        public DynamoDbDataAccess(IAmazonDynamoDB amazonDynamoDB)
        {
            this.amazonDynamoDB = amazonDynamoDB;
        }

        public async Task GetMessageAsync(CancellationToken cancellationToken)
        {
            await amazonDynamoDB.GetItemAsync(new GetItemRequest
            {
                TableName = $"{ConventionsPrefix}-table-messages",
                Key = new Dictionary<string, AttributeValue>()
                    .Append("id", "messageId")
                    .ToDictionary(x => x.Key, x => x.Value),
            }, cancellationToken);
        }
    }
}
