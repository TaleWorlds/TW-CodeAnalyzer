using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = TaleworldsCodeAnalysis.Test.CSharpCodeFixVerifier<
    TaleworldsCodeAnalysis.NameChecker.PropertyNameChecker,
    TaleworldsCodeAnalysis.TaleworldsCodeAnalysisCodeFixProvider>;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class PropertyNameUnitTests
    {

        [TestMethod]
        public async Task PropertyNoWarningTest()
        {
            var test = @"
            public class Test
            {   
                public int ValuePub { get => _valueLoc; }
                private int _valuePriv { get => _valueLoc; }
                protected int ValuePro { get => _valueLoc; }
                internal int _valueInt { get => _valueLoc; }

                private int _valueLoc;
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task PropertyPrivateWarningTest()
        {
            var test = @"
            public class Test
            {   
                private int {|#0:Value|} {get => _value;}
                private int {|#1:value|} {get => _value;}
                private int {|#2:value_|} {get => _value;}

                private int _value;
            }";

            var expected1 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(0).WithArguments("Value","Private","_uscoreCase");
            var expected2 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(1).WithArguments("value", "Private", "_uscoreCase");
            var expected3 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(2).WithArguments("value_", "Private", "_uscoreCase");
            await VerifyCS.VerifyAnalyzerAsync(test,expected1,expected2,expected3);
        }

        [TestMethod]
        public async Task PropertyPublicWarning()
        {
            var test = @"
            public class Test
            {   
                public int {|#0:_valueProp|} {get => _value;}
                public int {|#1:value|} {get => _value;}

                private int _value;
            }";
            var expected1 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(0).WithArguments("_valueProp", "Public", "PascalCase");
            var expected2 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(1).WithArguments("value", "Public", "PascalCase");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }

        [TestMethod]
        public async Task PropertyInternalWarning()
        {
            var test = @"
            public class Test
            {   
                internal int {|#0:Value|} {get => _value;}
                internal int {|#1:value|} {get => _value;}

                private int _value;
            }";
            var expected1 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(0).WithArguments("Value", "Internal", "_uscoreCase");
            var expected2 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(1).WithArguments("value", "Internal", "_uscoreCase");
            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2); 
        }

        [TestMethod]
        public async Task PropertyProtectedWarning()
        {
            var test = @"
            public class Test
            {   
                protected int {|#0:_valueProp|} {get => _value;}
                protected int {|#1:value|} {get => _value;}

                private int _value;
            }";
            var expected1 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(0).WithArguments("_valueProp", "Protected", "PascalCase");
            var expected2 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(1).WithArguments("value", "Protected", "PascalCase");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
        }
    }

    
}
