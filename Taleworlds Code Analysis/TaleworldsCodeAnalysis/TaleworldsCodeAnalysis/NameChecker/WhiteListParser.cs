using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
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
                    _instance = new WhiteListParser();
                }
                
                return _instance;
            } 
        }

        public string TestPathXml => _testPathXML;

        public IReadOnlyList<string> WhiteListWords => _whiteListedWords;
        private static WhiteListParser _instance;
        private IReadOnlyList<string> _whiteListedWords;
        private const string _testPathXML = "C:\\develop\\TW-CodeAnalyzer\\Taleworlds Code Analysis\\TaleworldsCodeAnalysis\\WhiteList.xml";

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

        public void UpdateWhiteList(ImmutableArray<AdditionalText> additionalFiles)
        {
            _readWhiteList(_getFileText(additionalFiles));
        }


        public void InitializeWhiteListParser(string sourceText)
        {
            _readWhiteList(sourceText);
        }


        private string _getFileText(ImmutableArray<AdditionalText> additionalFiles)
        {
            string fileText = "";
            if (additionalFiles!=null && additionalFiles.Length != 0 )
            {
                AdditionalText whiteListFile = additionalFiles.FirstOrDefault(file => Path.GetFileName(file.Path).Equals("WhiteList.xml"));
                SourceText fileSourceText = whiteListFile.GetText();
                fileText = fileSourceText.ToString();
            }
            else
            {
                XDocument document = XDocument.Load(_testPathXML);
                fileText = document.ToString();
            }
            return fileText;
        }
    }
}
