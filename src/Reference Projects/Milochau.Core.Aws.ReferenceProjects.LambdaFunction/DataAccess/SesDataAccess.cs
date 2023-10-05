using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface ISesDataAccess
    {
        Task SendEmailAsync(FunctionRequest emailRequest, CancellationToken cancellationToken);
    }

    public class SesDataAccess : ISesDataAccess
    {
        public static string FromAddress { get; set; } = Environment.GetEnvironmentVariable("FROM_ADDRESS")!;
        public static string ConventionsPrefix { get; set; } = Environment.GetEnvironmentVariable("CONVENTION__PREFIX")!;

        private readonly IAmazonSimpleEmailService amazonSimpleEmailService;

        public SesDataAccess(IAmazonSimpleEmailService amazonSimpleEmailService)
        {
            this.amazonSimpleEmailService = amazonSimpleEmailService;
        }

        public async Task SendEmailAsync(FunctionRequest emailRequest, CancellationToken cancellationToken)
        {
            var response = await amazonSimpleEmailService.SendTemplatedEmailAsync(new SendTemplatedEmailRequest
            {
                Source = "noreply@dev.milochau.com",
                Destination = new Destination
                {
                    ToAddresses = new List<string> { "aaa@outlook.com" },
                },
                Template = $"emails-dev-template-contacts-summary",
                TemplateData = JsonSerializer.Serialize(new EmailRequestContent
                {
                    Messages = new List<EmailRequestContentMessage>
                    {
                        new EmailRequestContentMessage
                        {
                            Message = "Ok"
                        }
                    }
                }, ApplicationJsonSerializerContext.Default.EmailRequestContent),
            }, cancellationToken);
        }
    }
}
