using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.NameChecker.FieldNameChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class FieldNameUnitTests
    {
        [TestMethod]
        public async Task FieldNoWarningTest ()
        {
            var test = @"
            public class Test
            {   
                private int _value;
            }";
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FieldPrivateUnderscoreWarningTest()
        {
            var test = @"
            public class Test
            {   
                private int {|#0:value|};
            }";
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            var expected = VerifyCS.Diagnostic("TW2002").WithLocation(0).WithArguments("value","_value"); 
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }     

    }
}
