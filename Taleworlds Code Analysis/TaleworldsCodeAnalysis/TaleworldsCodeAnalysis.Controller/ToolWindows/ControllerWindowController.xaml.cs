using Community.VisualStudio.Toolkit;
using EnvDTE;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using TaleworldsCodeAnalysis.Controller.ToolWindows;


namespace TaleworldsCodeAnalysis.Controller
{
    public partial class ControllerWindowController
    {
        private DTE _dte;
        

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

        public void Init()
        {
            Dispatcher.VerifyAccess();
            try
            {
                var document = SettingsChecker.Instance.GetSettingsFile(SettingsParser.Instance.GetSettingsFilePath());

                //Name Checkers
                OverAll.SelectedIndex = _getSeverityIndex("OverAll", document);
                TW2002.SetSelectedIndex(_getSeverityIndex("TW2002", document));
                TW2000.SetSelectedIndex(_getSeverityIndex("TW2000", document));
                TW2005.SetSelectedIndex(_getSeverityIndex("TW2005", document));
                TW2003.SetSelectedIndex(_getSeverityIndex("TW2003", document));
                TW2004.SetSelectedIndex(_getSeverityIndex("TW2004", document));
                TW2006.SetSelectedIndex(_getSeverityIndex("TW2006", document));
                TW2007.SetSelectedIndex(_getSeverityIndex("TW2007", document));
                TW2008.SetSelectedIndex(_getSeverityIndex("TW2008", document));

                //Accesibility Checkers
                TW2001.SetSelectedIndex(_getSeverityIndex("TW2001", document));
                TW2200.SetSelectedIndex(_getSeverityIndex("TW2200", document));

                //Inheritance Checkers
                TW2100.SetSelectedIndex(_getSeverityIndex("TW2100", document));
                TW2101.SetSelectedIndex(_getSeverityIndex("TW2101", document));
                TW2102.SetSelectedIndex(_getSeverityIndex("TW2102", document));
                TW2201.SetSelectedIndex(_getSeverityIndex("TW2201", document));
                TW2202.SetSelectedIndex(_getSeverityIndex("TW2202", document));
                TW2204.SetSelectedIndex(_getSeverityIndex("TW2204", document));
                TW2205.SetSelectedIndex(_getSeverityIndex("TW2205", document));
                
            }
            catch 
            {
                return;
            }
        }

        private int _getSeverityIndex(string name, XDocument document)
        { 
            return Int32.Parse(document.Root.Element(name).Value);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void OverAll_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                TW2002.SetSelectedIndex(OverAll.SelectedIndex);
                TW2000.SetSelectedIndex(OverAll.SelectedIndex);
                TW2005.SetSelectedIndex(OverAll.SelectedIndex);
                TW2003.SetSelectedIndex(OverAll.SelectedIndex);
                TW2004.SetSelectedIndex(OverAll.SelectedIndex);
                TW2006.SetSelectedIndex(OverAll.SelectedIndex);
                TW2007.SetSelectedIndex(OverAll.SelectedIndex);
                TW2008.SetSelectedIndex(OverAll.SelectedIndex);

                //Accesibility Checkers
                TW2001.SetSelectedIndex(OverAll.SelectedIndex);
                TW2200.SetSelectedIndex(OverAll.SelectedIndex);

                //Inheritance Checkers
                TW2100.SetSelectedIndex(OverAll.SelectedIndex);
                TW2101.SetSelectedIndex(OverAll.SelectedIndex);
                TW2102.SetSelectedIndex(OverAll.SelectedIndex);
                TW2201.SetSelectedIndex(OverAll.SelectedIndex);
                TW2202.SetSelectedIndex(OverAll.SelectedIndex);
                TW2204.SetSelectedIndex(OverAll.SelectedIndex);
                TW2205.SetSelectedIndex(OverAll.SelectedIndex);
                Dispatcher.VerifyAccess();

                var path = SettingsParser.Instance.GetSettingsFilePath();
                var xDocument = SettingsChecker.Instance.GetSettingsFile(path);
                var node = xDocument.Root.Element("OverAll");
                node.ReplaceNodes(OverAll.SelectedIndex);
                xDocument.Save(path);
            }
            catch (Exception exception){
                return;
            }
        }
    }
}