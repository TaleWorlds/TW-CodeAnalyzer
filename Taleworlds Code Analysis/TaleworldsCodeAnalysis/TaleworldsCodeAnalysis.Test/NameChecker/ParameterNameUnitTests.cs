using Microsoft;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.NameChecker.ParameterNameChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class ParameterNameUnitTests
    {
        [TestMethod]
        public async Task ParameterNoWarningTest()
        {
            var test = @"
            public class Test
            {   
                private int Foo(int dummyCase, string dummy, int a)
                {
                    return 0;
                }
            }"
            ;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ParameterWarningTest()
        {
            var test = @"
            public class Test
            {   
                private int _fooInt(int {|#0:Value|})
                {
                    return 0;
                }

                private void _fooVoid(int {|#1:_value|})
                {
                    return ;
                }   
            }";
            var expected1 = VerifyCS.Diagnostic("ParameterAndLocalNameChecker").WithLocation(0).WithArguments("Value");
            var expected2 = VerifyCS.Diagnostic("ParameterAndLocalNameChecker").WithLocation(1).WithArguments("_value");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }
    }

    
}
