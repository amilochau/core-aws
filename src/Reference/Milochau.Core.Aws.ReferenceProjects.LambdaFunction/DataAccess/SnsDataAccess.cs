using Milochau.Core.Aws.SNS;
using Milochau.Core.Aws.SNS.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface ISnsDataAccess
    {
        Task SendEventAsync(FunctionRequest emailRequest, CancellationToken cancellationToken);
    }

    public class SnsDataAccess : ISnsDataAccess
    {
        private readonly IAmazonSimpleNotificationService amazonSimpleNotificationService;

        public SnsDataAccess(IAmazonSimpleNotificationService amazonSimpleNotificationService)
        {
            this.amazonSimpleNotificationService = amazonSimpleNotificationService;
        }

        public async Task SendEventAsync(FunctionRequest emailRequest, CancellationToken cancellationToken)
        {
            var response = await amazonSimpleNotificationService.PublishAsync(new PublishRequest(null)
            {
                TopicArn = "arn:aws:sns:eu-west-3:381492034295:todelete-topic",
                Message = "Hey!!",
            }, cancellationToken);
        }
    }
}
