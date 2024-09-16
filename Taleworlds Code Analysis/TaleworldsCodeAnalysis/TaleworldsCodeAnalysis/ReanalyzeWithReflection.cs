using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using CommonServiceLocator;

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
                Type diagnosticServiceType = featureAssembly.GetType("Microsoft.CodeAnalysis.Diagnostics.IDiagnosticsRefresher");
                var workspace = project.Solution.Workspace;



                var reanalyzeMethod = diagnosticServiceType.GetMethod("RequestWorkspaceRefresh", BindingFlags.Instance | BindingFlags.Public);

                if (reanalyzeMethod == null)
                {
                    throw new InvalidOperationException("Failed to find the Reanalyze method.");
                }
                reanalyzeMethod.Invoke(diagnosticServiceType, new object[] { project });
            });
        }
    }
}
