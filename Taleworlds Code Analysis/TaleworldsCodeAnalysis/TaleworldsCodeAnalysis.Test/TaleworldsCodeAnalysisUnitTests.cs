using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisAnalyzer,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test
{
    [TestClass]
    public class TaleworldsCodeAnalysisUnitTest
    {
        [TestMethod]
        public async Task FieldNoWarningTest ()
        {
            var test = @"
            public class Test
            {   
                private int _value;
            }";

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

            var expected = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("Test"); //TODO: Add the argument here
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task FieldPublicWarningTest()
        {
            var test = @"
            public class Test
            {   
                public int {|#0:_value|};
            }";

            var expected = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("Test"); //TODO: Add the argument here
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task FieldInternalWarningTest()
        {
            var test = @"
            public class Test
            {   
                internal int {|#0:_value|};
            }";

            var expected = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("Test"); //TODO: Add the argument here
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task FieldProtectedWarningTest()
        {
            var test = @"
            public class Test
            {   
                protected int {|#0:_value|};
            }";

            var expected = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("Test"); //TODO: Add the argument here
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }


    }
}
