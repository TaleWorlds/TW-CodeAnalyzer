using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TaleworldsCodeAnalysis.Inheritance;

namespace TaleworldsCodeAnalysis
{
    public class FindAnalyzers
    {
        public static FindAnalyzers Instance
        {
            get
            {
                if (_instance==null)
                {
                    _instance = new FindAnalyzers();
                }
                return _instance;
            }
        }
        private static FindAnalyzers _instance;

        private List<AnalyzerInfo> taleworldsDiagnosticAnalyzers = new List<AnalyzerInfo>();

        public FindAnalyzers() {
            var analyzersAssembly = typeof(AbstractClassChecker).Assembly;
            var types = analyzersAssembly.GetTypes().Where(
                    t=> t.IsClass
                );
            foreach (var item in types)
            {
                var attributes = item.GetCustomAttributes<TaleworldsAnalyzerAttribute>(false);
                foreach (var attribute in attributes)
                {
                    taleworldsDiagnosticAnalyzers.Add(new AnalyzerInfo { 
                        Code= attribute.Code,
                        Name=attribute.Name
                    });
                }
            }
        }

        private struct AnalyzerInfo
        {
            public string Name;
            public string Code;
        }
    }
}
