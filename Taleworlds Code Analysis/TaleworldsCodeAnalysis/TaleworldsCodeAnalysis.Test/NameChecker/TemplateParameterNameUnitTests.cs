using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.NameChecker.TemplateParameterNameChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class TemplateParameterNameUnitTests
    {
        [TestMethod]
        public async Task TemplateParameterNoWarningTest()
        {
            var test1 = @"
            public class Test<TApp>
            {   
                
            }";

            await VerifyCS.VerifyAnalyzerAsync(test1);
        }

        [TestMethod]
        public async Task TemplateParameterWarningTest()
        {
            var test1 = @"
            public class Test<{|#0:tApp|}>
            {   
                
            }";

            var test2 = @"
            public class Test<{|#0:Tapp|}>
            {   
                
            }";

            var test3 = @"
            public class Test<{|#0:tapp|}>
            {   
                
            }";

            var test4 = @"
            public class Test<{|#0:IApp|}>
            {   
                
            }";

            var expected1 = VerifyCS.Diagnostic("TemplateParameterNameChecker").WithLocation(0).WithArguments("tApp","TApp");
            var expected2 = VerifyCS.Diagnostic("TemplateParameterNameChecker").WithLocation(0).WithArguments("Tapp","TApp");
            var expected3 = VerifyCS.Diagnostic("TemplateParameterNameChecker").WithLocation(0).WithArguments("tapp","TApp");
            var expected4 = VerifyCS.Diagnostic("TemplateParameterNameChecker").WithLocation(0).WithArguments("IApp","TIapp");
            await VerifyCS.VerifyAnalyzerAsync(test1, expected1);
            await VerifyCS.VerifyAnalyzerAsync(test2, expected2);
            await VerifyCS.VerifyAnalyzerAsync(test3, expected3);
            await VerifyCS.VerifyAnalyzerAsync(test4, expected4);
        }
    }
}
