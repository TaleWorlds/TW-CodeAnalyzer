namespace TaleworldsCodeAnalysis.Controller
{
    [Command(PackageIds.ControllerCommand)]
    internal sealed class ControllerWindowCommand : BaseCommand<ControllerWindowCommand>
    {
        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            return ControllerWindow.ShowAsync();
        }
    }
}
