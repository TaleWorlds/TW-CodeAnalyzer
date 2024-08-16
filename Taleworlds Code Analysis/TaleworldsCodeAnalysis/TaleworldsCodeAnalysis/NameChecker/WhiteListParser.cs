using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TaleworldsCodeAnalysis.NameChecker
{
    public class WhiteListParser
    {
        public static WhiteListParser Instance 
        { 
            get 
            {
                if (_instance == null)
                {
                    _instance = new WhiteListParser ();
                }
                
                return _instance;
            } 
        }

        public IReadOnlyList<string> WhiteListWords => _whiteListedWords;
        private static WhiteListParser _instance;
        private IReadOnlyList<string> _whiteListedWords;

        public WhiteListParser()
        {
            
        }

        private void _readWhiteList(string whiteListString)
        {
            var document =XDocument.Parse(whiteListString);

            var xElements=document.Descendants("Word");
            
            List<string> words = new List<string>();

            foreach (var xElement in xElements)
            {
                words.Add(xElement.Value);
            }

            _whiteListedWords = new List<string> (words);
        }

        public void SymbolWhiteListChecker(SymbolAnalysisContext context)
        {
            ImmutableArray<AdditionalText> additionalFiles = context.Options.AdditionalFiles;
            AdditionalText whiteListFile = additionalFiles.FirstOrDefault(file => Path.GetFileName(file.Path).Equals("WhiteList.xml"));
            SourceText fileText = whiteListFile.GetText(context.CancellationToken);

            WhiteListParser.Instance._readWhiteList(fileText.ToString());
        }

        public void SyntaxWhiteListChecker(SyntaxNodeAnalysisContext context)
        {
            ImmutableArray<AdditionalText> additionalFiles = context.Options.AdditionalFiles;
            AdditionalText whiteListFile = additionalFiles.FirstOrDefault(file => Path.GetFileName(file.Path).Equals("WhiteList.xml"));
            SourceText fileText = whiteListFile.GetText(context.CancellationToken);

            WhiteListParser.Instance._readWhiteList(fileText.ToString());
        }

    }
}
