using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace TaleworldsCodeAnalysis
{
    public class BlackListedProjects
    {
        private static BlackListedProjects _instance;

        public static BlackListedProjects Instance 
        {
            get 
            {
                if (_instance == null) 
                {
                    _instance = new BlackListedProjects();
                }
                
                return _instance;
            }
        }

        private string _localAppDataPath;
        private const string _pathAfterLocalAppData = "Microsoft\\VisualStudio\\BlackListedProjects.xml";
        
        private string _fullPath;

        private BlackListedProjects() 
        {
            _localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _fullPath = Path.Combine(_localAppDataPath, _pathAfterLocalAppData);
        }

        public bool IsBlackListedProjectFromCodePath(string codePath) 
        {
            var projectName = FindProjectNameFromCodeFilePath(codePath);
            return IsBlackListedProject(projectName);
        }

        public bool IsBlackListedProject(string projectName)
        {
            XDocument doc;

            if (File.Exists(_fullPath))
            {
                doc = XDocument.Load(_fullPath);
            }
            else
            {
                doc = new XDocument(new XElement("BlackListRoot", new XElement("Project", "ExampleProjectName")));
                doc.Save(_fullPath);
            }

            var projectXElements = doc.Descendants("Project");

            foreach (var node in projectXElements)
            {
                if (node.Value == projectName)
                {
                    return true;
                }
            }

            return false;
        }

        public string FindProjectNameFromCodeFilePath(string codeFilePath)
        {
            var folderNames = codeFilePath.Split('\\');
            for (int i = folderNames.Length - 2; i >= 0; i--)
            {
                var csprojFilePath = Path.Combine(String.Join("\\", folderNames, 0, i + 1), folderNames[i] + ".csproj");
                if (File.Exists(csprojFilePath))
                {
                    return folderNames[i];
                }
            }

            throw new Exception("Could not find project from source code path");
        }

    }
}
