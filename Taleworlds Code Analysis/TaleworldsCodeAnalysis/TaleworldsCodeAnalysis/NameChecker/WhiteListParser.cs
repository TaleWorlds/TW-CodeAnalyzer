using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Microsoft.Build.Tasks;

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
                    _instance = new WhiteListParser();
                }
                
                return _instance;
            } 
        }
        public string SharedPathXml => _sharedWhiteListPath;

        public IReadOnlyList<string> WhiteListWords => _whiteListedWords;
        private static WhiteListParser _instance;
        private IReadOnlyList<string> _whiteListedWords;
        private string _sharedWhiteListPath;

        private WhiteListParser(){}

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

        public void UpdateWhiteList()
        {
            _readWhiteList(_getFileText());
        }


        public void InitializeWhiteListParser(string sourceText)
        {
            _readWhiteList(sourceText);
        }


        private string _getFileText()
        {
            XDocument document;
            document = XDocument.Load(_sharedWhiteListPath);
            return document.ToString(); ;
        }

        public void ReadGlobalWhiteListPath(string codeFilePath)
        {
            if(_sharedWhiteListPath==null)
            {
                _sharedWhiteListPath = _findXMLFilePath(codeFilePath);
            }
        }

        private string _findXMLFilePath(string codeFilePath)
        {
            if (_sharedWhiteListPath!=null)
            {
                return _sharedWhiteListPath;
            }
            //codeFilePath = codeFilePath.Substring(11);
            var folderNames = codeFilePath.Split('\\');
            for (int i = folderNames.Length - 2; i >= 0; i--)
            {
                var solnFilePath = Path.Combine(String.Join("\\", folderNames, 0, i + 1), "WhiteList.xml");
                if (File.Exists(solnFilePath))
                {
                    return Path.Combine(String.Join("\\", folderNames, 0, i + 1), "WhiteList.xml");
                }
            }

            throw new Exception("Could not find project from source code path");
        }

        public void EnableTesting()
        {
            var _testPathXML = "C:\\develop\\TW-CodeAnalyzer\\Taleworlds Code Analysis\\" +
            "TaleworldsCodeAnalysis\\WhiteList.xml";
            _sharedWhiteListPath= _testPathXML;
        }


    }
}
