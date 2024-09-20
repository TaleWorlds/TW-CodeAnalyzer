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

        private string _name;
        private string _code;
        
        public TaleworldsAnalyzerAttribute(string name,string code)
        {
            _code = code;
            _name = name;
        }
    }
}
