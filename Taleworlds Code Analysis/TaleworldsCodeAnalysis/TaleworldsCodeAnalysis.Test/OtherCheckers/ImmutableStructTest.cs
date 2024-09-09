using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.OtherCheckers.ImmutableStructChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.OtherCheckers
{
    [TestClass]
    public class ImmutableStructTest
    {
        [TestMethod]
        public async Task NoWarning()
        {
            var test = @"
            public struct {|#0:Test|}
            {   
                public readonly int _valueInt;
            }";
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Warning()
        {
            var test = @"
            public struct {|#0:Test|}
            {   
                public int _valueInt;
            }";
            WhiteListParser.Instance.EnableTesting();
            PreAnalyzerConditions.Instance.EnableTest();
            var expected = VerifyCS.Diagnostic("TW2205").WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(test,expected);
        }
    }
}
