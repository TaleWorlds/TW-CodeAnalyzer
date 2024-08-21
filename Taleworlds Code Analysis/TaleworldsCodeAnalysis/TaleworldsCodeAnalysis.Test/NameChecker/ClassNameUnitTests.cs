using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.NameChecker.ClassNameChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class ClassNameUnitTests
    {
        [TestMethod]
        public async Task ClassNoWarningTest()
        {
            var test = @"
            namespace a{
                public class TestClassPub
                {   
                    
                    private class _testClassPriv
                    {   
                
                    }
                }
            }
 
            namespace c{
                internal class _testClassInt
                {   
                
                }
            }
            ";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ClassPrivateInternalWarningTest()
        {
            var test = @"
            internal class {|#2:TestClassInt|}
            {   
               private class {|#0:TestClassPriv|}
                {   
                
                } 
            }
            internal class {|#3:testClassInt|}
            {   
                private class {|#1:testClassPriv|}
                {   
                
                }
            }
            ";

            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("ClassNameChecker").WithLocation(0).WithArguments("TestClassPriv","_testClassPriv"),
                VerifyCS.Diagnostic("ClassNameChecker").WithLocation(1).WithArguments("testClassPriv", "_testClassPriv"),
                VerifyCS.Diagnostic("ClassNameChecker").WithLocation(2).WithArguments("TestClassInt", "_testClassInt"),
                VerifyCS.Diagnostic("ClassNameChecker").WithLocation(3).WithArguments("testClassInt", "_testClassInt")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }

        [TestMethod]
        public async Task ClassPublicProtectedWarningTest()
        {
            var test = @"
            public class {|#0:_testClassPub|}
            {   
                protected class {|#2:TestClassPro|}
                {   
               
                }
            }

            public class {|#1:testClassPub|}
            {   
               protected class {|#3:testClassPro|}
                {   
               
                }
            }
            ";

            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("ClassNameChecker").WithLocation(0).WithArguments("_testClassPub","TestClassPub"),
                VerifyCS.Diagnostic("ClassNameChecker").WithLocation(1).WithArguments("testClassPub","TestClassPub"),
                VerifyCS.Diagnostic("ClassModifierChecker").WithLocation(2).WithArguments("TestClassPro"),
                VerifyCS.Diagnostic("ClassModifierChecker").WithLocation(3).WithArguments("testClassPro")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);

        }

    }
}
