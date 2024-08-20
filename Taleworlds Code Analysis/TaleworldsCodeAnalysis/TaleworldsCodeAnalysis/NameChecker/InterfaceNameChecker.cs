using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "InterfaceNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.InterfaceNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.InterfaceNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.InterfaceNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(_analyzer, SymbolKind.NamedType);
        }

        private void _analyzer(SymbolAnalysisContext context)
        {
            WhiteListParser.Instance.UpdateWhiteList(context.Options.AdditionalFiles);

            var symbol = (INamedTypeSymbol)context.Symbol;
            if (symbol.TypeKind != TypeKind.Interface)
            {
                return;
            }
            var symbolName = symbol.Name;

            var properties = new Dictionary<string, string>
            {
                { "Name", symbolName },
            };

            if(!(symbolName.StartsWith("I") && NameCheckerLibrary.IsMatchingConvention(symbolName,ConventionType.IPascalCase)))
            {
                properties["NamingConvention"] = "IPascalCase";
                context.ReportDiagnostic(Diagnostic.Create(_rule, symbol.Locations[0], properties.ToImmutableDictionary(), symbolName));
            }

        }
    }
}
