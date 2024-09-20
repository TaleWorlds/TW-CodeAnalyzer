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
            var instance=FindAnalyzers.Instance;
            try
            {
                xDocument = XDocument.Load(settingPath);
            }
            catch
            {
                xDocument = new XDocument(new XElement("Settings",
                    new XElement("TW2200","2"),
                    new XElement("TW2001","2"),
                    new XElement("TW2002", "2"),
                    new XElement("TW2005", "2"),
                    new XElement("TW2000", "2"),
                    new XElement("TW2003", "2"),
                    new XElement("TW2004", "2"),
                    new XElement("TW2006", "2"),
                    new XElement("TW2007", "2"),
                    new XElement("TW2008", "2"),
                    new XElement("TW2100", "2"),
                    new XElement("TW2101", "2"),
                    new XElement("TW2102", "2"),
                    new XElement("TW2202","2"),
                    new XElement("TW2204", "2"),
                    new XElement("TW2201", "2"),
                    new XElement("TW2205","2"),
                    new XElement("OverAll", "2")
                    ));
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
