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
    public static class SettingsChecker
    {
        private const string _pathOfSettingsFile = "Settings.xml";
        public static bool IsAnalysisEnabled(string diagnosticID, Solution solution)
        {
            var document = GetSettingsFile(GetSettingsFilePath(solution));
            return document.Root.Element(diagnosticID).Value == "True";
        }

        public static string GetSettingsFilePath(Solution solution)
        {
            string directoryPath = solution.FilePath;
            string settingPath = Path.Combine(Path.GetDirectoryName(directoryPath), _pathOfSettingsFile);
            return settingPath;
        }
        public static XDocument GetSettingsFile(string settingPath)
        {
            XDocument xDocument;
            try
            {
                xDocument = XDocument.Load(settingPath);
            }
            catch
            {
                xDocument = new XDocument(new XElement("Settings",
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
                    new XElement("TW2102", "True")));
                xDocument.Save(settingPath);
            }
            return xDocument;
        }
    }
}
