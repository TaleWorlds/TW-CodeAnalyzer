using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;
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
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task InterfaceWarningTest()
        {
            var test = @"
            public interface IBase
            {
            }
            public interface {|#0:iTest|} : IBase {}
            internal interface {|#1:Test|} : IBase {}
            public interface {|#2:_test|} : IBase{}
            ";
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("TW2003").WithLocation(0).WithArguments("iTest","ITest"),
                VerifyCS.Diagnostic("TW2003").WithLocation(1).WithArguments("Test","ITest"),
                VerifyCS.Diagnostic("TW2003").WithLocation(2).WithArguments("_test","ITest")
            };
            

            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }

    }
}
