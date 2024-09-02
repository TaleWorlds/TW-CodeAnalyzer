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
            var xDocument = GetSettingsFile(path);
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

        private XDocument GetSettingsFile(string settingPath)
        {
            XDocument xDocument;
            try
            {
                 xDocument = XDocument.Load(settingPath);
            }
            catch 
            {
                xDocument = new XDocument(new XElement("Settings", 
                    new XElement("FieldNameCheckerEnabled", "True"),
                    new XElement("MethodNameCheckerEnabled", "True"),
                    new XElement("ClassNameCheckerEnabled", "True"),
                    new XElement("InterfaceNameCheckerEnabled", "True"),
                    new XElement("LocalNameCheckerEnabled", "True"),
                    new XElement("ParameterNameCheckerEnabled", "True"),
                    new XElement("PropertyNameCheckerEnabled", "True"),
                    new XElement("TemplateParameterNameCheckerEnabled", "True"),
                    new XElement("AbstractClassCheckerEnabled", "True"),
                    new XElement("DepthOfInheritanceCheckerEnabled", "True"),
                    new XElement("SealedOverrideCheckerEnabled", "True")));
                xDocument.Save(settingPath);
            }
            return xDocument;
        }

        public void Init()
        {
            Dispatcher.VerifyAccess();
            try
            {
                var document = GetSettingsFile(GetSettingsFilePath());
                
                //Name Checkers
                FieldNameCheckerEnabled.IsChecked = _isTrue("FieldNameCheckerEnabled",document);
                MethodNameCheckerEnabled.IsChecked = _isTrue("MethodNameCheckerEnabled", document);
                ClassNameCheckerEnabled.IsChecked = _isTrue("ClassNameCheckerEnabled", document);
                InterfaceNameCheckerEnabled.IsChecked = _isTrue("InterfaceNameCheckerEnabled", document);
                LocalNameCheckerEnabled.IsChecked = _isTrue("LocalNameCheckerEnabled", document);
                ParameterNameCheckerEnabled.IsChecked = _isTrue("ParameterNameCheckerEnabled", document);
                PropertyNameCheckerEnabled.IsChecked = _isTrue("PropertyNameCheckerEnabled", document);
                TemplateParameterNameCheckerEnabled.IsChecked = _isTrue("TemplateParameterNameCheckerEnabled", document);

                //Inheritance Checkers
                AbstractClassCheckerEnabled.IsChecked = _isTrue("AbstractClassCheckerEnabled", document);
                DepthOfInheritanceCheckerEnabled.IsChecked = _isTrue("DepthOfInheritanceCheckerEnabled", document);
                SealedOverrideCheckerEnabled.IsChecked = _isTrue("SealedOverrideCheckerEnabled", document);

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