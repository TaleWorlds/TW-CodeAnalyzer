using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.Inheritance.DepthOfInheritanceChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;
using Microsoft.CodeAnalysis.Testing;

namespace TaleworldsCodeAnalysis.Test.Inheritance
{
    [TestClass]
    public class DepthOfInheritanceTest
    {
        [TestMethod]
        public async Task NoWarning()
        {
            var test = @"
            class BaseClass {}
            class FirstDerivedClass : BaseClass {}
            ";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Warning()
        {
            var test = @"
            class BaseClass {}
            class FirstDerivedClass : BaseClass {}
            class {|#0:SecondDerivedClass|} : FirstDerivedClass {}
            class {|#1:ThirdDerivedClass|} : SecondDerivedClass {}
            class {|#2:FourthDerivedClass|} : ThirdDerivedClass {}
            ";
            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("TW2102").WithLocation(0),
                VerifyCS.Diagnostic("TW2102").WithLocation(1),
                VerifyCS.Diagnostic("TW2102").WithLocation(2)
            };
            await VerifyCS.VerifyAnalyzerAsync(test,expectedResults);
        }
    }
}
