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
                _instance = new WhiteListParser();
                return _instance;
            } 
        }

        public IReadOnlyList<string> WhiteListWords => _whiteListedWords;
        private static readonly LocalizableString whiteList = new LocalizableResourceString(nameof(NameCheckerResources.WhiteList), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static WhiteListParser _instance;
        private IReadOnlyList<string> _whiteListedWords;

        public WhiteListParser()
        {
            _readWhiteList();
        }

        private void _readWhiteList()
        {
            var document =XDocument.Parse(whiteList.ToString());

            var xElements=document.Descendants("Word");
            
            List<string> words = new List<string>();

            foreach (var xElement in xElements)
            {
                words.Add(xElement.Value);
            }

            _whiteListedWords = new List<string> (words);
        }

    }
}
