using System;
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
            var response = await amazonDynamoDB.GetItemAsync(new GetItemRequest
            {
                TableName = $"{EnvironmentVariables.ConventionPrefix}-table-test",
                Key = new Dictionary<string, AttributeValue>()
                    .Append("id", "0dc388584487498c98c98a4b9d2cad3c")
                    .ToDictionary(x => x.Key, x => x.Value),
            }, cancellationToken);
        }
    }
}
