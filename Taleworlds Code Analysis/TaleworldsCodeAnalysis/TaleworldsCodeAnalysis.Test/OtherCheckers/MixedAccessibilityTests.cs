using Microsoft;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.OtherCheckers.MixedAccessModifierChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.OtherCheckers
{
    [TestClass]
    public class MixedAccessibilityTests
    {
        [TestMethod]
        public async Task MixedAccesibilityNoWarning()
        {
            var test = @"
            public class Test
            {   
                protected void Foo2(){}
                protected int Foo => 0;
            }";
            WhiteListParser.Instance.EnableTesting();
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MixedAccesibilityWarning()
        {
            var test = @"
            public class Test
            {   
                protected {|#0:internal|} void Foo2(){}
                protected {|#1:internal|} int Foo => 0;
            }";
            WhiteListParser.Instance.EnableTesting();
            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("TW2202").WithLocation(0),
                VerifyCS.Diagnostic("TW2202").WithLocation(1)
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }

    }
}
