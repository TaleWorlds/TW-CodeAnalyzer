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
        private const string testPathXML = "C:\\develop\\TW-CodeAnalyzer\\Taleworlds Code Analysis\\TaleworldsCodeAnalysis\\WhiteList.xml";

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
            WhiteListParser.Instance._readWhiteList(_getFileText(additionalFiles));
        }

        public void SyntaxWhiteListChecker(SyntaxNodeAnalysisContext context)
        {
            ImmutableArray<AdditionalText> additionalFiles = context.Options.AdditionalFiles;
            WhiteListParser.Instance._readWhiteList(_getFileText(additionalFiles));
        }

        private string _getFileText(ImmutableArray<AdditionalText> additionalFiles)
        {
            string fileText = "";
            if (additionalFiles.Length != 0)
            {
                AdditionalText whiteListFile = additionalFiles.FirstOrDefault(file => Path.GetFileName(file.Path).Equals("WhiteList.xml"));
                SourceText fileSourceText = whiteListFile.GetText();
                fileText = fileSourceText.ToString();
            }
            else
            {
                XDocument document = XDocument.Load(testPathXML);
                fileText = document.ToString();
            }
            return fileText;
        }


    }
}
