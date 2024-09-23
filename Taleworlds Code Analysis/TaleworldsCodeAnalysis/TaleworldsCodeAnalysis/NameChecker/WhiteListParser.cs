﻿using Microsoft.CodeAnalysis;
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
using System.Collections;

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
            if (File.Exists(path))
            {
                document = XDocument.Load(path);
            }
            else
            {
                document = new XDocument(new XElement("WhiteListRoot", new XElement("Word", "ExampleWord")));
                document.Save(path);
            }

            return document.ToString(); ;
        }

        public void ReadGlobalWhiteListPath(string codeFilePath)
        {
            if(_sharedWhiteListPath==null)
            {
                _sharedWhiteListPath = _findSharedXMLFilePath();
            }
            UpdateWhiteList();
        }

        private string _findSharedXMLFilePath()
        {
            var currentPath =ProjectAndSolutionFinder.Instance.GetCurrentProjectPath();

            var currentDirectory = Path.GetDirectoryName(currentPath);
            var expectedPath = currentDirectory + "\\WhiteList.xml";
           
            while (!File.Exists(expectedPath))
            {
                currentDirectory = Path.GetDirectoryName(currentDirectory);
                expectedPath = currentDirectory + "\\WhiteList.xml";
            }

            return expectedPath;
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

        public void AddStringToWhiteList(string filePath, string wordToAdd)
        {
            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Element("WhiteListRoot");
                if (root != null)
                {
                    var existingWord = root.Elements("Word").FirstOrDefault(e => e.Value.Equals(wordToAdd));
                    if (existingWord == null)
                    {
                        root.Add(new XElement("Word", wordToAdd));
                    }
                    doc.Save(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void RemoveWord(IList selectedItems)
        {
            void RemoveFormXML(string path)
            {
                var doc = XDocument.Load(path);
                var root = doc.Element("WhiteListRoot");
                if (root != null)
                {
                    foreach (var item in selectedItems)
                    {
                        var existingWord = root.Elements("Word").FirstOrDefault(e => e.Value.Equals(item.ToString()));
                        if (existingWord != null)
                        {
                            existingWord.Remove();
                        }
                    }
                }
                doc.Save(path);
            }
            RemoveFormXML(SharedPathXml);
            RemoveFormXML(LocalPathXml);
            
        }
    }
}
