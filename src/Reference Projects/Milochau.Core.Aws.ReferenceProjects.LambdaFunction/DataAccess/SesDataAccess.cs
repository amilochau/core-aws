using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System;
using System.Collections.Generic;
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
            await amazonSimpleEmailService.SendTemplatedEmailAsync(new SendTemplatedEmailRequest
            {
                Source = FromAddress,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { "test@test.com" },
                },
                Template = $"{ConventionsPrefix}-template-templateId",
                TemplateData = "templateData",
            }, cancellationToken);
        }
    }
}
