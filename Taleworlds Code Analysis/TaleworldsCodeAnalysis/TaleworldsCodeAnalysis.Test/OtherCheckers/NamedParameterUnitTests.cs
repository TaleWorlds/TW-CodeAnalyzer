using Microsoft;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.OtherCheckers.NamedParameterChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.OtherCheckers
{
    [TestClass]
    public class NamedParameterUnitTests
    {
        [TestMethod]
        public async Task NoWarningTest()
        {
            var test = @"
            public class Test
            {   
                public int GetOutNow()
                {
                    return {|#0:GetOut(x:1,y:1,z:1,t:1)|};
                }


                private int GetOut(int x,int y,int z, int t)
                {
                    return x+y+z+t;
                }   
            }";
            WhiteListParser.Instance.EnableTesting();
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task WarningTest()
        {
            var test = @"
            public class Test
            {   
                public int GetOutNow()
                {
                    return {|#0:GetOut(1,y:1,z:1,t:1)|};
                }


                private int GetOut(int x,int y,int z, int t)
                {
                    return x+y+z+t;
                }   
            }";
            WhiteListParser.Instance.EnableTesting();
            var expected = VerifyCS.Diagnostic("TW2201").WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
