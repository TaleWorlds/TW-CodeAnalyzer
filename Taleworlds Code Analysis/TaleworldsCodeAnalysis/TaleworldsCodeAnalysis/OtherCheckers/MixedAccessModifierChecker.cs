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
        public const string DiagnosticId = "TW2202";
        internal static readonly LocalizableString Title = "MixedAccessModifierChecker Title";
        internal static readonly LocalizableString MessageFormat = "MixedAccessModifierChecker '{0}'";
        internal const string Category = "MixedAccessModifierChecker Category";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
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
                        context.ReportDiagnostic(Diagnostic.Create(Rule, item.GetLocation()));
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
