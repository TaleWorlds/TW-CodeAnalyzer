using Community.VisualStudio.Toolkit;
using EnvDTE;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using Path = System.IO.Path;
using Solution = EnvDTE.Solution;

namespace TaleworldsCodeAnalysis.Controller
{
    public partial class ControllerWindowController
    {
        private DTE _dte;
        private const string _pathOfSettingsFile = "TaleworldsCodeAnalysisSettings.xml";
        private string _fullPath;

        public ControllerWindowController()
        {
            Dispatcher.VerifyAccess();
            InitializeComponent();
            _dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            _dte.Events.WindowEvents.WindowActivated += WindowActivated;
        }

        ~ControllerWindowController()
        {
            _dte.Events.WindowEvents.WindowActivated -= WindowActivated;
        }

        private void WindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
        {
            Init();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.VerifyAccess();
            CheckBox source = (CheckBox)e.OriginalSource;

            var path = GetSettingsFilePath();
            var xDocument = SettingsChecker.Instance.GetSettingsFile(path);
            string name = source.Name;
            var node = xDocument.Root.Element(name);
            node.ReplaceNodes(source.IsChecked.ToString());
            xDocument.Save(path);
            ThreadHelper.JoinableTaskFactory.RunAsync(_reanalayzeTheSolution);
        }

    private async Task _reanalayzeTheSolution()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        ThreadHelper.CheckAccess();
        DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            //dte.ExecuteCommand("Analyze.OnSolution");
            dte.ExecuteCommand("Analyze.ForSolution");
    }

    private string GetSettingsFilePath()
        {
            Dispatcher.VerifyAccess();
            Solution solution = _dte.Solution;
            string directoryPath = new FileInfo(solution.FullName).FullName;
            string settingPath = Path.Combine(Path.GetDirectoryName(directoryPath), _pathOfSettingsFile);
            return settingPath;
        }



        public void Init()
        {
            Dispatcher.VerifyAccess();
            try
            {
                var document = SettingsChecker.Instance.GetSettingsFile(GetSettingsFilePath());

                //Name Checkers
                TW2002.IsChecked = _isTrue("TW2002", document);
                TW2005.IsChecked = _isTrue("TW2005", document);
                TW2000.IsChecked = _isTrue("TW2000", document);
                TW2003.IsChecked = _isTrue("TW2003", document);
                TW2004.IsChecked = _isTrue("TW2004", document);
                TW2006.IsChecked = _isTrue("TW2006", document);
                TW2007.IsChecked = _isTrue("TW2007", document);
                TW2008.IsChecked = _isTrue("TW2008", document);

                //Accesibility Checkers
                TW2001.IsChecked = _isTrue("TW2001", document);
                TW2200.IsChecked = _isTrue("TW2200", document);

                //Inheritance Checkers
                TW2100.IsChecked = _isTrue("TW2100", document);
                TW2101.IsChecked = _isTrue("TW2101", document);
                TW2102.IsChecked = _isTrue("TW2102", document);
            }
            catch 
            {
                return;
            }
        }

        private bool _isTrue(string name, XDocument document)
        {
            return document.Root.Element(name).Value == "True";
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Init();
        }
    }
}