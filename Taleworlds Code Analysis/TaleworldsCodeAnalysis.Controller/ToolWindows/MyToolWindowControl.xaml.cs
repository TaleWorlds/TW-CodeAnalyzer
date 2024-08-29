using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell.Interop;
using MSXML;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Linq;
using Path = System.IO.Path;
using Solution = EnvDTE.Solution;

namespace TaleworldsCodeAnalysis.Controller
{
    public partial class MyToolWindowControl : UserControl
    {
        private const string _pathOfSettingsFile = "Settings.xml";
        private string _fullPath;

        public MyToolWindowControl()
        {
            InitializeComponent();
            Init();
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox source = (CheckBox)e.OriginalSource;

            var path = GetSettingsFilePath();
            var xDocument = GetSettingsFile(path);
            string name = source.Content.ToString();
            name = name.Replace(" ", "");
            var node = xDocument.Root.Element(name);
            node.ReplaceNodes(source.IsChecked.ToString());
            xDocument.Save(path);
            
        }

        private string GetSettingsFilePath()
        {
            Dispatcher.VerifyAccess();
            var _dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            Solution solution = _dte.Solution;
            string directoryPath = new FileInfo(solution.FullName).FullName;
            string settingPath = Path.Combine(directoryPath, _pathOfSettingsFile);
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
                    new XElement("MethodNameCheckerEnabled", "True")));
                xDocument.Save(settingPath);
            }
            return xDocument;
        }

        private void Init()
        {
            var document = GetSettingsFile(GetSettingsFilePath());
            FieldNameCheckerEnabled.IsChecked = document.Root.Element("FieldNameCheckerEnabled").Value=="True";
            MethodNameCheckerEnabled.IsChecked = document.Root.Element("MethodNameCheckerEnabled").Value == "True";
        }

      
    }
}