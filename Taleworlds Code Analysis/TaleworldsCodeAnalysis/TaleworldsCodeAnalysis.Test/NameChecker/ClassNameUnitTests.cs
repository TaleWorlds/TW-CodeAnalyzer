using Microsoft;
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
            var expected1 = VerifyCS.Diagnostic("ClassNameChecker").WithLocation(0).WithArguments("TestClassPriv", "Private", "_uscoreCase");
            var expected2 = VerifyCS.Diagnostic("ClassNameChecker").WithLocation(1).WithArguments("testClassPriv", "Private", "_uscoreCase");
            var expected3 = VerifyCS.Diagnostic("ClassNameChecker").WithLocation(2).WithArguments("TestClassInt", "Internal", "_uscoreCase");
            var expected4 = VerifyCS.Diagnostic("ClassNameChecker").WithLocation(3).WithArguments("testClassInt", "Internal", "_uscoreCase");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2,expected3,expected4);
        }

        [TestMethod]
        public async Task ClassPublicProtectedWarningTest()
        {
            var test = @"
            public class {|#0:_testClassPub|}
            {   
                
            }

            public class {|#1:testClassPub|}
            {   
               
            }
            ";

            var expected1 = VerifyCS.Diagnostic("ClassNameChecker").WithLocation(0).WithArguments("_testClassPub", "Public", "PascalCase");
            var expected2 = VerifyCS.Diagnostic("ClassNameChecker").WithLocation(1).WithArguments("testClassPub", "Public", "PascalCase");
            //var expected3 = VerifyCS.Diagnostic("ClassNameChecker").WithLocation(2).WithArguments("_testClassPro", "Internal", "_uscoreCase");
            //var expected4 = VerifyCS.Diagnostic("ClassNameChecker").WithLocation(3).WithArguments("testClassPro", "Internal", "_uscoreCase");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);

        }

    }
}
