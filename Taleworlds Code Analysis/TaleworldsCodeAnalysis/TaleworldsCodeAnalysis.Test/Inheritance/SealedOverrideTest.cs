using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.Inheritance.SealedOverrideChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.Inheritance
{
    [TestClass]
    public class SealedOverrideTest
    {
        [TestMethod]
        public async Task NoWarning()
        {
            var test = @"
            namespace a{
                abstract class AbstractClass
                {
                    public abstract void HiThere();
                }

                class TestClass:AbstractClass
                {   
                    public sealed override void HiThere()
                    {

                    }
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
            namespace a{
                abstract class AbstractClass
                {
                    public abstract void HiThere();
                }

                class TestClass:AbstractClass
                {   
                    public {|#0:override|} void HiThere()
                    {

                    }
                }
            }
            ";

            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("TW2102").WithLocation(0),
            };
            PreAnalyzerConditions.Instance.EnableTest();
            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }
    }
}
