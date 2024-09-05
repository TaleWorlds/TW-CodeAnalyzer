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
        public string LocalPathXml => _findLocalXMLFilePath();
        public HashSet<string> WhiteListWords => _whiteListedWords;

        private const string _pathAfterLocalAppData = "Microsoft\\VisualStudio\\LocalWhiteList.xml";
        private static WhiteListParser _instance;
        private HashSet<string> _whiteListedWords;
        private string _sharedWhiteListPath;

        private WhiteListParser(){}

        private void _readWhiteList(string whiteListString)
        {
            var document =XDocument.Parse(whiteListString);

            var xElements=document.Descendants("Word");
            
            foreach (var xElement in xElements)
            {
                _whiteListedWords.Add(xElement.Value);
            }
        }

        public void UpdateWhiteList()
        {
            _whiteListedWords = new HashSet<string>();
            _readWhiteList(_getFileText(_sharedWhiteListPath));
            _readWhiteList(_getFileText(_findLocalXMLFilePath()));
        }


        private string _getFileText(string path)
        {
            XDocument document;
            try
            {
                document = XDocument.Load(path);
            }
            catch (FileNotFoundException)
            {
                document = new XDocument(new XElement("WhiteListRoot",new XElement("Word", "ExampleWord")));
                document.Save(path);
            }

            return document.ToString(); ;
        }

        public void ReadGlobalWhiteListPath(string codeFilePath)
        {
            if(_sharedWhiteListPath==null)
            {
                _sharedWhiteListPath = _findSharedXMLFilePath(codeFilePath);
            }
            UpdateWhiteList();
        }

        private string _findSharedXMLFilePath(string codeFilePath)
        {
            if (_sharedWhiteListPath!=null)
            {
                return _sharedWhiteListPath;
            }
            var folderNames = codeFilePath.Split('\\');
            string solnFilePath="";
            for (int i = folderNames.Length - 2; i >= 0; i--)
            {
                solnFilePath = Path.Combine(String.Join("\\", folderNames, 0, i + 1), "WhiteList.xml");
                if (File.Exists(solnFilePath))
                {
                    return solnFilePath;
                }
                else if (Directory.GetFiles(Path.Combine(String.Join("\\", folderNames, 0, i + 1)), "*.sln").Length!=0)
                {
                    return solnFilePath ;
                }

            }

            return solnFilePath;
        }

        private string _findLocalXMLFilePath()  
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appData, _pathAfterLocalAppData);
        }

        public void EnableTesting()
        {
            var testPathXML = "C:\\develop\\TW-CodeAnalyzer\\Taleworlds Code Analysis\\" +
            "TaleworldsCodeAnalysis\\WhiteList.xml";
            _sharedWhiteListPath= testPathXML;
        }
    }
}
