using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TaleworldsCodeAnalysis
{
    public class BlackListedProjects
    {
        private static BlackListedProjects _instance;

        public static BlackListedProjects Instance {
            get {
                if (_instance == null) _instance = new BlackListedProjects();
                return _instance;
            }
        }

        private string localAppDataPath;
        private const string pathAfterLocalAppData = "Microsoft\\VisualStudio\\BlackListedProjects.xml";
        private string fullPath;

        private BlackListedProjects() {
            localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            fullPath = Path.Combine(localAppDataPath, pathAfterLocalAppData);
        }

        public bool isBlackListedProjectFromCodePath(string codePath) {
            var projectName = findProjectNameFromCodeFilePath(codePath);
            return isBlackListedProject(projectName);
        }

        public bool isBlackListedProject(string projectName)
        {
            XDocument doc;

            try
            {
                doc = XDocument.Load(fullPath);
            }
            catch (FileNotFoundException)
            {
                doc = new XDocument(new XElement("BlackListRoot", new XElement("Project", "ExampleProjectName")));
                doc.Save(fullPath);
            }

            var projectXElements = doc.Descendants("Project");

            foreach (var node in projectXElements)
            {
                if (node.Value == projectName) return true;
            }

            return false;
        }

        public string findProjectNameFromCodeFilePath(string codeFilePath)
        {
            var folderNames = codeFilePath.Split('\\');
            for (int i = folderNames.Length - 2; i >= 0; i--) {
                var csprojFilePath = Path.Combine(String.Join("\\", folderNames, 0, i + 1), folderNames[i] + ".csproj");
                if (File.Exists(csprojFilePath)) { 
                    return folderNames[i];
                }
            }

            throw new Exception("Could not find project from source code path");
        }

    }
}
