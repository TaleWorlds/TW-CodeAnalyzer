using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace TaleworldsCodeAnalysis.Inheritance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DepthOfInheritanceChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "TW2101";
        private static readonly LocalizableString _title = "Depth of inheritance should be 2 at maximum";
        private static readonly LocalizableString _messageFormat = "Depth of inheritance should be 2 at maximum";
        private const string _category = "Inheritance";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.ClassDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId)) return;

            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
            var semanticModel = context.SemanticModel;
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);

            var currentBaseType = classSymbol.BaseType;
            int depth = 0;
            while (currentBaseType != null && currentBaseType.SpecialType!=SpecialType.System_Object)
            {
                depth++;
                currentBaseType = currentBaseType.BaseType;
            }

            if (depth>1)
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, classDeclarationSyntax.Identifier.GetLocation()));
            }
        }
    }
}
