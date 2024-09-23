using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Task = System.Threading.Tasks.Task;
using EnvDTE;
using EnvDTE80;
using System.Windows.Threading;


namespace TaleworldsCodeAnalysis.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    
//TWCodeAnalysis disable all
    internal sealed class AddToBlacklistCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("c8be943e-3650-4515-b592-bd0465e6f655");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToBlacklistCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AddToBlacklistCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AddToBlacklistCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in Command1's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AddToBlacklistCommand(package, commandService);
        }

        private string localAppDataPath;
        private const string pathAfterLocalAppData = "Microsoft\\VisualStudio\\BlackListedProjects.xml";
        private string fullPath;

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE developmentToolsEnvironment = (DTE)Package.GetGlobalService(typeof(DTE));
            SelectedItems selectedItems = developmentToolsEnvironment.SelectedItems;
            if (selectedItems != null && selectedItems.Count == 1)
            {
                SelectedItem item = selectedItems.Item(1);
                String projectName = item.Project.Name;

                localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                fullPath = Path.Combine(localAppDataPath, pathAfterLocalAppData);


                XDocument doc;

                if (File.Exists(fullPath))
                {
                    doc = XDocument.Load(fullPath);
                }
                else
                {
                    doc = new XDocument(new XElement("BlackListRoot", new XElement("Project", "ExampleProjectName")));
                    doc.Save(fullPath);
                }

                var root = doc.Element("BlackListRoot");
                if (root != null)
                {
                    var existingProject = root.Elements("Project").FirstOrDefault(elem => elem.Value.Equals(projectName, StringComparison.OrdinalIgnoreCase));
                    if (existingProject == null)
                    {
                        root.Add(new XElement("Project", projectName));
                    }
                    doc.Save(fullPath);
                }
                ReAnalyze.Instance.ForceReanalyzeAsync();
                string message = string.Format("Added {0} to the blacklist", projectName);
                string title = "Add to Blacklist";

                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
            
    }
}
