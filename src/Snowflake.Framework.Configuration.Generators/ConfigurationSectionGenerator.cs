﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snowflake.Configuration.Generators
{
    [Generator]
    public sealed class ConfigurationSectionGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ConfigurationTemplateInterfaceSyntaxReceiver receiver)
                return;
            bool errorOccured = false;
            var compilation = context.Compilation;
            CSharpParseOptions options = (compilation as CSharpCompilation).SyntaxTrees[0].Options as CSharpParseOptions;
            INamedTypeSymbol configSectionAttr = compilation.GetTypeByMetadataName("Snowflake.Configuration.Attributes.ConfigurationSectionAttribute");
            INamedTypeSymbol configOptionAttr = compilation.GetTypeByMetadataName("Snowflake.Configuration.Attributes.ConfigurationOptionAttribute");

            INamedTypeSymbol configSectionInterface = compilation.GetTypeByMetadataName("Snowflake.Configuration.IConfigurationSection");
            INamedTypeSymbol configSectionGenericInterface = compilation.GetTypeByMetadataName("Snowflake.Configuration.IConfigurationSection`1");
            INamedTypeSymbol configInstanceAttr = compilation.GetTypeByMetadataName("Snowflake.Configuration.Generators.ConfigurationGenerationInstanceAttribute");
            List<IPropertySymbol> symbols = new();

            foreach (var iface in receiver.CandidateInterfaces)
            {
                var model = compilation.GetSemanticModel(iface.SyntaxTree);
                var ifaceSymbol = model.GetDeclaredSymbol(iface);
                var memberSyntax = iface.Members;

                if (memberSyntax.FirstOrDefault(m => m is not PropertyDeclarationSyntax) is MemberDeclarationSyntax badSyntax)
                {
                    var badSymbol = model.GetDeclaredSymbol(badSyntax);
                    context.ReportError(1004, "Invalid members in template interface.",
                        $"Template interface '{ifaceSymbol.Name}' must only declare property members. " +
                        $"{badSymbol.Kind} '{ifaceSymbol.Name}.{badSymbol?.Name}' is not a property.",
                        badSyntax.GetLocation(), ref errorOccured);
                    continue;
                }

                if (!iface.Modifiers.Any(p => p.IsKind(SyntaxKind.PartialKeyword)))
                {
                    context.ReportError(1001,
                               "Unextendible template interface",
                               $"Template interface '{ifaceSymbol.Name}' must be marked partial.",
                               iface.GetLocation(), ref errorOccured);
                    continue;
                }

                foreach (var prop in memberSyntax.Cast<PropertyDeclarationSyntax>())
                {
                    var propSymbol = model.GetDeclaredSymbol(prop);
                    if (!propSymbol.ContainingType.GetAttributes()
                        .Any(attr => attr.AttributeClass.Equals(configSectionAttr, SymbolEqualityComparer.Default))
                        && propSymbol.ContainingType.TypeKind != TypeKind.Interface)
                    {
                        continue;
                    }

                    var attrs = propSymbol.GetAttributes().Where(attr => attr.AttributeClass.Equals(configOptionAttr, SymbolEqualityComparer.Default));
                    if (!attrs.Any())
                    {
                        context.ReportError(1013, "Undecorated section property member",
                                   $"Property {propSymbol.Name} must be decorated with a ConfigurationOptionAttribute.",
                               prop.GetLocation(), ref errorOccured);
                        continue;
                    }

                    if (attrs.First().ConstructorArguments.Length == 1)
                    {
                        // todo: check if GUID
                    }

                    var defaultValue = attrs.First().ConstructorArguments.Skip(1).First();
                    if (!SymbolEqualityComparer.Default.Equals(defaultValue.Type, propSymbol.Type))
                    {
                        context.ReportError(1014, "Mismatched default value type",
                                   $"Property {propSymbol.Name} is of type '{propSymbol.Type}' but has default value of type '{defaultValue.Type}'.",
                               prop.GetLocation(), ref errorOccured);
                        continue;
                    }

                    symbols.Add(propSymbol);
                }
            }

            if (errorOccured)
                return;

            foreach (IGrouping<INamedTypeSymbol, IPropertySymbol> group in symbols.GroupBy(f => f.ContainingType))
            {
                string classSource = ProcessClass(group.Key, group.ToList(), configSectionInterface, configSectionGenericInterface, configInstanceAttr, context);
                context.AddSource($"{group.Key.Name}_ConfigurationSection.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        public string ProcessClass(INamedTypeSymbol classSymbol, List<IPropertySymbol> props,

            INamedTypeSymbol configSectionInterface,
            INamedTypeSymbol configSectionGenericInterface,
            INamedTypeSymbol configInstanceAttr,
            GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            string generatedNamespaceName = $"Snowflake.Configuration.GeneratedConfigurationProxies.Section_{namespaceName}";

            string tag = RandomString(6);
            string backingClassName = $"{classSymbol.Name}Proxy_{tag}";
            StringBuilder source = new StringBuilder($@"
namespace {namespaceName}
{{
    [{configInstanceAttr.ToDisplayString()}(typeof({generatedNamespaceName}.{backingClassName}))]
    public partial interface {classSymbol.Name}
    {{
    
    }}

}}

namespace {generatedNamespaceName}
{{
    using System.ComponentModel;
    [EditorBrowsable(EditorBrowsableState.Never)]
    sealed class {backingClassName} : {classSymbol.ToDisplayString()}
    {{
        readonly Snowflake.Configuration.IConfigurationSectionDescriptor __sectionDescriptor;
        readonly Snowflake.Configuration.IConfigurationValueCollection __backingCollection;

        private {backingClassName}(Snowflake.Configuration.IConfigurationSectionDescriptor sectionDescriptor, Snowflake.Configuration.IConfigurationValueCollection collection) 
        {{
            this.__sectionDescriptor = sectionDescriptor;
            this.__backingCollection = collection;
        }}
");

            foreach (var prop in props)
            {
                source.Append($@"
{prop.Type.ToDisplayString()} {classSymbol.ToDisplayString()}.{prop.Name}
{{
    get {{ return ({prop.Type.ToDisplayString()})this.__backingCollection[this.__sectionDescriptor, nameof({prop.ToDisplayString()})]?.Value; }}
    set {{ 
            var existingValue = this.__backingCollection[this.__sectionDescriptor, nameof({prop.ToDisplayString()})];
            if (existingValue != null && value != null) {{ existingValue.Value = value; }}
            if (existingValue != null && value == null && this.__sectionDescriptor[nameof({prop.ToDisplayString()})].Type == typeof(string)) 
            {{ existingValue.Value = this.__sectionDescriptor[nameof({prop.ToDisplayString()})].Unset; }}
        }}
}}
");
            }


            source.Append("}}");
            return source.ToString();
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
#endif 
            context.RegisterForSyntaxNotifications(() => new ConfigurationTemplateInterfaceSyntaxReceiver("ConfigurationSection"));
        }
    }
}
