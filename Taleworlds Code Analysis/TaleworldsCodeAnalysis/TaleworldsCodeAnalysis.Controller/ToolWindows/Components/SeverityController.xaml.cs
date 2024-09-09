using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaleworldsCodeAnalysis.Controller.ToolWindows.Components
{
    /// <summary>
    /// Interaction logic for SeverityController.xaml
    /// </summary>
    public partial class SeverityController : UserControl
    {

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string AdditionalChoice
        {
            get { return (string)GetValue(AdditionalChoiceProperty); }
            set { SetValue(AdditionalChoiceProperty, value); }
        }

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        public SeverityController()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SeverityController), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(SeverityController), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty AdditionalChoiceProperty =
            DependencyProperty.Register("AdditionalChoice", typeof(string), typeof(SeverityController), new PropertyMetadata(string.Empty));

        internal void SetSelectedIndex(int index)
        {
            ComboBox.SelectedIndex = index;
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.VerifyAccess();
                ComboBox source = (ComboBox)e.OriginalSource;

                var path = SettingsParser.Instance.GetSettingsFilePath();
                var xDocument = SettingsChecker.Instance.GetSettingsFile(path);
                var node = xDocument.Root.Element(Code);
                node.ReplaceNodes(source.SelectedIndex);
                xDocument.Save(path);
            }
            catch (Exception ex)
            {
                return;
            }
            
        }

    }
}
