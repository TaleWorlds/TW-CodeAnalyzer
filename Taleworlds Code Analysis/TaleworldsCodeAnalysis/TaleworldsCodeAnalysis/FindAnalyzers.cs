using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Elfie.Model.Map;
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
                    var fields = item.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
                    var field = item.GetField("_category",BindingFlags.NonPublic | BindingFlags.Static);

                    _taleworldsDiagnosticAnalyzers.Add(new AnalyzerInfo
                    {
                        Code = attribute.Code,
                        Name = attribute.Name,
                        Subtitle = attribute.Subtitle,
                        Category = (DiagnosticCategories)field.GetRawConstantValue()
                    });
                }
            }
        }

        public struct AnalyzerInfo
        {
            public string Name;
            public string Code;
            public string Subtitle;
            public DiagnosticCategories Category;
        }
    }
}
