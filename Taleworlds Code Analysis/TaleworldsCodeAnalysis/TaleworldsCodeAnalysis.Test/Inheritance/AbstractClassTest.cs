using Microsoft;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.Inheritance.AbstractClassChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.Inheritance
{
    [TestClass]
    public class AbstractClassTest
    {
        [TestMethod]
        public async Task NoWarning()
        {
            var test = @"
            namespace a{
                public abstract class TestClassPub
                {   
                    protected abstract void HoyThere();
                    public virtual void HiThere()
                    {

                    }
                }
            }
            ";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Warning()
        {
            var test = @"
            namespace a{
                public abstract class TestClassPub
                {   
                    protected virtual void {|#0:HoyThere|}()
                    {
                        int abc=0;   
                    }
                }
            }
            ";

            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("TW2101").WithLocation(0),
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }
    }
}
