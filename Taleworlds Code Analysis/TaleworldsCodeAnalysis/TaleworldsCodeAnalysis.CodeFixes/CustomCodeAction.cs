using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.RpcContracts.DiagnosticManagement;

namespace TaleworldsCodeAnalysis
{
    public class CustomCodeAction : CodeAction
    {

        public override string Title { get; }
        public override string EquivalenceKey { get; }

        private readonly Func<CancellationToken, bool, Task<Solution>> _createChangedSolution;

        public CustomCodeAction(string title, Func<CancellationToken, bool, Task<Solution>> createChangedSolution, string equivalenceKey = null)
        {
            Title = title;
            EquivalenceKey = equivalenceKey;
            _createChangedSolution = createChangedSolution;
        }

        public static CustomCodeAction Create(string title, Func<CancellationToken, bool, Task<Solution>> createChangedSolution, string equivalenceKey = null) { 
            return new CustomCodeAction(title, createChangedSolution, equivalenceKey);
        }

        protected sealed override async Task<IEnumerable<CodeActionOperation>> ComputePreviewOperationsAsync(CancellationToken cancellationToken)
        {
            const bool isPreview = true;
            // Content copied from http://sourceroslyn.io/#Microsoft.CodeAnalysis.Workspaces/CodeActions/CodeAction.cs,81b0a0866b894b0e,references
            var changedSolution = await GetChangedSolutionWithPreviewAsync(cancellationToken, isPreview).ConfigureAwait(false);
            
            return new CodeActionOperation[] { new ApplyChangesOperation(changedSolution)};
        }

        protected sealed override Task<Solution> GetChangedSolutionAsync(CancellationToken cancellationToken)
        {
            const bool isPreview = false;
            return GetChangedSolutionWithPreviewAsync(cancellationToken, isPreview);
        }

        protected virtual Task<Solution> GetChangedSolutionWithPreviewAsync(CancellationToken cancellationToken, bool isPreview)
        {
            return _createChangedSolution(cancellationToken, isPreview);
        }

    }
}
