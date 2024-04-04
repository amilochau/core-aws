using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
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
            IncrementalValuesProvider<DynamoDbToGenerate?> contextGenerationSpecsTable = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "SourceGenerator.Abstractions.DynamoDbTableAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => GetDynamoDbTableToGenerate(context, ClassType.Table))
                 .Where(static m => m is not null);

            IncrementalValuesProvider<DynamoDbToGenerate?> contextGenerationSpecsProjection = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "SourceGenerator.Abstractions.DynamoDbProjectionAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => GetDynamoDbTableToGenerate(context, ClassType.Projection))
                 .Where(static m => m is not null);

            IncrementalValuesProvider<DynamoDbToGenerate?> contextGenerationSpecsIndex = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "SourceGenerator.Abstractions.DynamoDbIndexAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => GetDynamoDbTableToGenerate(context, ClassType.Index))
                 .Where(static m => m is not null);

            IncrementalValuesProvider<DynamoDbToGenerate?> contextGenerationSpecsNested = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "SourceGenerator.Abstractions.DynamoDbNestedAttribute",
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (context, _) => GetDynamoDbTableToGenerate(context, ClassType.Nested))
                 .Where(static m => m is not null);

            context.RegisterSourceOutput(contextGenerationSpecsTable, Execute);
            context.RegisterSourceOutput(contextGenerationSpecsProjection, Execute);
            context.RegisterSourceOutput(contextGenerationSpecsIndex, Execute);
            context.RegisterSourceOutput(contextGenerationSpecsNested, Execute);
        }

        private static void Execute(SourceProductionContext context, DynamoDbToGenerate? dynamoDbTableToGenerate)
        {
            if (dynamoDbTableToGenerate is { } value)
            {
                // generate the source code and add it to the output
                string result = SourceGenerationHelper.GeneratePartialClass(value);
                // Create a separate partial class file for each enum
                context.AddSource($"DynamoDbTables.{value.Class}.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }

        private static AttributeType GetAttributeType(string typeName)
        {
            return typeName switch
            {
                "String" => AttributeType.String,
                "Guid" => AttributeType.Guid,
                "Int32" => AttributeType.Int,
                "Int64" => AttributeType.Int,
                "Decimal" => AttributeType.Decimal,
                "Double" => AttributeType.Double,
                "Boolean" => AttributeType.Boolean,
                "DateTimeOffset" => AttributeType.DateTimeOffset,
                "DateTime" => AttributeType.DateTime,
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
                var memberAttribute = memberAttributes.FirstOrDefault(x => x.AttributeClass?.Name == SourceGenerationHelper.DynamoDbAttributeAttributeName);
                if (memberAttribute == null)
                {
                    continue;
                }

                if (propertyMember.Type is not INamedTypeSymbol propertyMemberNamedType)
                {
                    continue;
                }

                var propertyMemberType_NotNullable = propertyMemberNamedType.OriginalDefinition.Name == "Nullable" ? propertyMemberNamedType.TypeArguments[0] : propertyMemberNamedType;
                var propertyMemberType_NotNullable_Named = propertyMemberType_NotNullable as INamedTypeSymbol;


                var type = propertyMemberType_NotNullable.Name;
                var attributeKey = memberAttribute.ConstructorArguments[0].Value!.ToString();
                var attributeType = GetAttributeType(propertyMemberType_NotNullable.Name);
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
                        type = propertyMemberType_NotNullable_Named.TypeArguments[1].Name;
                        attributeType = GetAttributeType(propertyMemberType_NotNullable_Named.TypeArguments[1].Name);
                    }
                }
                else if (attributeType == AttributeType.Object && propertyMemberType_NotNullable.Interfaces.Any(x => x.Name == "IEnumerable"))
                {
                    isList = true;
                    if (propertyMemberType_NotNullable_Named != null)
                    {
                        type = propertyMemberType_NotNullable_Named.TypeArguments[0].Name;
                        attributeType = GetAttributeType(propertyMemberType_NotNullable_Named.TypeArguments[0].Name);
                    }
                }

                dynamoDbAttributes.Add(new DynamoDbAttributeToGenerate(type, propertyMember.Name, attributeKey, attributeType, isNullable, isList, isDictionary));
            }
            return dynamoDbAttributes;
        }

        private static DynamoDbToGenerate? GetDynamoDbTableToGenerate(GeneratorAttributeSyntaxContext context, ClassType classType)
        {
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

            // 1. Get data for class

            (string? tableNameSuffix, string? indexName) = GetClassSettings(dynamoDbTableAttribute, classType);

            // 2. Get data for DynamoDB attributes

            var dynamoDbAttributes = GetClassProperties(typeSymbol);

            // Return an equatable value, to be cached

            var isParsable = true;
            var isFormattable = classType == ClassType.Table || classType == ClassType.Nested;
            var isProjectable = classType == ClassType.Projection;

            return new DynamoDbToGenerate(typeSymbol.ContainingNamespace.Name, typeSymbol.Name, tableNameSuffix, indexName, isParsable, isFormattable, isProjectable, dynamoDbAttributes); ;
        }
    }
}
