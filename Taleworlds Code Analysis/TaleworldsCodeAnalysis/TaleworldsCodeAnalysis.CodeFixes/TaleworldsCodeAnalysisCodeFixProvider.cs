using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            var diagnosticProperties = diagnostic.Properties;

            var document = context.Document;
            if (_getWordsToAddToWhitelist(document,diagnostic).Count== 0)
            {
                return;
            }

            context.RegisterCodeFix(CustomCodeAction.Create(title: CodeFixResources.CodeFixTitle,
                createChangedSolution: (c, isPreview) => _addToWhitelistAsync(document, c, diagnostic, isPreview), 
                equivalenceKey: nameof(CodeFixResources.CodeFixTitle)), diagnostic);


        }

        private async Task<Solution> _addToWhitelistAsync(Document document, CancellationToken cancellationToken,Diagnostic diagnostic, bool isPreview)
        {
            if (isPreview)
            {
                return document.Project.Solution;
            }
            IReadOnlyList<string> words = _getWordsToAddToWhitelist(document, diagnostic);
            var path = _getPathOfXml(document.Project.AdditionalDocuments);

            AddStringToWhiteList(path, words);

            var originalSolution = document.Project.Solution;
            return originalSolution;
        }

        private IReadOnlyList<string> _getWordsToAddToWhitelist(Document document, Diagnostic diagnostic)
        {
            var additionalFiles = document.Project.AdditionalDocuments;
            var diagnosticProperties = diagnostic.Properties;
            var identifier = diagnosticProperties["Name"];

            XDocument doc = XDocument.Load(_getPathOfXml(additionalFiles));
            
            WhiteListParser.Instance.InitializeWhiteListParser(doc.ToString());
            IReadOnlyList<string> words = new List<string>();

            if (diagnosticProperties.ContainsKey("NamingConvention"))
            {
                var convention = diagnosticProperties["NamingConvention"];
                var conventionEnum = (ConventionType)Enum.Parse(typeof(ConventionType), convention);
                words = NameCheckerLibrary.GetForbiddenPieces(identifier, conventionEnum);
            }

            return words;
        }

        private string _getPathOfXml(IEnumerable<TextDocument> additionalFiles)
        {
            string path = "";
            if (additionalFiles.Count() != 0)
            {
                var externalFile = additionalFiles.FirstOrDefault(file => Path.GetFileName(file.FilePath).Equals("WhiteList.xml", StringComparison.OrdinalIgnoreCase));
                path = externalFile.FilePath;
            }
            else
            {
                path = WhiteListParser.Instance.TestPathXml;
            }
            return path;
        }

        private void AddStringToWhiteList(string filePath, IReadOnlyList<string> wordsToAdd)
        {
            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Element("WhiteListRoot");
                if (root != null)
                {
                    foreach (var word in wordsToAdd)
                    {
                        var existingWord = root.Elements("Word").FirstOrDefault(e => e.Value.Equals(word, StringComparison.OrdinalIgnoreCase));
                        if (existingWord == null)
                        {
                            root.Add(new XElement("Word", word));
                        }
                    }
                    doc.Save(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

    }
}
