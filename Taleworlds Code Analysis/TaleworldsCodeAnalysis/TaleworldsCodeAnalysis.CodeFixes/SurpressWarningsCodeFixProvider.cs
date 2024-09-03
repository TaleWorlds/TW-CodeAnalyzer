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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SurpressWarningsCodeFixProvider)), Shared]
    public class SurpressWarningsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                List<string> fixableDiagnosticIds = new List<string> { ClassNameChecker.DiagnosticId, FieldNameChecker.DiagnosticId, InterfaceNameChecker.DiagnosticId, LocalNameChecker.DiagnosticId, MethodNameChecker.DiagnosticId, ParameterNameChecker.DiagnosticId, PropertyNameChecker.DiagnosticId, TemplateParameterNameChecker.DiagnosticId, ClassAccessibilityChecker.DiagnosticId, FieldAccessibilityChecker.DiagnosticId, AbstractClassChecker.DiagnosticId, DepthOfInheritanceChecker.DiagnosticId, SealedOverrideChecker.DiagnosticId};
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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the node at the location of the diagnostic
            var node = root.FindNode(diagnosticSpan);

            // Traverse up the syntax tree to find the containing class declaration
            var classDeclaration = node.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if(classDeclaration != null) {
                context.RegisterCodeFix(CustomCodeAction.Create("Surpress warnings for class " + classDeclaration.GetText(),
                    createChangedSolution: (c, isPreview) => _surpressWarningsForClass(c, isPreview, context.Document, classDeclaration, root)), diagnostic);
            }

            

        }

        private async Task<Solution> _surpressWarningsForClass(CancellationToken c, bool isPreview, Document document, ClassDeclarationSyntax classDeclaration, SyntaxNode root) 
        {
            var attributeList = SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("SuppressWarnings"))));

            // Add the attribute list to the class declaration
            var newClassDeclaration = classDeclaration.AddAttributeLists(attributeList);

            // Replace the old class declaration with the new one
            var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);

            // Apply the changes to the document
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument.Project.Solution;
        }



    }
}
