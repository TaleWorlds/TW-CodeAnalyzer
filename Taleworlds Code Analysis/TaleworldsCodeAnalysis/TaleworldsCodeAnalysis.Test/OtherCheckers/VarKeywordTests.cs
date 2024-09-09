using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.OtherCheckers.VarKeywordChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.OtherCheckers
{
    [TestClass]
    public class VarKeywordTests
    {
        [TestMethod]
        public async Task NoWarning()
        {
            var test = @"
            public class Test
            {   
                public int Foo()
                {
                    {|#0:var|} value = 0;
                    return 0;
                }
            }
            ";
            PreAnalyzerConditions.Instance.EnableTest();
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Warning()
        {
            var test = @"
            public class Test
            {   
                public int Foo()
                {
                    {|#0:int|} value = 0;
                    return 0;
                }
            }
            ";
            PreAnalyzerConditions.Instance.EnableTest();
            var expected = VerifyCS.Diagnostic("TW2204").WithLocation(0).WithArguments("int");
            await VerifyCS.VerifyAnalyzerAsync(test,expected);
        }
    }
}
