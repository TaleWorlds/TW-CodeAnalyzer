using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualStudio.Shell;

namespace TaleworldsCodeAnalysis
{
    public static class ReanalyzeWithReflection
    {
        public static void ForceReanalyzeUsingReflection(Project project)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assembly featureAssembly =null;
                foreach (var assembly in loadedAssemblies)
                {
                    if (assembly.GetName().Name == "Microsoft.CodeAnalysis.Features")
                    {
                        featureAssembly = assembly;
                    }
                    
                }
                if (featureAssembly == null) return;
                var diagnosticServiceType = featureAssembly.GetType("Microsoft.CodeAnalysis.Diagnostics.IDiagnosticsRefresher");

                if (diagnosticServiceType == null)
                {
                    throw new InvalidOperationException("Failed to find DiagnosticAnalyzerService in Microsoft.CodeAnalysis.Features.");
                }

                // Get the DiagnosticAnalyzerService instance from the workspace using reflection
                var workspace = project.Solution.Workspace;


                // Get the Reanalyze method
                var reanalyzeMethod = diagnosticServiceType.GetMethod("RequestWorkspaceRefresh", BindingFlags.Instance | BindingFlags.Public);

                if (reanalyzeMethod == null)
                {
                    throw new InvalidOperationException("Failed to find the Reanalyze method.");
                }
                reanalyzeMethod.Invoke(diagnosticServiceType, new object[] { project });
                // Invoke Reanalyze on the project to force reanalysis
                //reanalyzeMethod.Invoke();
            });
        }
    }
}
