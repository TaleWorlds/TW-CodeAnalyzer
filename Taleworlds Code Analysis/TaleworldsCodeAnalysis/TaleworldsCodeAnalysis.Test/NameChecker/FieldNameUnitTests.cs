﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
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

            var expected = VerifyCS.Diagnostic("FieldNameChecker").WithLocation(0).WithArguments("value"); 
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }     

    }
}
