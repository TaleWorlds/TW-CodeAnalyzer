using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.NameChecker.MethodNameChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;


namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class MethodNameUnitTests
    {

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
        public async Task MethodPrivateInternalWarningTest()
        {
            var test = @"
            public class Test
            {   
                private void {|#0:fooPriv|}(){}
                private void {|#1:_FooPriv|}(){}
                internal void {|#2:fooInt|}(){}
                internal void {|#3:_FooInt|}(){}

            }"
            ;

            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("MethodNameChecker").WithLocation(0).WithArguments("fooPriv", "Private", "_uscoreCase"),
                VerifyCS.Diagnostic("MethodNameChecker").WithLocation(1).WithArguments("_FooPriv", "Private", "_uscoreCase"),
                VerifyCS.Diagnostic("MethodNameChecker").WithLocation(2).WithArguments("fooInt", "Internal", "_uscoreCase"),
                VerifyCS.Diagnostic("MethodNameChecker").WithLocation(3).WithArguments("_FooInt", "Internal", "_uscoreCase")
            };
            
            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }

        [TestMethod]
        public async Task MethodPublicProtectedWarningTest()
        {
            var test = @"
            public class Test
            {   
                public void {|#0:fooPub|}(){}
                public void {|#1:_FooPub|}(){}

                protected void {|#2:fooPro|}(){}
                protected void {|#3:_FooPro|}(){}
            }";

            var expectedResults = new DiagnosticResult[]
            {
                VerifyCS.Diagnostic("MethodNameChecker").WithLocation(0).WithArguments("fooPub", "Public", "PascalCase"),
                VerifyCS.Diagnostic("MethodNameChecker").WithLocation(1).WithArguments("_FooPub", "Public", "PascalCase"),
                VerifyCS.Diagnostic("MethodNameChecker").WithLocation(2).WithArguments("fooPro", "Protected", "PascalCase"),
                VerifyCS.Diagnostic("MethodNameChecker").WithLocation(3).WithArguments("_FooPro", "Protected", "PascalCase")
        };

            await VerifyCS.VerifyAnalyzerAsync(test, expectedResults);
        }

    }
}
