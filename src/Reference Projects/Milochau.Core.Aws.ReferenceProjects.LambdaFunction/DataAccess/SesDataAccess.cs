using Milochau.Core.Aws.SESv2;
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

        private readonly IAmazonSimpleEmailServiceV2 amazonSimpleEmailServiceV2;

        public SesDataAccess(IAmazonSimpleEmailServiceV2 amazonSimpleEmailServiceV2)
        {
            this.amazonSimpleEmailServiceV2 = amazonSimpleEmailServiceV2;
        }

        public async Task SendEmailAsync(FunctionRequest emailRequest, CancellationToken cancellationToken)
        {
            var response = await amazonSimpleEmailServiceV2.SendEmailAsync(new Milochau.Core.Aws.SESv2.Model.SendEmailRequest
            {
                FromEmailAddress = "noreply@dev.milochau.com",
                Destination = new Milochau.Core.Aws.SESv2.Model.Destination
                {
                    ToAddresses = new List<string> { "aaa@outlook.com" },
                },
                Content = new Milochau.Core.Aws.SESv2.Model.EmailContent
                {
                    Template = new Milochau.Core.Aws.SESv2.Model.Template
                    {
                        TemplateName = "emails-dev-template-contacts-summary",
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
                    }
                }
            }, cancellationToken);
        }
    }
}
