using Milochau.Core.Aws.Core.References;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public class EmailRequest
    {
        public string TemplateId { get; }
        public string? UnsubscribeFunctionName { get; }

        public List<EmailRequestRecipient> Tos { get; set; }
        public string RawTemplateData { get; set; }

        public EmailRequest(List<EmailRequestRecipient> tos, string rawTemplateData)
        {
            TemplateId = "contacts-summary";
            UnsubscribeFunctionName = $"contacts-{EnvironmentVariables.ConventionHost}-fn-async-unsubscribe-emails";
            Tos = tos;
            RawTemplateData = rawTemplateData;
        }
    }

    public class EmailRequestRecipient
    {
        public required string EmailAddress { get; set; }
    }

    public class EmailRequestContent
    {
        [JsonPropertyName("unsubscribe_url")]
        public string UnsubscribeUrl { get; set; } = "__UNSUBSCRIBE_URL__";

        [JsonPropertyName("messages")]
        public List<EmailRequestContentMessage> Messages { get; set; } = new List<EmailRequestContentMessage>();
    }

    public class EmailRequestContentMessage
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("status")]
        public required string Status { get; set; }

        [JsonPropertyName("sender_email")]
        public required string SenderEmail { get; set; }

        [JsonPropertyName("sender_name")]
        public required string SenderName { get; set; }

        [JsonPropertyName("message")]
        public required string Message { get; set; }
    }
}
