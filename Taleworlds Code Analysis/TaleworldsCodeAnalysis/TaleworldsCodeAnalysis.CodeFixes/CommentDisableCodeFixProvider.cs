using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaleworldsCodeAnalysis.NameChecker;
using TaleworldsCodeAnalysis.Inheritance;
using TaleworldsCodeAnalysis.OtherCheckers;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CommentDisableCodeFixProvider)), Shared]
    public class CommentDisableCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                List<string> fixableDiagnosticIds = new List<string>();
                foreach (var item in FindAnalyzers.Instance.Analyzers)
                {
                    fixableDiagnosticIds.Add(item.Code);
                }
                return ImmutableArray.Create(fixableDiagnosticIds.ToArray());
            }
        }


        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }


        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            
            context.RegisterCodeFix(CustomCodeAction.Create("Disable spesific warning beginning from this line.",
                createChangedSolution: (c, isPreview) => _addDisablingCommentSpesific(c, isPreview, context)), diagnostic);

            context.RegisterCodeFix(CustomCodeAction.Create("Disable all warnings beginning from this line.",
                createChangedSolution: (c, isPreview) => _addDisablingCommentAll(c, isPreview, context)), diagnostic);

            context.RegisterCodeFix(CustomCodeAction.Create("Disable all warnings in this line.",
                createChangedSolution: (c, isPreview) => _addDisablingCommentAllOneLine(c, isPreview, context)), diagnostic);

            context.RegisterCodeFix(CustomCodeAction.Create("Disable specific warning in this line.",
                createChangedSolution: (c, isPreview) => _addDisablingCommentSpecificOneLine(c, isPreview, context)), diagnostic);
        }

        private async Task<Solution> _addCommentBeforeDiagnostic(CancellationToken c, CodeFixContext context, String comment)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the node at the location of the diagnostic
            var node = root.FindNode(diagnosticSpan);

            // Traverse upwards to find the first node with leading trivia
            SyntaxNode targetNode = node;
            while (targetNode != null && !targetNode.GetLeadingTrivia().Any())
            {
                targetNode = targetNode.Parent;
            }

            Solution changedSolution = context.Document.Project.Solution;

            if (targetNode != null)
            {
                var commentTrivia = SyntaxFactory.Comment(comment);

                var newLineTrivia = SyntaxFactory.CarriageReturnLineFeed;

                var leadingTrivia = targetNode.GetLeadingTrivia();
                var lastTrivia = leadingTrivia.LastOrDefault();
                leadingTrivia = leadingTrivia.Add(newLineTrivia).Add(commentTrivia).Add(newLineTrivia);

                if (lastTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    leadingTrivia = leadingTrivia.Add(lastTrivia);
                }

                var newTargetNode = targetNode.WithLeadingTrivia(leadingTrivia);
                var newRoot = root.ReplaceNode(targetNode, newTargetNode);

                var newDocument = context.Document.WithSyntaxRoot(newRoot);
                changedSolution = newDocument.Project.Solution;
                ReAnalyze.Instance.ForceReanalyze();
            }

            return changedSolution;
        }

        private async Task<Solution> _addDisablingCommentSpesific(CancellationToken c, bool isPreview, CodeFixContext context) 
        {
            return await _addCommentBeforeDiagnostic(c, context, "//TWCodeAnalysis disable " + context.Diagnostics.First().Id);
        }

        private async Task<Solution> _addDisablingCommentAll(CancellationToken c, bool isPreview, CodeFixContext context)
        {
            return await _addCommentBeforeDiagnostic(c, context, "//TWCodeAnalysis disable all");
        }

        private async Task<Solution> _addDisablingCommentAllOneLine(CancellationToken c, bool isPreview, CodeFixContext context)
        {
            return await _addCommentBeforeDiagnostic(c, context, "//TWCodeAnalysis disable next line all");
        }

        private async Task<Solution> _addDisablingCommentSpecificOneLine(CancellationToken c, bool isPreview, CodeFixContext context)
        {
            return await _addCommentBeforeDiagnostic(c, context, "//TWCodeAnalysis disable next line "+ context.Diagnostics.First().Id);
        }


    }
}
