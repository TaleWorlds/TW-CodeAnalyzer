using System;
using System.Collections.Generic;
using System.Text;

namespace TaleworldsCodeAnalysis
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class TaleworldsAnalyzerAttribute:System.Attribute
    {
        public string Name => _name;
        public string Code => _code;
        public string Subtitle => _subtitle;

        private string _name;
        private string _code;
        private string _subtitle;
        
        public TaleworldsAnalyzerAttribute(string name,string code, string title)
        {
            _code = code;
            _name = name;
            _subtitle = title;
        }
    }
}
