﻿using Milochau.Core.Aws.Lambda;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface IEmailsLambdaDataAccess
    {
        Task SendSummaryAsync(EmailRequest emailRequest, CancellationToken cancellationToken);
    }

    public class EmailsLambdaDataAccess : IEmailsLambdaDataAccess
    {
        public static string ConventionsHost { get; set; } = Environment.GetEnvironmentVariable("CONVENTION__HOST")!;
        private readonly IAmazonLambda amazonLambda;

        public EmailsLambdaDataAccess(IAmazonLambda amazonLambda)
        {
            this.amazonLambda = amazonLambda;
        }

        public async Task SendSummaryAsync(EmailRequest emailRequest, CancellationToken cancellationToken)
        {
            await amazonLambda.InvokeAsync(new Milochau.Core.Aws.Lambda.Model.InvokeRequest
            {
                FunctionName = $"emails-{ConventionsHost}-fn-async-send-emails",
                InvocationType = InvocationType.Event,
                Payload = JsonSerializer.Serialize(emailRequest, ApplicationJsonSerializerContext.Default.EmailRequest),
            }, cancellationToken);
        }
    }
}
