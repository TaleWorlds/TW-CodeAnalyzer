
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Windows.Threading;
using Microsoft.VisualStudio.RpcContracts.DiagnosticManagement;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Xml.Linq;

namespace TaleworldsCodeAnalysis
{
    public class ReAnalyze
    {
        public static ReAnalyze Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReAnalyze();
                }
                return _instance;
            }
        }
        private static ReAnalyze _instance;
        private DTE _dte;
        public async Task ForceReanalyzeAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (_dte==null)
            {
                _dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            }
            await PlaceDummySpaceFromGlobalAsync((Microsoft.CodeAnalysis.Document)(_dte.Documents.Item(0)));
            await RemoveDummySpaceFromGlobalAsync((Microsoft.CodeAnalysis.Document)(_dte.Documents.Item(0)));
        }

        public async Task RemoveDummySpaceFromGlobalAsync(Microsoft.CodeAnalysis.Document document)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            SyntaxNode root = null;
            document.TryGetSyntaxRoot(out root);
            var originalDeclaration = root.ReplaceNode(
                root, root.WithoutTrailingTrivia());
            var newRoot = root.ReplaceNode(root, originalDeclaration);
            var changesOperation = new ApplyChangesOperation(document.WithSyntaxRoot(newRoot).Project.Solution);
            changesOperation.Apply(document.Project.Solution.Workspace, new CancellationToken());
        }

        public async Task PlaceDummySpaceFromGlobalAsync(Microsoft.CodeAnalysis.Document document)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(); ;
            SyntaxNode root = null;
            document.TryGetSyntaxRoot(out root);
            var spacedDeclaration = root.ReplaceNode(
                root, root.WithTrailingTrivia(SyntaxFactory.Space));
            var newRoot = root.ReplaceNode(root, spacedDeclaration);
            var changesOperation = new ApplyChangesOperation(document.WithSyntaxRoot(newRoot).Project.Solution);
            changesOperation.Apply(document.Project.Solution.Workspace, new CancellationToken());
        }
    }
}
