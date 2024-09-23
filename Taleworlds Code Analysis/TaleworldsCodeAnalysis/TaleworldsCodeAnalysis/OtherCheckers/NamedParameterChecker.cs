﻿using Microsoft.CodeAnalysis;
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
    [TaleworldsAnalyzer("Named Parameter Checker", _diagnosticId, title: "Other Checkers")]
    public class NamedParameterChecker : DiagnosticAnalyzer
    {
        public string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = nameof(DiagnosticIDs.TW2201);
        private static readonly LocalizableString _title = 
            new LocalizableResourceString(nameof(OtherCheckerResource.NamedParameterCheckerTitle),OtherCheckerResource.ResourceManager,typeof(OtherCheckerResource)).ToString()+_argumentThreshold+".";
        private static readonly LocalizableString _messageFormat =
            new LocalizableResourceString(nameof(OtherCheckerResource.NamedParameterCheckerTitle), OtherCheckerResource.ResourceManager, typeof(OtherCheckerResource)).ToString() + _argumentThreshold + ".";
        private const DiagnosticCategories _category = DiagnosticCategories.Parameter;

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), DiagnosticSeverity.Warning, true);

        private const int _argumentThreshold = 3;
        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public sealed override void Initialize(AnalysisContext context)
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
                    var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                    _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), severity, isEnabledByDefault: true);
                    context.ReportDiagnostic(Diagnostic.Create(_rule, invocationExpr.GetLocation()));
                }
            }
        }
    }
}
