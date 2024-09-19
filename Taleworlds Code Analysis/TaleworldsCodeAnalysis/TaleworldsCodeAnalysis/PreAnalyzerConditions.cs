using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis
{
    public class PreAnalyzerConditions
    {
        public static PreAnalyzerConditions _instance;
        public static PreAnalyzerConditions Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PreAnalyzerConditions();
                }
                return _instance;
            }
        }
        public bool TestMod => _testMod;
        private bool _testMod=false;

        private PreAnalyzerConditions() { }

        public bool IsNotAllowedToAnalyze(SyntaxNodeAnalysisContext context, String diagnosticId) 
        {
            if(_testMod) return false; ;
            var filePath = context.Node.GetLocation().SourceTree.FilePath;
            WhiteListParser.Instance.ReadGlobalWhiteListPath(filePath);
            if (BlackListedProjects.Instance.IsBlackListedProjectFromCodePath(filePath)) return true;
            if (AnalyzerDisablingComments.Instance.IsInDisablingComments(context.Node, diagnosticId)) return true;
            if (!SettingsChecker.Instance.IsAnalysisEnabled(diagnosticId,filePath)) return true;
            return false;
        }

        public void EnableTest()
        {
            _testMod = true;
        }
    }
}
