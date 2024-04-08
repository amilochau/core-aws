using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Milochau.Core.Aws.DynamoDB.Generator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Generator
{
    [Generator]
    public class DynamoDbFormattableEntity : IIncrementalGenerator
    {
        public DynamoDbFormattableEntity()
        {
#if DEBUG
            // Uncomment the following lines to debug code generation - you should then select a VS instance
            /*
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
            */
#endif
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<DynamoDbClassToGenerate?> contextGenerationSpecsTable = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "Milochau.Core.Aws.DynamoDB.Abstractions.DynamoDbTableAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => GetDynamoDbClassToGenerate(context, ClassType.Table))
                 .Where(static m => m is not null);

            IncrementalValuesProvider<DynamoDbClassToGenerate?> contextGenerationSpecsProjection = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "Milochau.Core.Aws.DynamoDB.Abstractions.DynamoDbProjectionAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => GetDynamoDbClassToGenerate(context, ClassType.Projection))
                 .Where(static m => m is not null);

            IncrementalValuesProvider<DynamoDbClassToGenerate?> contextGenerationSpecsIndex = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "Milochau.Core.Aws.DynamoDB.Abstractions.DynamoDbIndexAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => GetDynamoDbClassToGenerate(context, ClassType.Index))
                 .Where(static m => m is not null);

            IncrementalValuesProvider<DynamoDbClassToGenerate?> contextGenerationSpecsNested = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "Milochau.Core.Aws.DynamoDB.Abstractions.DynamoDbNestedAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => GetDynamoDbClassToGenerate(context, ClassType.Nested))
                 .Where(static m => m is not null);

            context.RegisterSourceOutput(contextGenerationSpecsTable, Execute);
            context.RegisterSourceOutput(contextGenerationSpecsProjection, Execute);
            context.RegisterSourceOutput(contextGenerationSpecsIndex, Execute);
            context.RegisterSourceOutput(contextGenerationSpecsNested, Execute);
        }

        private static void Execute(SourceProductionContext context, DynamoDbClassToGenerate? dynamoDbTableToGenerate)
        {
            if (dynamoDbTableToGenerate is { } value)
            {
                // Report any diagnostics ahead of emitting.
                foreach (var diagnostic in value.Diagnostics)
                {
                    context.ReportDiagnostic(diagnostic.CreateDiagnostic());
                }

                // generate the source code and add it to the output
                string result = SourceGenerationHelper.GeneratePartialClass(value);
                // Create a separate partial class file for each enum
                context.AddSource($"DynamoDbTables.{value.Class}.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }

        private static DynamoDbClassToGenerate? GetDynamoDbClassToGenerate(GeneratorAttributeSyntaxContext context, ClassType classType)
        {
            var diagnostics = new List<DiagnosticInfo>();

            if (context.TargetNode is not ClassDeclarationSyntax cds)
            {
                return null; // Something went wrong
            }

            INamedTypeSymbol? contextTypeSymbol = context.SemanticModel.GetDeclaredSymbol(cds);
            if (contextTypeSymbol == null)
            {
                return null; // Something went wrong
            }

            var _contextClassLocation = contextTypeSymbol.Locations.FirstOrDefault();
            if (_contextClassLocation == null)
            {
                return null; // Something went wrong
            }

            if (!IsPartialType(cds))
            {
                diagnostics.Add(new DiagnosticInfo(DiagnosticDescriptors.ContextClassesMustBePartial, _contextClassLocation, contextTypeSymbol.Name));
            }

            if (context.SemanticModel.GetDeclaredSymbol(context.TargetNode) is not INamedTypeSymbol typeSymbol)
            {
                return null; // Something went wrong
            }

            string? expectedClassAttributeName = classType switch
            {
                ClassType.Table => SourceGenerationHelper.DynamoDbTableAttributeName,
                ClassType.Projection => SourceGenerationHelper.DynamoDbProjectionAttributeName,
                ClassType.Index => SourceGenerationHelper.DynamoDbIndexAttributeName,
                ClassType.Nested => SourceGenerationHelper.DynamoDbNestedAttributeName,
                _ => null,
            };

            var dynamoDbTableAttribute = context.Attributes.FirstOrDefault(x => x.AttributeClass?.Name == expectedClassAttributeName);
            if (dynamoDbTableAttribute == null)
            {
                return null; // Something went wrong
            }

            var @namespace = GetNamespace(cds);
            (string? tableNameSuffix, string? indexName) = GetClassSettings(dynamoDbTableAttribute, classType);
            var dynamoDbAttributes = GetClassProperties(typeSymbol);

            return new DynamoDbClassToGenerate(@namespace, typeSymbol.Name, tableNameSuffix, indexName, classType, dynamoDbAttributes, diagnostics); ;
        }

        private static AttributeType GetAttributeType(string typeName)
        {
            return typeName switch
            {
                "string" => AttributeType.String,
                "int" => AttributeType.Int,
                "long" => AttributeType.Long,
                "decimal" => AttributeType.Decimal,
                "double" => AttributeType.Double,
                "bool" => AttributeType.Boolean,
                "System.Guid" => AttributeType.Guid,
                "System.DateTimeOffset" => AttributeType.DateTimeOffset,
                "System.DateTime" => AttributeType.DateTime,
                _ => AttributeType.Object,
            };
        }

        private static (string? tableNameSuffix, string? indexName) GetClassSettings(AttributeData attributeData, ClassType classType)
        {
            return classType switch
            {
                ClassType.Table => (attributeData.ConstructorArguments[0].Value!.ToString(), null),
                ClassType.Projection => (attributeData.ConstructorArguments[0].Value!.ToString(), attributeData.NamedArguments.SingleOrDefault(x => x.Key == SourceGenerationHelper.DynamoDbAttribute_IndexName).Value.Value?.ToString()),
                ClassType.Index => (attributeData.ConstructorArguments[0].Value!.ToString(), attributeData.ConstructorArguments[1].Value!.ToString()),
                _ => (null, null),
            };
        }

        private static List<DynamoDbAttributeToGenerate> GetClassProperties(INamedTypeSymbol typeSymbol)
        {
            var dynamoDbAttributes = new List<DynamoDbAttributeToGenerate>();
            var members = typeSymbol.GetMembers();
            foreach (var member in members)
            {
                if (member is not IPropertySymbol propertyMember)
                {
                    continue;
                }

                var memberAttributes = propertyMember.GetAttributes();
                var memberAttribute = memberAttributes.FirstOrDefault(x =>
                       x.AttributeClass?.Name == SourceGenerationHelper.DynamoDbAttributeAttributeName
                    || x.AttributeClass?.Name == SourceGenerationHelper.DynamoDbAttributeAttributeName_PartitionKey
                    || x.AttributeClass?.Name == SourceGenerationHelper.DynamoDbAttributeAttributeName_SortKey);
                if (memberAttribute == null)
                {
                    continue;
                }
                var attributeCategory = memberAttribute.AttributeClass!.Name switch
                {
                    SourceGenerationHelper.DynamoDbAttributeAttributeName_PartitionKey => AttributeCategory.Partition,
                    SourceGenerationHelper.DynamoDbAttributeAttributeName_SortKey => AttributeCategory.Sort,
                    _ => AttributeCategory.Regular,
                };

                if (propertyMember.Type is not INamedTypeSymbol propertyMemberNamedType)
                {
                    continue;
                }

                var propertyMemberType_NotNullable = propertyMemberNamedType.OriginalDefinition.Name == "Nullable" ? propertyMemberNamedType.TypeArguments[0] : propertyMemberNamedType;
                var propertyMemberType_NotNullable_Named = propertyMemberType_NotNullable as INamedTypeSymbol;


                var type = propertyMemberType_NotNullable.OriginalDefinition.ToString();
                var attributeKey = memberAttribute.ConstructorArguments[0].Value!.ToString();
                var attributeType = GetAttributeType(type);
                var isNullable = propertyMemberNamedType.NullableAnnotation == NullableAnnotation.Annotated;
                var isList = false;
                var isDictionary = false;

                if (attributeType == AttributeType.Object && propertyMemberType_NotNullable.TypeKind == TypeKind.Enum)
                {
                    attributeType = AttributeType.Enum;
                }
                else if (attributeType == AttributeType.Object && (
                    propertyMemberType_NotNullable.Interfaces.Any(x => x.Name == "IDictionary") || propertyMemberType_NotNullable_Named?.ConstructedFrom.Name == "IDictionary"
                    ))
                {
                    // We want to avoid string as IEnumerable
                    isDictionary = true;
                    if (propertyMemberType_NotNullable_Named != null)
                    {
                        type = propertyMemberType_NotNullable_Named.TypeArguments[1].OriginalDefinition.ToString();
                        attributeType = GetAttributeType(type);
                    }
                }
                else if (attributeType == AttributeType.Object && propertyMemberType_NotNullable.Interfaces.Any(x => x.Name == "IEnumerable"))
                {
                    isList = true;
                    if (propertyMemberType_NotNullable_Named != null)
                    {
                        type = propertyMemberType_NotNullable_Named.TypeArguments[0].OriginalDefinition.ToString();
                        attributeType = GetAttributeType(type);
                    }
                }

                dynamoDbAttributes.Add(new DynamoDbAttributeToGenerate(type, propertyMember.Name, attributeKey, attributeType, attributeCategory, isNullable, isList, isDictionary));
            }
            return dynamoDbAttributes;
        }

        private static bool IsPartialType(ClassDeclarationSyntax contextClassSyntax)
        {
            for (TypeDeclarationSyntax? currentType = contextClassSyntax; currentType != null; currentType = currentType.Parent as TypeDeclarationSyntax)
            {
                bool isPartialType = false;

                foreach (SyntaxToken modifier in currentType.Modifiers)
                {
                    isPartialType |= modifier.IsKind(SyntaxKind.PartialKeyword);
                }

                if (!isPartialType)
                {
                    return false;
                }
            }

            return true;
        }

        static string GetNamespace(BaseTypeDeclarationSyntax syntax)
        {
            // If we don't have a namespace at all we'll return an empty string
            // This accounts for the "default namespace" case
            string nameSpace = string.Empty;

            if (syntax == null)
            {
                return nameSpace;
            }

            // Get the containing syntax node for the type declaration
            // (could be a nested type, for example)
            SyntaxNode? potentialNamespaceParent = syntax.Parent;

            // Keep moving "out" of nested classes etc until we get to a namespace
            // or until we run out of parents
            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            // Build up the final namespace by looping until we no longer have a namespace declaration
            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                // We have a namespace. Use that as the type
                nameSpace = namespaceParent.Name.ToString();

                // Keep moving "out" of the namespace declarations until we 
                // run out of nested namespace declarations
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    // Add the outer namespace as a prefix to the final namespace
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            // return the final namespace
            return nameSpace;
        }
    }
}
