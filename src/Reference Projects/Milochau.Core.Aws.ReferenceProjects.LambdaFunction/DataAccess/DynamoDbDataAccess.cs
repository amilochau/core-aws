using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Milochau.Core.Aws.DynamoDB;
using Milochau.Core.Aws.DynamoDB.Model;
using System.Linq;
using Milochau.Core.Aws.DynamoDB.Helpers;

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
            var response = await amazonDynamoDB.GetItemAsync(new GetItemRequest
            {
                TableName = $"{ConventionsPrefix}-table-maps",
                Key = new Dictionary<string, AttributeValue>()
                    .Append("id", "0dc388584487498c98c98a4b9d2cad3c")
                    .ToDictionary(x => x.Key, x => x.Value),
            }, cancellationToken);
        }
    }
}
