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
    public class VarKeywordChecker : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "TW2204";
        internal static readonly LocalizableString Title = "You can use var instead of {0}";
        internal static readonly LocalizableString MessageFormat = "You can use var instead of {0}.";
        internal const string Category = "LocalType";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.LocalDeclarationStatement);

        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId)) return;

            var localDec = (LocalDeclarationStatementSyntax) context.Node;

            if(!localDec.Declaration.Type.IsVar)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, localDec.Declaration.Type.GetLocation(),localDec.Declaration.Type));
            }
        }
    }
}
