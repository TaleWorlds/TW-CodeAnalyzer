using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TaleworldsCodeAnalysis.Controller
{
    public class ControllerWindow : BaseToolWindow<ControllerWindow>
    {

        public override string GetTitle(int toolWindowId) => "Taleworlds Code Analysis Controller";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            var toolWindow = new ControllerWindowController();
            return Task.FromResult<FrameworkElement>(toolWindow);
        }

        [Guid("97ec4ae0-4ce3-4a7b-8701-ee0e7fca6c80")]
        internal class Pane : ToolkitToolWindowPane
        {
            private ControllerWindowController _control;
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }


        }
    }
}