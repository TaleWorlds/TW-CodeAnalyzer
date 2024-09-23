using Community.VisualStudio.Toolkit;
using EnvDTE;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using Path = System.IO.Path;
using Solution = EnvDTE.Solution;

namespace TaleworldsCodeAnalysis.Controller.ToolWindows
{
    public class SettingsParser
    {
        public static SettingsParser Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsParser();
                }

                return _instance;
            }
        }

        private static SettingsParser _instance;
        private DTE _developmentToolsEnvironment; //TODO: uzun yaz
        private const string _pathOfSettingsFile = "TaleworldsCodeAnalysisSettings.xml";
        private string _fullPath;

        public SettingsParser()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            _developmentToolsEnvironment = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        }

        public string GetSettingsFilePath()
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            Solution solution = _developmentToolsEnvironment.Solution;
            string directoryPath = new FileInfo(solution.FullName).FullName;
            string settingPath = Path.Combine(Path.GetDirectoryName(directoryPath), _pathOfSettingsFile);
            return settingPath;
        }

    }
}
