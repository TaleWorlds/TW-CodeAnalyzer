using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.NameChecker.InterfaceNameChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class InterfaceNameUnitTests
    {
        [TestMethod]
        public async Task InterfaceNoWarningTest()
        {
            var test = @"
            public interface IBase
            {
            }
            public interface ITest : IBase
            {
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task InterfaceWarningTest()
        {
            var test = @"
            public interface {|#0:iTest|} : IBase {};
            internal interface {|#1:Test|} : IBase {}
            public interface {|#2:_test|} : iBase;
            ";

            var expected1 = VerifyCS.Diagnostic("InterfaceNameChecker").WithLocation(0).WithArguments("iTest");
            var expected2 = VerifyCS.Diagnostic("InterfaceNameChecker").WithLocation(1).WithArguments("Test");
            var expected3 = VerifyCS.Diagnostic("InterfaceNameChecker").WithLocation(2).WithArguments("_test");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2, expected3);
        }

    }
}
