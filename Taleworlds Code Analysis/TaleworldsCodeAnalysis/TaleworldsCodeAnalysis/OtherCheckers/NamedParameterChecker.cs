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
    public class NamedParameterChecker : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "TW2201";
        internal static readonly LocalizableString Title = "You need to call method with its parameter names " +
            "if it has more than "+argumentThreshold+".";
        internal static readonly LocalizableString MessageFormat = "You need to call method with its parameter names " +
            "if it has more than "+argumentThreshold+".";
        internal const string Category = "Parameter";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        private const int argumentThreshold = 3;
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.InvocationExpression);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;
            var argumentList = invocationExpr.ArgumentList;

            if (argumentList == null || argumentList.Arguments.Count<3)
            {
                return;
            }

            foreach (var item in argumentList.Arguments)
            {
                if (item.NameColon==null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, invocationExpr.GetLocation()));
                }
            }
        }
    }
}
