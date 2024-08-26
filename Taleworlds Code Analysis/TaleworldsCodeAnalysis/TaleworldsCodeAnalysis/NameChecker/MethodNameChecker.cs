using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "MethodNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.MethodNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.MethodNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.MethodNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(_analyzer, SymbolKind.Method);
        }

        private void _analyzer(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol) context.Symbol;

            WhiteListParser.Instance.ReadGlobalWhiteListPath(context.Symbol.Locations[0].SourceTree.FilePath);
            WhiteListParser.Instance.UpdateWhiteList();

            if (method.MethodKind == MethodKind.PropertyGet || MethodKind.PropertySet == method.MethodKind || MethodKind.Constructor==method.MethodKind)
            {
                return;
            }

            var properties = new Dictionary<string, string>
            {
                { "Name", method.Name },
            };

            if (method.DeclaredAccessibility == Accessibility.Private || 
                method.DeclaredAccessibility == Accessibility.Internal)
            {
                if (!UnderScoreCaseBehaviour.Instance.IsMatching(method.Name))
                {
                    properties["NamingConvention"] = "_uscoreCase";
                    context.ReportDiagnostic(Diagnostic.Create(_rule, method.Locations[0], properties.ToImmutableDictionary(), method.Name, 
                        UnderScoreCaseBehaviour.Instance.FixThis(method.Name)));
                }
            }
            else
            {
                if(!PascalCaseBehaviour.Instance.IsMatching(method.Name))
                {
                    properties["NamingConvention"] = "PascalCase";
                    context.ReportDiagnostic(Diagnostic.Create(_rule, method.Locations[0], properties.ToImmutableDictionary(), method.Name,
                        PascalCaseBehaviour.Instance.FixThis(method.Name)));
                }
            }
        }
    }
}
