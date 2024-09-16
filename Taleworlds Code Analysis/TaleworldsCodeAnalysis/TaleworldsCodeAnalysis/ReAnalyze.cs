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
        private DTE _dte;
        public void ForceReanalyze()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            _dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            _dte.ExecuteCommand("Build.BuildSolution");
        }
    }
}
