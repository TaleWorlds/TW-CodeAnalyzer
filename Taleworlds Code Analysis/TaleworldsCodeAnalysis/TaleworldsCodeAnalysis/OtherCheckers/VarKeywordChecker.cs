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
        public string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "TW2204";
        private static readonly LocalizableString _title = "You can use var instead of {0}";
        private static readonly LocalizableString _messageFormat = "You can use var instead of {0}";
        private const string _category = "LocalType";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Warning, true);

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public sealed override void Initialize(AnalysisContext context)
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
                var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, severity, isEnabledByDefault: true);
                context.ReportDiagnostic(Diagnostic.Create(_rule, localDec.Declaration.Type.GetLocation(),localDec.Declaration.Type));
            }
        }
    }
}
