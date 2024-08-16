using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

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

            context.RegisterSymbolAction(_analyzeMethod, SymbolKind.Method);
        }

        private void _analyzeMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol) context.Symbol;
            WhiteListParser.Instance.SymbolWhiteListChecker(context);

            if(method.MethodKind == MethodKind.PropertyGet || MethodKind.PropertySet == method.MethodKind || MethodKind.Constructor==method.MethodKind)
            {
                return;
            }

            if (method.DeclaredAccessibility == Accessibility.Private || 
                method.DeclaredAccessibility == Accessibility.Internal)
            {
                if (!NameCheckerLibrary.IsMatchingConvention(method.Name, ConventionType._uscoreCase))
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, method.Locations[0], method.Name, method.DeclaredAccessibility.ToString(), "_uscoreCase"));
                }
            }
            else
            {
                    if(!NameCheckerLibrary.IsMatchingConvention(method.Name, ConventionType.PascalCase))
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, method.Locations[0], method.Name, method.DeclaredAccessibility.ToString(), "PascalCase"));
                }
            }
        }
    }
}
