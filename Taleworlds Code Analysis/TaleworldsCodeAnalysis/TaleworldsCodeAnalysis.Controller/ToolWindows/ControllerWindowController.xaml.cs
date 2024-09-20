using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Package;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using TaleworldsCodeAnalysis.Controller.ToolWindows;
using TaleworldsCodeAnalysis.Controller.ToolWindows.Components;


namespace TaleworldsCodeAnalysis.Controller
{
    public partial class ControllerWindowController
    {
        private DTE _dte;
        private List<SeverityController> severityControllers;
        private bool _hasInitialized;

        public ControllerWindowController()
        {
            Dispatcher.VerifyAccess();
            InitializeComponent();

            _dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            _dte.Events.WindowEvents.WindowActivated += WindowActivated;

            severityControllers = new List<SeverityController>() //bütün severity controllerları ekle böyle yazma
            {
                TW2002, TW2000, TW2005, TW2003,TW2004, TW2006, TW2007,TW2008,
                TW2001,TW2200, TW2100,TW2101,TW2102,TW2201,TW2202, TW2204, TW2205 
            };
            
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
                OverAll.SelectedIndex = _getSeverityIndex("OverAll", document);
                foreach (var item in severityControllers)
                {
                    item.SetSelectedIndex(_getSeverityIndex(item.Code, document),false);
                }
                _dte.Events.WindowEvents.WindowActivated -= WindowActivated;
                _hasInitialized = true;
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
            if (_hasInitialized)
            {
                return ;
            }
            Init();
        }

        private void OverAll_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((ComboBox)e.OriginalSource).SelectedIndex==3)
                {
                    return ;
                }
                foreach (var item in severityControllers)
                {
                    item.SetSelectedIndex(OverAll.SelectedIndex,true);
                    item.ResetSkipAction();
                }
            }
            catch (Exception exception){
                return;
            }

        }

        private void IndividualSeverityChanged()
        {
            var selectedIndex = OverAll.SelectedIndex;
            foreach (var item in severityControllers)
            {
                if (item.GetSelectedIndex()!=selectedIndex)
                {
                    OverAll.SelectedIndex = 3;
                    return;
                }
            }
           
        }

        private void Save()
        {
            Dispatcher.VerifyAccess();
            var path = SettingsParser.Instance.GetSettingsFilePath();
            var xDocument = SettingsChecker.Instance.GetSettingsFile(path);
            var node = xDocument.Root.Element("OverAll");
            node.ReplaceNodes(OverAll.SelectedIndex);

            foreach (var item in severityControllers)
            {
                node = xDocument.Root.Element(item.Code);
                node.ReplaceNodes(item.ComboBox.SelectedIndex);
            }

            xDocument.Save(path);
            ReAnalyze.Instance.ForceReanalyze();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
    }
}