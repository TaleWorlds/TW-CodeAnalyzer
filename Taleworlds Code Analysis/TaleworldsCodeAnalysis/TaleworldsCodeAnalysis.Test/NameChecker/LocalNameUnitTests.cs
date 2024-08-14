﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.NameChecker.LocalNameChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class LocalNameUnitTests
    {
        [TestMethod]
        public async Task LocalNoWarningTest()
        {
            var test = @"
            public class Test
            {   
                public int Foo()
                {
                    int value = 0;
                    var value2 = Foo();
                    var a = 3;
                    return a;
                }
            }
            ";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task InterfaceWarningTest()
        {
            var test = @"
            public class Test
            {   
                public int Foo()
                {
                    int {|#0:Value|} = 0;
                    var {|#1:_value2|} = Foo();
                    return 0;
                }
            }
            ";

            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("LocalNameChecker").WithLocation(0).WithArguments("Value"),
                VerifyCS.Diagnostic("LocalNameChecker").WithLocation(1).WithArguments("_value2")
            };

            

            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }

    }
}
