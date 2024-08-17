using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId =>_diagnosticId;

        private const string _diagnosticId = "PropertyNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.PropertyNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.PropertyNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.PropertyNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(_analyzeMethod, SymbolKind.Property);
        }

        private void _analyzeMethod(SymbolAnalysisContext context)
        {
            var property = (IPropertySymbol)context.Symbol;
            WhiteListParser.Instance.SymbolWhiteListChecker(context);

            var properties = new Dictionary<string, string>
            {
                { "Name", property.Name },
            };

            if (property.DeclaredAccessibility == Accessibility.Private ||
                 property.DeclaredAccessibility == Accessibility.Internal)
            {
                if (!NameCheckerLibrary.IsMatchingConvention(property.Name, ConventionType._uscoreCase))
                {
                    properties["NamingConvention"] = "_uscoreCase";
                    context.ReportDiagnostic(Diagnostic.Create(_rule, property.Locations[0], properties.ToImmutableDictionary(), property.Name, property.DeclaredAccessibility.ToString(), "_uscoreCase"));
                }
            }
            else
            {
                if (!NameCheckerLibrary.IsMatchingConvention(property.Name, ConventionType.PascalCase))
                {
                    properties["NamingConvention"] = "PascalCase";
                    context.ReportDiagnostic(Diagnostic.Create(_rule, property.Locations[0], properties.ToImmutableDictionary(), property.Name, property.DeclaredAccessibility.ToString(), "PascalCase"));
                }
            }
        }
    }
}
