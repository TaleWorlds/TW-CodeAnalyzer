using Microsoft.Build.Tasks;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace TaleworldsCodeAnalysis
{
    public class SettingsChecker
    {
        public static SettingsChecker Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new SettingsChecker();
                }
                return _instance;
            }
        } 
        private static SettingsChecker _instance;

        private const string _nameOfSettingsFile = "TaleworldsCodeAnalysisSettings.xml";
        private string _settingsFilePath;
        

        public bool IsAnalysisEnabled(string diagnosticID, string contextPath)
        {
            var document = GetSettingsFile(GetSettingsFilePath(contextPath));
            if (document.Root.Element(diagnosticID)==null)
            {
                document.Root.Add(new XElement(diagnosticID, "2"));
                document.Save(GetSettingsFilePath(contextPath));
            }
            
            return document.Root.Element(diagnosticID).Value != "0";
        }

        public string GetSettingsFilePath(string contextPath)
        {
            var solnFilePath = _settingsFilePath;
            if (_settingsFilePath == null)
            {
                var folderNames = contextPath.Split('\\');
                for (int i = folderNames.Length - 2; i >= 0; i--)
                {
                    solnFilePath = Path.Combine(String.Join("\\", folderNames, 0, i + 1), _nameOfSettingsFile);
                    if (File.Exists(solnFilePath))
                    {
                        return solnFilePath;
                    }
                    else if (Directory.GetFiles(Path.Combine(String.Join("\\", folderNames, 0, i + 1)), "*.sln").Length != 0)
                    {
                        return solnFilePath;
                    }
                }
            }
            return solnFilePath;
        }
        public XDocument GetSettingsFile(string settingPath)
        {
            XDocument xDocument;
            if (File.Exists(settingPath))
            {
                xDocument = XDocument.Load(settingPath);
            }
            else
            {
                var root = new XElement("Settings");
                xDocument = new XDocument(root);
                foreach (var item in FindAnalyzers.Instance.Analyzers)
                {
                    xDocument.Root.Add(new XElement(item.Code, "2"));
                }
                xDocument.Root.Add(new XElement("OverAll", "2"));
                xDocument.Save(settingPath);
            }
            return xDocument;
        }

        public DiagnosticSeverity GetDiagnosticSeverity(string diagnosticId, string contextPath, DiagnosticSeverity defaultSeverity) 
        {
            var severity = defaultSeverity;
            if (!PreAnalyzerConditions.Instance.TestMod)
            {
                var document = GetSettingsFile(GetSettingsFilePath(contextPath));
                switch (document.Root.Element(diagnosticId).Value)
                {
                    case "0":
                        severity = DiagnosticSeverity.Hidden;
                        break;
                    case "1":
                        severity = DiagnosticSeverity.Warning;
                        break;
                    case "2":
                        severity = DiagnosticSeverity.Error;
                        break;
                }
            }
            return severity;
        }

    }
}
