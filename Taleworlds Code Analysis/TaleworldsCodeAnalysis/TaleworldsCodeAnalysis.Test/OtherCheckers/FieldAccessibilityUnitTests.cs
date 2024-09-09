using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.OtherCheckers.FieldAccessibilityChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;


namespace TaleworldsCodeAnalysis.Test.OtherCheckers
{
    [TestClass]
    public class FieldAccessibilityUnitTests
    {

        [TestMethod]
        public async Task FieldPublicWarningTest()
        {
            var test = @"
            public class Test
            {   
                public int {|#0:_value|};
            }"
            ;
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            var expected = VerifyCS.Diagnostic("TW2200").WithLocation(0).WithArguments("_value");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task FieldInternalWarningTest()
        {
            var test = @"
            public class Test
            {   
                internal int {|#0:_value|};
            }"
            ;
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            var expected = VerifyCS.Diagnostic("TW2200").WithLocation(0).WithArguments("_value");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task FieldProtectedWarningTest()
        {
            var test = @"
            public class Test
            {   
                protected int {|#0:_value|};
            }"
            ;
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            var expected = VerifyCS.Diagnostic("TW2200").WithLocation(0).WithArguments("_value");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
