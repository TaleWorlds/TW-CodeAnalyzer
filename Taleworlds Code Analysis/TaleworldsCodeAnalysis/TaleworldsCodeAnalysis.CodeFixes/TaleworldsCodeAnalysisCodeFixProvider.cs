using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
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

namespace TaleworldsCodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TaleworldsCodeAnalysisCodeFixProvider)), Shared]
    public class TaleworldsCodeAnalysisCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                List<string> fixableDiagnosticIds = new List<string> {ClassNameChecker.ModifierDiagnosticId, ClassNameChecker.NameDiagnosticId, FieldNameChecker.DiagnosticId, InterfaceNameChecker.DiagnosticId, LocalNameChecker.DiagnosticId, MethodNameChecker.DiagnosticId, ParameterNameChecker.DiagnosticId, PropertyNameChecker.DiagnosticId, TemplateParameterNameChecker.DiagnosticId };
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

            var node = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedSolution: c => _addToWhitelistAsync(context.Document, node, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);

        }

        private async Task<Solution> _addToWhitelistAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var identifier = GetIdentifier(node);

            var additionalFiles = document.Project.AdditionalDocuments;
            var externalFile = additionalFiles.FirstOrDefault(file => Path.GetFileName(file.FilePath).Equals("WhiteList.xml", StringComparison.OrdinalIgnoreCase));
            await AddStringToWhiteListAsync(externalFile.FilePath, identifier);
            

            var originalSolution = document.Project.Solution;
            return originalSolution;
        }

        private async Task AddStringToWhiteListAsync(string filePath, string word)
        {
            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Element("WhiteListRoot");
                if (root != null)
                {
                    // Check if the word already exists
                    var existingWord = root.Elements("Word").FirstOrDefault(e => e.Value.Equals(word, StringComparison.OrdinalIgnoreCase));
                    if (existingWord == null)
                    {
                        root.Add(new XElement("Word", word));
                        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                        {
                            using (var writer = new StreamWriter(stream))
                            {
                                await writer.WriteAsync(doc.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
            }
        }

        private string GetIdentifier(SyntaxNode node)
        {
            switch (node)
            {
                case ClassDeclarationSyntax classDecl:
                    return classDecl.Identifier.Text;
                case MethodDeclarationSyntax methodDecl:
                    return methodDecl.Identifier.Text;
                case PropertyDeclarationSyntax propertyDecl:
                    return propertyDecl.Identifier.Text;
                case FieldDeclarationSyntax fieldDecl:
                    return fieldDecl.Declaration.Variables.First().Identifier.Text;
                case VariableDeclaratorSyntax variableDecl:
                    return variableDecl.Identifier.Text;
                case ParameterSyntax parameterDecl:
                    return parameterDecl.Identifier.Text;
                // Add more cases as needed
                default:
                    return null;
            }
        }
    }
}
