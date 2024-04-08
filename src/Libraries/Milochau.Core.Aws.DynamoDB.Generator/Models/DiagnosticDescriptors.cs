using Microsoft.CodeAnalysis;

namespace Milochau.Core.Aws.DynamoDB.Generator.Models
{
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
