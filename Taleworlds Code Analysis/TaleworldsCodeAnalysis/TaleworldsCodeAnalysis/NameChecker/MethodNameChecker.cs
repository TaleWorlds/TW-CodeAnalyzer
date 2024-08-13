using Microsoft.CodeAnalysis;
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
        public const string DiagnosticId = "MethodNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.MethodNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.MethodNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.MethodNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);


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

           if (method.DeclaredAccessibility == Accessibility.Private || 
                method.DeclaredAccessibility == Accessibility.Internal)
           {
                if (!NameCheckerLibrary.IsUnderScoreCase(method.Name))
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, method.Locations[0], method.Name, method.DeclaredAccessibility.ToString(), "_uscoreCase"));
                }
           }
           else
           {
                 if(!NameCheckerLibrary.IsPascalCase(method.Name))
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, method.Locations[0], method.Name, method.DeclaredAccessibility.ToString(), "PascalCase"));
                }
           }
        }
    }
}
