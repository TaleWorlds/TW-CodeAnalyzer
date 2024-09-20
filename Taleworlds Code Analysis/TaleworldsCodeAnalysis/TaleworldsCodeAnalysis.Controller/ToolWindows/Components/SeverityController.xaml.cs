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
            get 
            { 
                return (string)GetValue(TitleProperty); 
            }
            set 
            {
                SetValue(TitleProperty, value); 
            }
        }

        public string AdditionalChoice
        {
            get 
            { 
                return (string)GetValue(AdditionalChoiceProperty); 
            }
            set 
            { 
                SetValue(AdditionalChoiceProperty, value); 
            }
        }

        public string Code
        {
            get 
            { 
                return (string)GetValue(CodeProperty); 
            }
            set 
            { 
                SetValue(CodeProperty, value); 
            }
        }

        public Action IndividualChanged
        {
            get 
            { 
                return (Action)GetValue(IndividualChangedProperty); 
            }
            set 
            { 
                SetValue(IndividualChangedProperty, value); 
            }
        }



        public static readonly DependencyProperty IndividualChangedProperty =
            DependencyProperty.Register("IndividualChanged", typeof(Action), typeof(SeverityController), new PropertyMetadata());


        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SeverityController), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(SeverityController), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty AdditionalChoiceProperty =
            DependencyProperty.Register("AdditionalChoice", typeof(string), typeof(SeverityController), new PropertyMetadata(string.Empty));

        private bool _skipAction;

        public SeverityController()
        {
            InitializeComponent();
        }

        public SeverityController(string name, string code, Action individualChanged)
        {
            InitializeComponent();
            Name = code;
            Title = name;
            Code = code;
            IndividualChanged = individualChanged;
        }

        internal void SetSelectedIndex(int index,bool byOverAll)
        {
            _skipAction = byOverAll;
            ComboBox.SelectedIndex = index;
        }

        internal void ResetSkipAction()
        {
            _skipAction = false;
        }

        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            try // Overhead i kötü, kullanma
            {
                Console.WriteLine(ComboBox.SelectedIndex);
                if (IndividualChanged==null)
                {
                    return;
                }
                if (!_skipAction)
                {
                    IndividualChanged.Invoke();
                }
            }
            catch (Exception ex)
            {
                return;
            }
            
        }

        internal int GetSelectedIndex()
        {
            return ComboBox.SelectedIndex;
        }
    }
}
