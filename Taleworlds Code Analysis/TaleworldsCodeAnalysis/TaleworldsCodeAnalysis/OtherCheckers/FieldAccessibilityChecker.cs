using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.OtherCheckers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FieldAccessibilityChecker : DiagnosticAnalyzer
    {
        public static string AccesabilityDiagnosticId => _accessibilityDiagnosticId;
        private const string _accessibilityDiagnosticId = "FieldAccessibilityChecker";
        private static readonly LocalizableString _accessibilityTitle = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _accessibilityMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _accessibilityDescription = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _accessibilityCategory = "Accessibility";

        private static readonly DiagnosticDescriptor _accessibilityRule = new DiagnosticDescriptor(_accessibilityDiagnosticId, _accessibilityTitle, _accessibilityMessageFormat, _accessibilityCategory, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _accessibilityDescription);



        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_accessibilityRule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(_analyzer, SymbolKind.Field);
        }

        private void _analyzer(SymbolAnalysisContext context)
        {
            if (BlackListedProjects.Instance.isBlackListedProjectFromCodePath(context.Symbol.Locations[0].ToString())) return;

            WhiteListParser.Instance.UpdateWhiteList(context.Options.AdditionalFiles);

            var field = (IFieldSymbol)context.Symbol;

            if (field.ContainingType.TypeKind == TypeKind.Enum)
            {
                return;
            }

            var properties = new Dictionary<string, string>
            {
                { "Name", field.Name },
            };

            if (field.DeclaredAccessibility != Accessibility.Private)
            {
                var diagnostic = Diagnostic.Create(_accessibilityRule, field.Locations[0], properties.ToImmutableDictionary(), field.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
