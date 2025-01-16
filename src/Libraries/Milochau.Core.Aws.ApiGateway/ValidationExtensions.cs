using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>Extensions methods used for validation</summary>
    public static class ValidationExtensions
    {
        /// <summary>Try validate model</summary>
        public static bool TryValidateModel<TModel, TValidator>(TModel model, [NotNullWhen(false)] out ValidationProblemDetails? validationProblemDetails)
            where TValidator : IValidateOptions<TModel>, new() where TModel : class
        {
            validationProblemDetails = null;
            var validator = new TValidator();
            var validateOptionsResult = validator.Validate(null, model);
            if (validateOptionsResult.Failed)
            {
                var modelStateDictionary = new ModelStateDictionary();
                modelStateDictionary.AddModelError(string.Empty, validateOptionsResult.FailureMessage);
                validationProblemDetails = new ValidationProblemDetails(modelStateDictionary);
                return false;
            }
            return true;
        }
    }
}
