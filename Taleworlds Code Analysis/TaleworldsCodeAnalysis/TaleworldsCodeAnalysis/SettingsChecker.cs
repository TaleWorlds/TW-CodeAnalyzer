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
            return document.Root.Element(diagnosticID).Value == "True";
        }

        public string GetSettingsFilePath(string contextPath)
        {
            if (_settingsFilePath != null)
            {
                return _settingsFilePath;
            }
            var folderNames = contextPath.Split('\\');
            string solnFilePath = "";
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
            return solnFilePath;
        }
        public XDocument GetSettingsFile(string settingPath)
        {
            XDocument xDocument;
            try
            {
                xDocument = XDocument.Load(settingPath);
            }
            catch
            {
                xDocument = new XDocument(new XElement("Settings",
                    new XElement("TW2200","True"),
                    new XElement("TW2001","True"),
                    new XElement("TW2002", "True"),
                    new XElement("TW2005", "True"),
                    new XElement("TW2000", "True"),
                    new XElement("TW2003", "True"),
                    new XElement("TW2004", "True"),
                    new XElement("TW2006", "True"),
                    new XElement("TW2007", "True"),
                    new XElement("TW2008", "True"),
                    new XElement("TW2100", "True"),
                    new XElement("TW2101", "True"),
                    new XElement("TW2102", "True"),
                    new XElement("TW2202","True"),
                    new XElement("TW2204", "True"),
                    new XElement("TW2201", "True"),
                    new XElement("TW2205","True")
                    ));
                xDocument.Save(settingPath);
            }
            return xDocument;
        }
    }
}
