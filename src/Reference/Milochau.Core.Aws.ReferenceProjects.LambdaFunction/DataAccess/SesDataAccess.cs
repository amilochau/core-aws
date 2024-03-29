﻿using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.SESv2;
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
        public static string FromAddress { get; set; } = EnvironmentVariables.GetEnvironmentVariable("FROM_ADDRESS")!;
        public static string ConventionsPrefix { get; set; } = EnvironmentVariables.ConventionPrefix;

        private readonly IAmazonSimpleEmailServiceV2 amazonSimpleEmailServiceV2;

        public SesDataAccess(IAmazonSimpleEmailServiceV2 amazonSimpleEmailServiceV2)
        {
            this.amazonSimpleEmailServiceV2 = amazonSimpleEmailServiceV2;
        }

        public async Task SendEmailAsync(FunctionRequest emailRequest, CancellationToken cancellationToken)
        {
            _ = await amazonSimpleEmailServiceV2.SendEmailAsync(new SESv2.Model.SendEmailRequest(null)
            {
                FromEmailAddress = "noreply@dev.milochau.com",
                Destination = new SESv2.Model.Destination
                {
                    ToAddresses = new List<string> { "aaa@outlook.com" },
                },
                Content = new SESv2.Model.EmailContent
                {
                    Template = new SESv2.Model.Template
                    {
                        TemplateName = "emails-dev-template-contacts-summary",
                        TemplateData = JsonSerializer.Serialize(new EmailRequestContent
                        {
                            Messages = new List<EmailRequestContentMessage>
                            {
                                new EmailRequestContentMessage
                                {
                                    Id = System.Guid.NewGuid().ToString(),
                                    Message = "Ok",
                                    SenderEmail = "aaa@outlook.com",
                                    SenderName = "aaa",
                                    Status = "OK",
                                }
                            }
                        }, ApplicationJsonSerializerContext.Default.EmailRequestContent),
                    }
                }
            }, cancellationToken);
        }
    }
}
