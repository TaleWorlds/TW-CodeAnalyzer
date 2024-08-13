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
        public async Task PropertyPrivateInternalWarningTest()
        {
            var test = @"
            public class Test
            {   
                private int {|#0:ValuePriv|} {get => _value;}
                private int {|#1:valuePriv|} {get => _value;}
                private int {|#2:value_Priv|} {get => _value;}
                internal int {|#3:ValueInt|} {get => _value;}
                internal int {|#4:valueInt|} {get => _value;}

                private int _value;
            }";

            var expected1 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(0).WithArguments("ValuePriv", "Private","_uscoreCase");
            var expected2 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(1).WithArguments("valuePriv", "Private", "_uscoreCase");
            var expected3 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(2).WithArguments("value_Priv", "Private", "_uscoreCase");
            var expected4 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(3).WithArguments("ValueInt", "Internal", "_uscoreCase");
            var expected5 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(4).WithArguments("valueInt", "Internal", "_uscoreCase");
            await VerifyCS.VerifyAnalyzerAsync(test,expected1,expected2,expected3,expected4,expected5);
        }

        [TestMethod]
        public async Task PropertyPublicProtWarning()
        {
            var test = @"
            public class Test
            {   
                public int {|#0:_valuePub|} {get => _value;}
                public int {|#1:valuePub|} {get => _value;}
                protected int {|#2:_valueProp|} {get => _value;}
                protected int {|#3:valueProp|} {get => _value;}

                private int _value;
            }";
            var expected1 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(0).WithArguments("_valuePub", "Public", "PascalCase");
            var expected2 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(1).WithArguments("valuePub", "Public", "PascalCase");
            var expected3 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(2).WithArguments("_valueProp", "Protected", "PascalCase");
            var expected4 = VerifyCS.Diagnostic("PropertyNameChecker").WithLocation(3).WithArguments("valueProp", "Protected", "PascalCase");

            await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2,expected3,expected4);
        }

    }

    
}
