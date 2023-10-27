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
        public string EmailAddress { get; set; } = null!;
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
        public string Id { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("sender_email")]
        public string SenderEmail { get; set; } = null!;

        [JsonPropertyName("sender_name")]
        public string SenderName { get; set; } = null!;

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;
    }
}
