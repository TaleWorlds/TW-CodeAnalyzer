﻿using Community.VisualStudio.Toolkit;
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
using TaleworldsCodeAnalysis.NameChecker;


namespace TaleworldsCodeAnalysis.Controller
{
    public partial class ControllerWindowController
    {
        private DTE _developmentToolsEnvironment;
        private List<SeverityController> _severityControllers = new List<SeverityController>();
        private Dictionary<string,Label> categoryTabs = new Dictionary<string,Label>();
        private bool _hasInitialized;

        public ControllerWindowController()
        {
            Dispatcher.VerifyAccess();
            InitializeComponent();
            _setSeverityControllers();
            Init();
            _developmentToolsEnvironment = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            _developmentToolsEnvironment.Events.WindowEvents.WindowActivated += WindowActivated;
        }

        private void _setSeverityControllers()
        {
            var analyzers = FindAnalyzers.Instance.Analyzers;

            for (int i = 0; i < analyzers.Count; i++)
            {
                var controller = new SeverityController(
                    analyzers[i].Name+" Enabled", 
                    analyzers[i].Code,
                    IndividualSeverityChanged
                    );
                if (!categoryTabs.ContainsKey(analyzers[i].Subtitle))
                {
                    var label = new Label();
                    var category = analyzers[i].Subtitle;
                    label.Content = category;
                    SeveritiesPanel.Children.Insert(SeveritiesPanel.Children.Count-1,label);
                    categoryTabs.Add(category, label);
                }
                var index = SeveritiesPanel.Children.IndexOf(categoryTabs[analyzers[i].Subtitle]);
                SeveritiesPanel.Children.Insert(index+1, controller);
                _severityControllers.Add(controller);
            }
        }

        ~ControllerWindowController()
        {
            _developmentToolsEnvironment.Events.WindowEvents.WindowActivated -= WindowActivated;
        }

        private void WindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
        {
            Init();
        }

        public void Init()
        {
            Dispatcher.VerifyAccess();
            if(_developmentToolsEnvironment==null)
            {
                _developmentToolsEnvironment = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            }
            if(_developmentToolsEnvironment.Solution.IsOpen)
            {
                var document = SettingsChecker.Instance.GetSettingsFile(SettingsParser.Instance.GetSettingsFilePath());
                OverAll.SelectedIndex = _getSeverityIndex("OverAll", document);
                foreach (var item in _severityControllers)
                {
                    item.SetSelectedIndex(_getSeverityIndex(item.Code, document), false);
                }
                if (_developmentToolsEnvironment.ActiveDocument!=null)
                {
                    WhiteListParser.Instance.ReadGlobalWhiteListPath(_developmentToolsEnvironment.ActiveDocument.Path);
                    WhiteList.ItemsSource = WhiteListParser.Instance.WhiteListWords;
                    _developmentToolsEnvironment.Events.WindowEvents.WindowActivated -= WindowActivated;
                    _hasInitialized = true;
                }
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
            if (((ComboBox)e.OriginalSource).SelectedIndex!=3)
            {
                foreach (var item in _severityControllers)
                {
                    item.SetSelectedIndex(OverAll.SelectedIndex, true);
                    item.ResetSkipAction();
                }
            }
        }

        private void IndividualSeverityChanged()
        {
            var selectedIndex = OverAll.SelectedIndex;
            foreach (var item in _severityControllers)
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

            foreach (var item in _severityControllers)
            {
                node = xDocument.Root.Element(item.Code);
                node.ReplaceNodes(item.ComboBox.SelectedIndex);
            }

            xDocument.Save(path);
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            Save();
            ReAnalyze.Instance.ForceReanalyzeAsync();
        }

        private void RefreshButton(object sender, RoutedEventArgs e)
        {
            Init();
            ReAnalyze.Instance.ForceReanalyzeAsync();
        }

        private void AddToWhiteList(object sender, RoutedEventArgs e)
        {
            var path = WhiteListScope.SelectedIndex==0 ? 
                WhiteListParser.Instance.LocalPathXml : 
                WhiteListParser.Instance.SharedPathXml;
            WhiteListParser.Instance.AddStringToWhiteList(path, NewWhiteListItem.Text.Trim());
            NewWhiteListItem.Text = "[Insert New Item Here]";
            RefreshButton(sender, e);
        }

        private void RemoveItem(object sender, RoutedEventArgs e)
        {
            WhiteListParser.Instance.RemoveWord(WhiteList.SelectedItems);
            RefreshButton(sender, e);
        }
    }
}