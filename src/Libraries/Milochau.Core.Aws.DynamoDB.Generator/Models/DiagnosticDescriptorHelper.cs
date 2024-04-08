using Microsoft.CodeAnalysis;

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
            LocalizableString? description = null)
        {
            return new DiagnosticDescriptor(id, title, messageFormat, category, defaultSeverity, isEnabledByDefault, description);
        }
    }
}
