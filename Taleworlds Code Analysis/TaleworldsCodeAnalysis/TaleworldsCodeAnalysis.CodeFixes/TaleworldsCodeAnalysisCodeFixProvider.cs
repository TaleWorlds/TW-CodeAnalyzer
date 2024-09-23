﻿using Microsoft.CodeAnalysis;
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
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TaleworldsCodeAnalysisCodeFixProvider)), Shared]
    public class TaleworldsCodeAnalysisCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                List<string> fixableDiagnosticIds = new List<string>();
                foreach (var item in FindAnalyzers.Instance.Analyzers)
                {
                    if (item.Category == DiagnosticCategories.Naming)
                    {
                        fixableDiagnosticIds.Add(item.Code);
                    }
                }
                return ImmutableArray.Create(fixableDiagnosticIds.ToArray());
            }
        }


        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }


        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            WhiteListParser.Instance.ReadGlobalWhiteListPath(context.Document.FilePath);
            var diagnostic = context.Diagnostics.First();

            var document = context.Document;
            var listOfWords = _getWordsToAddToWhitelist(document, diagnostic);
            if (listOfWords.Count != 0)
            {
                foreach (var item in listOfWords)
                {
                    context.RegisterCodeFix(CustomCodeAction.Create(title: "Add " + item + " to shared whitelist.",
                    createChangedSolution: (c, isPreview) => _addToWhitelistAsync(document, c, diagnostic, isPreview, item, WhiteListType.Shared),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle) + item + WhiteListType.Shared), diagnostic);
                }

                foreach (var item in listOfWords)
                {
                    context.RegisterCodeFix(CustomCodeAction.Create(title: "Add " + item + " to local whitelist.",
                    createChangedSolution: (c, isPreview) => _addToWhitelistAsync(document, c, diagnostic, isPreview, item, WhiteListType.Local),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle) + item + WhiteListType.Local), diagnostic);
                }
            }

            return Task.CompletedTask;
        }

        private async Task<Solution> _addToWhitelistAsync(Document document, CancellationToken cancellationToken, Diagnostic diagnostic, bool isPreview, string word, WhiteListType whiteListType)
        {
            if (!isPreview)
            {
                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                var path = whiteListType == WhiteListType.Shared ? WhiteListParser.Instance.SharedPathXml : WhiteListParser.Instance.LocalPathXml;
                var solution = document.Project.Solution;
                _addStringToWhiteList(path, word);
                ReAnalyze.Instance.ForceReanalyzeAsync();
            }

            return document.Project.Solution;
        }

        private IReadOnlyList<string> _getWordsToAddToWhitelist(Document document, Diagnostic diagnostic)
        {
            var diagnosticProperties = diagnostic.Properties;
            var identifier = diagnosticProperties["Name"];
            WhiteListParser.Instance.UpdateWhiteList();
            IReadOnlyList<string> words = new List<string>();

            if (diagnosticProperties.ContainsKey("NamingConvention"))
            {
                var convention = diagnosticProperties["NamingConvention"];
                var conventionEnum = (ConventionType)Enum.Parse(typeof(ConventionType), convention);
                words = _getNewWhiteListItemsToFix(identifier, conventionEnum);
            }

            return words;
        }

        private IReadOnlyList<string> _getNewWhiteListItemsToFix(string identifier, ConventionType conventionEnum)
        {
            IReadOnlyList<string> items = new List<string>();
            switch (conventionEnum)
            {
                case ConventionType.CamelCase:
                    items = CamelCaseBehaviour.Instance.FindWhiteListCandidates(identifier);
                    break;
                case ConventionType.UnderScoreCase:
                    items = UnderScoreCaseBehaviour.Instance.FindWhiteListCandidates(identifier);
                    break;
                case ConventionType.PascalCase:
                    items = PascalCaseBehaviour.Instance.FindWhiteListCandidates(identifier);
                    break;
                case ConventionType.IPascalCase:
                    items = IPascalCaseBehaviour.Instance.FindWhiteListCandidates(identifier);
                    break;
                case ConventionType.TPascalCase:
                    items = TPascalCaseBehaviour.Instance.FindWhiteListCandidates(identifier);
                    break;
            }
            return items;
        }

        private void _addStringToWhiteList(string filePath, string wordToAdd)
        {
            var doc = XDocument.Load(filePath);
            var root = doc.Element("WhiteListRoot");
            if (root != null)
            {
                var existingWord = root.Elements("Word").FirstOrDefault(e => e.Value.Equals(wordToAdd, StringComparison.OrdinalIgnoreCase));
                if (existingWord == null)
                {
                    root.Add(new XElement("Word", wordToAdd));
                }
                doc.Save(filePath);
            }

        }
    }
}
    
