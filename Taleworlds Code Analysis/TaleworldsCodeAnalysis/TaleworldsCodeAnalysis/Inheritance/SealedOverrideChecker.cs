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
    public class SealedOverrideChecker : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "TW2103";
        internal static readonly LocalizableString _title = "Overridden methods should be sealed.";
        internal static readonly LocalizableString _messageFormat = "Overridden methods should be sealed.";
        internal const string _category = "Inheritance";

        internal static DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.MethodDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context)) return;

            var method = (MethodDeclarationSyntax) context.Node;
            var modifiers = method.Modifiers;

            bool sealedFound = false;
            bool overrideFound = false;
            Location overrideLocation = null;

            foreach (var item in modifiers)
            {
                if (item.IsKind(SyntaxKind.SealedKeyword))
                {
                    sealedFound = true;
                }

                if(item.IsKind(SyntaxKind.OverrideKeyword))
                {
                    overrideFound = true;
                    overrideLocation=item.GetLocation();
                }
            }
            
            if(!sealedFound && overrideFound)
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, overrideLocation));
            }
            
        }
    }
}
