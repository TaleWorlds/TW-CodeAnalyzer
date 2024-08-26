using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClassNameChecker : DiagnosticAnalyzer
    {
        public static string NameDiagnosticId =>_nameDiagnosticId;
        public static string ModifierDiagnosticId=>_modifierDiagnosticId;

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
            context.RegisterSymbolAction(_analyzer, SymbolKind.NamedType);
        }

        private void _analyzer(SymbolAnalysisContext context)
        {
            WhiteListParser.Instance.ReadGlobalWhiteListPath(context.Symbol.Locations[0].SourceTree.FilePath);
            WhiteListParser.Instance.UpdateWhiteList();

            var symbol = (INamedTypeSymbol)context.Symbol;

            if (symbol.TypeKind != TypeKind.Class)
            {
                return;
            }

            var properties = new Dictionary<string, string>
            {
                { "Name", symbol.Name },
            };

            if (symbol.DeclaredAccessibility == Accessibility.Private ||
                symbol.DeclaredAccessibility == Accessibility.Internal)
            {
                if (!UnderScoreCaseBehaviour.Instance.IsMatching(symbol.Name))
                {
                    properties["NamingConvention"] = "_uscoreCase";
                    context.ReportDiagnostic(Diagnostic.Create(_nameRule, symbol.Locations[0], properties.ToImmutableDictionary(), symbol.Name,
                        UnderScoreCaseBehaviour.Instance.FixThis(symbol.Name)));
                }
            }
            else if (symbol.DeclaredAccessibility == Accessibility.Public)
            {
                if (!PascalCaseBehaviour.Instance.IsMatching(symbol.Name))
                {
                    properties["NamingConvention"] = "PascalCase";
                    context.ReportDiagnostic(Diagnostic.Create(_nameRule, symbol.Locations[0], properties.ToImmutableDictionary(), symbol.Name,
                        PascalCaseBehaviour.Instance.FixThis(symbol.Name)));
                }
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(_modifierRule, symbol.Locations[0], symbol.Name));
            }
        }
    }
}