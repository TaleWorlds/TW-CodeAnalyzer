using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.OtherCheckers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentsChecker : DiagnosticAnalyzer
    {

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        "SingleLineCommentAnalyzer",
        "Single line comment found",
        "Comment: {0}",
        "Commenting",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            var singleLineComments = root.DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia));

            foreach (var comment in singleLineComments)
            {
                if (comment.ToString().ToLower() == "//twcodeanalysis off")
                {
                    var diagnostic = Diagnostic.Create(Rule, comment.GetLocation(), comment.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
                else if (comment.ToString().ToLower() == "//twcodeanalysis on")
                {
                    var diagnostic = Diagnostic.Create(Rule, comment.GetLocation(), comment.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

    }
}
