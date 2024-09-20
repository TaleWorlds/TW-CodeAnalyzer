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

        public IReadOnlyList<AnalyzerInfo> Analyzers => _taleworldsDiagnosticAnalyzers;

        private List<AnalyzerInfo> _taleworldsDiagnosticAnalyzers = new List<AnalyzerInfo>();

        public FindAnalyzers() {
            var analyzersAssembly = typeof(FindAnalyzers).Assembly;
            var types = analyzersAssembly.GetTypes().Where(
                    t=> t.IsClass
                );
            foreach (var item in types)
            {
                var attributes = item.GetCustomAttributes<TaleworldsAnalyzerAttribute>(false);
                foreach (var attribute in attributes)
                {
                    _taleworldsDiagnosticAnalyzers.Add(new AnalyzerInfo { 
                        Code= attribute.Code,
                        Name=attribute.Name,
                        Category=attribute.Category,
                    });
                }
            }
        }

        public struct AnalyzerInfo
        {
            public string Name;
            public string Code;
            public string Category;
        }
    }
}
