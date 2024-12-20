using Milochau.Core.Aws.Core.Lambda.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>Parsable request</summary>
    public interface IParsable<TSelf>
    {
        /// <summary>Try parse an API Gateway request</summary>
        abstract static bool TryParse(APIGatewayHttpApiV2ProxyRequest request, [NotNullWhen(true)] out TSelf? result);
    }

    /// <summary>Validatable request</summary>
    public interface IValidatable<TSelf>
    {
        /// <summary>Validate the class</summary>
        void Validate(Dictionary<string, Collection<string>> modelStateDictionary);
    }

    /// <summary>Parsable and validatable request</summary>
    public interface IParsableAndValidatable<TSelf> : IParsable<TSelf>, IValidatable<TSelf>
    {
    }

    /// <summary>Validation options</summary>
    public class ValidationOptions
    {
        /// <summary>Whether the authentication is required</summary>
        public bool AuthenticationRequired { get; set; } = true;

        /// <summary>List of required Cognito groups</summary>
        public List<string> GroupsRequired { get; set; } = new List<string>();
    }

    /// <summary>Validation problem details</summary>
    public class ValidationProblemDetails
    {
        /// <summary>Problem type</summary>
        public string? Type { get; set; }

        /// <summary>Problem title</summary>
        public string? Title { get; set; }

        /// <summary>Problem status</summary>
        public int? Status { get; set; }

        /// <summary>Problem status</summary>
        public string? Detail { get; set; }

        /// <summary>Problem errors</summary>
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>(StringComparer.Ordinal);

        /// <summary>Default constructor</summary>
        public ValidationProblemDetails()
        {
            Title = "One or more validation errors occurred.";
        }

        /// <summary>Constructor</summary>
        public ValidationProblemDetails(ReadOnlyDictionary<string, Collection<string>> modelState)
        {
            ArgumentNullException.ThrowIfNull(modelState);

            foreach (KeyValuePair<string, Collection<string>> item in modelState)
            {
                string key = item.Key;
                Collection<string> errors = item.Value;
                if (errors == null || errors.Count <= 0)
                {
                    continue;
                }

                Errors.Add(key, errors.ToArray());
            }
        }
    }
}
