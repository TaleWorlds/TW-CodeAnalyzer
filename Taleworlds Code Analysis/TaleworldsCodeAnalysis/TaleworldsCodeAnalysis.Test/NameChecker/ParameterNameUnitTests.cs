using Microsoft;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;
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
            WhiteListParser.Instance.EnableTesting();
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
            WhiteListParser.Instance.EnableTesting();
            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("ParameterNameChecker").WithLocation(0).WithArguments("Value","value"),
                VerifyCS.Diagnostic("ParameterNameChecker").WithLocation(1).WithArguments("_value","value")
            };
            
            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }
    }

    
}
