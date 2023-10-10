using Milochau.Core.Aws.Lambda;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface IEmailsLambdaDataAccess
    {
        Task SendSummaryAsync(CancellationToken cancellationToken);
    }

    public class EmailsLambdaDataAccess : IEmailsLambdaDataAccess
    {
        public static string ConventionsHost { get; set; } = Environment.GetEnvironmentVariable("CONVENTION__HOST")!;
        private readonly IAmazonLambda amazonLambda;

        public EmailsLambdaDataAccess(IAmazonLambda amazonLambda)
        {
            this.amazonLambda = amazonLambda;
        }

        public async Task SendSummaryAsync(CancellationToken cancellationToken)
        {
            var emailRequest = new EmailRequest(new System.Collections.Generic.List<EmailRequestRecipient>
            {
                new EmailRequestRecipient
                {
                    EmailAddress = "aaa@oulook.com",
                }
            }, JsonSerializer.Serialize(new EmailRequestContent
            {
                Messages = new System.Collections.Generic.List<EmailRequestContentMessage>
                {
                    new EmailRequestContentMessage
                    {
                        Id = "id",
                        Status = "status",
                        Message = "message",
                        SenderEmail = "sender email",
                        SenderName = "sender name",
                    }
                }
            }, ApplicationJsonSerializerContext.Default.EmailRequestContent));

            await amazonLambda.InvokeAsync(new Lambda.Model.InvokeRequest
            {
                FunctionName = $"emails-{ConventionsHost}-fn-async-send-emails",
                InvocationType = InvocationType.Event,
                Payload = JsonSerializer.Serialize(emailRequest, ApplicationJsonSerializerContext.Default.EmailRequest),
            }, cancellationToken);
        }
    }
}
