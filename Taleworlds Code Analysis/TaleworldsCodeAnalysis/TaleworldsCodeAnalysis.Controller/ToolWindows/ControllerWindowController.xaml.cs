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
        private const string _pathOfSettingsFile = "Settings.xml";
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
            var xDocument = SettingsChecker.GetSettingsFile(path);
            string name = source.Name;
            var node = xDocument.Root.Element(name);
            node.ReplaceNodes(source.IsChecked.ToString());
            xDocument.Save(path);
           
            var activeDocument = _dte.ActiveDocument;
            if (activeDocument!=null)
            {
                activeDocument.Activate();
            }
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
                var document = SettingsChecker.GetSettingsFile(GetSettingsFilePath());
                
                //Name Checkers
                FieldNameCheckerEnabled.IsChecked = _isTrue("TW2002", document);
                MethodNameCheckerEnabled.IsChecked = _isTrue("TW2005", document);
                ClassNameCheckerEnabled.IsChecked = _isTrue("TW2000", document);
                InterfaceNameCheckerEnabled.IsChecked = _isTrue("TW2003", document);
                LocalNameCheckerEnabled.IsChecked = _isTrue("TW2004", document);
                ParameterNameCheckerEnabled.IsChecked = _isTrue("TW2006", document);
                PropertyNameCheckerEnabled.IsChecked = _isTrue("TW2007", document);
                TemplateParameterNameCheckerEnabled.IsChecked = _isTrue("TW2008", document);

                //Inheritance Checkers
                AbstractClassCheckerEnabled.IsChecked = _isTrue("TW2100", document);
                DepthOfInheritanceCheckerEnabled.IsChecked = _isTrue("TW2101", document);
                SealedOverrideCheckerEnabled.IsChecked = _isTrue("TW2102", document);

                _dte.Events.WindowEvents.WindowActivated -= WindowActivated;
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