using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClassNameChecker : DiagnosticAnalyzer
    {
        public string NameDiagnosticId =>_nameDiagnosticId;
        public string ModifierDiagnosticId=>_modifierDiagnosticId;

        private const string _nameDiagnosticId = "ClassNameChecker";
        private const string _modifierDiagnosticId = "ClassModifierChecker";

        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.ClassNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.ClassNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.ClassNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _modifierMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.ClassModifierCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));

        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _nameRule = new DiagnosticDescriptor(_nameDiagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);
        private static readonly DiagnosticDescriptor _modifierRule = new DiagnosticDescriptor(_modifierDiagnosticId, _title, _modifierMessageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_nameRule, _modifierRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            

            context.RegisterSymbolAction(_analyzeMethod, SymbolKind.NamedType);
        }

        private void _analyzeMethod(SymbolAnalysisContext context)
        {
            WhiteListParser.Instance.SymbolWhiteListChecker(context);

            var symbol = (INamedTypeSymbol)context.Symbol;

            if (symbol.TypeKind != TypeKind.Class)
            {
                return;
            }

            if (symbol.DeclaredAccessibility == Accessibility.Private ||
                symbol.DeclaredAccessibility == Accessibility.Internal)
            {
                if (!NameCheckerLibrary.IsMatchingConvention(symbol.Name,ConventionType._uscoreCase))
                {
                    context.ReportDiagnostic(Diagnostic.Create(_nameRule, symbol.Locations[0], symbol.Name, symbol.DeclaredAccessibility.ToString(), "_uscoreCase"));
                }
            }
            else if (symbol.DeclaredAccessibility == Accessibility.Public)
            {
                if (!NameCheckerLibrary.IsMatchingConvention(symbol.Name, ConventionType.PascalCase))
                {
                    context.ReportDiagnostic(Diagnostic.Create(_nameRule, symbol.Locations[0], symbol.Name, symbol.DeclaredAccessibility.ToString(), "PascalCase"));
                }
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(_modifierRule, symbol.Locations[0], symbol.Name));
            }
        }
    }
}