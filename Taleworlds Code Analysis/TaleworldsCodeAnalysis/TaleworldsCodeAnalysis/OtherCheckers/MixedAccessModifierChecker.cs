using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace TaleworldsCodeAnalysis.OtherCheckers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MixedAccessModifierChecker : DiagnosticAnalyzer
    {
        public string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "TW2202";
        private static readonly LocalizableString _title = "MixedAccessModifierChecker Title";
        private static readonly LocalizableString _messageFormat = "MixedAccessModifierChecker '{0}'";
        private const string _category = "MixedAccessModifierChecker Category";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Warning, true);

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_methodAnalyzer, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(_propertyAnalyzer, SyntaxKind.PropertyDeclaration);
        }

        private void _methodAnalyzer(SyntaxNodeAnalysisContext context)
        {
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId)) return;

            var methodNode = (MethodDeclarationSyntax)context.Node;
            _checkMixedAccessibility(methodNode.Modifiers,context);
            
        }

        private void _checkMixedAccessibility(SyntaxTokenList modifiers, SyntaxNodeAnalysisContext context)
        {
            bool accesibilityFound = false;
            foreach (var item in modifiers)
            {
                if (item.IsKind(SyntaxKind.PrivateKeyword) ||
                    item.IsKind(SyntaxKind.PublicKeyword) ||
                    item.IsKind(SyntaxKind.ProtectedKeyword) ||
                    item.IsKind(SyntaxKind.InternalKeyword))
                {
                    if (accesibilityFound)
                    {
                        var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                        _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, severity, isEnabledByDefault: true);
                        context.ReportDiagnostic(Diagnostic.Create(_rule, item.GetLocation()));
                        return;
                    }
                    accesibilityFound = true;
                }
            }
        }

        private void _propertyAnalyzer(SyntaxNodeAnalysisContext context)
        {
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId)) return;

            var propertyNode = (PropertyDeclarationSyntax)context.Node;
            _checkMixedAccessibility(propertyNode.Modifiers, context);
        }
    }
}
