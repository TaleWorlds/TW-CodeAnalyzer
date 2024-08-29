using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TaleworldsCodeAnalysis.Controller
{
    public class MyToolWindow : BaseToolWindow<MyToolWindow>
    {

        public override string GetTitle(int toolWindowId) => "My Tool Window";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new MyToolWindowControl());
        }

        [Guid("97ec4ae0-4ce3-4a7b-8701-ee0e7fca6c80")]
        internal class Pane : ToolkitToolWindowPane
        {
            private MyToolWindowControl _control;
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }


        }
    }
}