using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.Controller;

[Command(PackageIds.ControllerCommand)]
internal sealed class ControllerWindowCommand : BaseCommand<ControllerWindowCommand>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        return ControllerWindow.ShowAsync();
    }
}
