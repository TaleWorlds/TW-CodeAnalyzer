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

            var expected = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("_value"); //TODO: Add the argument here
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

            var expected = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("_value"); //TODO: Add the argument here
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

            var expected = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("_value"); //TODO: Add the argument here
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

            var expected = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("_value"); //TODO: Add the argument here
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task MethodNoWarningTest()
        {
            var test = @"
            public class Test
            {   
                private void _foo0(){}
                internal void _foo1(){}
                protected void Foo2(){}
                public void Foo3(){}
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }


        [TestMethod]
        public async Task MethodPrivateWarningTest()
        {
            var test = @"
            public class Test
            {   
                private void {|#0:foo|}(){}
                private void {|#1:_Foo|}(){}

            }";

            var expected1 = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("foo");
            var expected2 = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(1).WithArguments("_Foo");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task MethodInternalWarningTest()
        {
            var test = @"
            public class Test
            {   
                internal void {|#0:foo|}(){}
                internal void {|#1:_Foo|}(){}

            }";

            var expected1 = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("foo");
            var expected2 = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(1).WithArguments("_Foo");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task MethodPublicWarningTest()
        {
            var test = @"
            public class Test
            {   
                public void {|#0:foo|}(){}
                public void {|#1:_Foo|}(){}

            }";

            var expected1 = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("foo");
            var expected2 = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(1).WithArguments("_Foo");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task MethodProtectedWarningTest()
        {
            var test = @"
            public class Test
            {   
                private void {|#0:foo|}(){}
                private void {|#1:_Foo|}(){}

            }";

            var expected1 = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(0).WithArguments("foo");
            var expected2 = VerifyCS.Diagnostic("TaleworldsCodeAnalysis").WithLocation(1).WithArguments("_Foo");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

    }
}
