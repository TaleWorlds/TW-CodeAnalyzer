using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace TaleworldsCodeAnalysis
{
    public class ProjectAndSolutionFinder
    {
        public static ProjectAndSolutionFinder Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new ProjectAndSolutionFinder();
                }
                return _instance;
            }
        }
        private static ProjectAndSolutionFinder _instance;

        private DTE _developmentToolsEnvironment;

        public ProjectAndSolutionFinder()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            _developmentToolsEnvironment = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        }

        public string GetCurrentProjectName()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            var projectItem = _developmentToolsEnvironment.ActiveDocument.ProjectItem;
            var projectName = projectItem.ContainingProject.Name;
            return projectItem.ContainingProject.Name;
        }

        public string GetCurrentProjectPath()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            var projectItem = _developmentToolsEnvironment.ActiveDocument.ProjectItem;
            var path = projectItem.ContainingProject.FullName;
            return path;
        }

        public string GetSolutionPath()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            var solutionPath = _developmentToolsEnvironment.Solution.FullName;
            return solutionPath;
        }
    }
}

