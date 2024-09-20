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
    [TaleworldsAnalyzer("Sealed Override Checker", _diagnosticId,title:"Inheritance Checker")]
    public class SealedOverrideChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = nameof(DiagnosticIDs.TW2102);
        private static readonly LocalizableString _title = 
            new LocalizableResourceString(nameof(InheritanceResources.SealedOverrideCheckerTitle),InheritanceResources.ResourceManager,typeof(InheritanceResources));
        private static readonly LocalizableString _messageFormat =
            new LocalizableResourceString(nameof(InheritanceResources.SealedOverrideCheckerTitle), InheritanceResources.ResourceManager, typeof(InheritanceResources));
        private const string _category = nameof(DiagnosticCategories.Inheritance);

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule); 


        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.MethodDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var method = (MethodDeclarationSyntax)context.Node;
                var modifiers = method.Modifiers;

                var sealedFound = false;
                var overrideFound = false;
                Location overrideLocation = null;

                foreach (var item in modifiers)
                {
                    if (item.IsKind(SyntaxKind.SealedKeyword))
                    {
                        sealedFound = true;
                    }

                    if (item.IsKind(SyntaxKind.OverrideKeyword))
                    {
                        overrideFound = true;
                        overrideLocation = item.GetLocation();
                    }
                }

                if (!sealedFound && overrideFound)
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, overrideLocation));
                }
            }
        }
    }
}
