using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Generator.Models
{
    internal static partial class DiagnosticDescriptorHelper
    {
        public static DiagnosticDescriptor Create(
            string id,
            LocalizableString title,
            LocalizableString messageFormat,
            string category,
            DiagnosticSeverity defaultSeverity,
            bool isEnabledByDefault,
            LocalizableString? description = null,
            params string[] customTags)
        {
            string helpLink = $"https://learn.microsoft.com/dotnet/fundamentals/syslib-diagnostics/{id.ToLowerInvariant()}";

            return new DiagnosticDescriptor(id, title, messageFormat, category, defaultSeverity, isEnabledByDefault, description, helpLink, customTags);
        }
    }

    internal static class DiagnosticDescriptors
    {
        private const string category = "Milochau.Core.Aws.DynamoDB.Generator";

        public static DiagnosticDescriptor ContextClassesMustBePartial { get; } = DiagnosticDescriptorHelper.Create(
            id: "MIL001",
            title: "Classes decorated with the DynamoDB attributes must be partial.",
            messageFormat: "Class {0} uses a DynamoDB attribute. The class must be made partial to kick off source generation.",
            category: category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    }
}
