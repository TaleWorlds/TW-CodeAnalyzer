using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using CommonServiceLocator;
using EnvDTE;
using System.Windows.Threading;

namespace TaleworldsCodeAnalysis
{
    public class ReAnalyze
    {
        public static ReAnalyze Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReAnalyze();
                }
                return _instance;
            }
        }
        private static ReAnalyze _instance;
        private DTE _developmentToolsEnvironment;
        public void ForceReanalyze()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            if (_developmentToolsEnvironment==null)
            {
                _developmentToolsEnvironment = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            }
            _developmentToolsEnvironment.ExecuteCommand("Build.BuildSolution");
        }
    }
}
