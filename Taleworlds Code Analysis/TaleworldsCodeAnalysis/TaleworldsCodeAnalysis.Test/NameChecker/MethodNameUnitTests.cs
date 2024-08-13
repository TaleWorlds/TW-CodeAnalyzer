﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public async Task MethodPrivateWarningTest()
        {
            var test = @"
            public class Test
            {   
                private void {|#0:foo|}(){}
                private void {|#1:_Foo|}(){}

            }"
            ;

            var expected1 = VerifyCS.Diagnostic("MethodNameChecker").WithLocation(0).WithArguments("foo", "Private", "_uscoreCase");
            var expected2 = VerifyCS.Diagnostic("MethodNameChecker").WithLocation(1).WithArguments("_Foo", "Private", "_uscoreCase");
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

            var expected1 = VerifyCS.Diagnostic("MethodNameChecker").WithLocation(0).WithArguments("foo", "Internal", "_uscoreCase");
            var expected2 = VerifyCS.Diagnostic("MethodNameChecker").WithLocation(1).WithArguments("_Foo", "Internal", "_uscoreCase");
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

            var expected1 = VerifyCS.Diagnostic("MethodNameChecker").WithLocation(0).WithArguments("foo", "Public", "PascalCase");
            var expected2 = VerifyCS.Diagnostic("MethodNameChecker").WithLocation(1).WithArguments("_Foo", "Public", "PascalCase");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task MethodProtectedWarningTest()
        {
            var test = @"
            public class Test
            {   
                protected void {|#0:foo|}(){}
                protected void {|#1:_Foo|}(){}

            }";

            var expected1 = VerifyCS.Diagnostic("MethodNameChecker").WithLocation(0).WithArguments("foo", "Protected", "PascalCase");
            var expected2 = VerifyCS.Diagnostic("MethodNameChecker").WithLocation(1).WithArguments("_Foo", "Protected", "PascalCase");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }
    }
}
