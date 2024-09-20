using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.Inheritance;

namespace TaleworldsCodeAnalysis.Test.OtherCheckers
{
    [TestClass]
    public class FindAnalyzerTests
    {
        [TestMethod]
        public void NoError()
        {
            var findAnalyzers=FindAnalyzers.Instance;
        }
    }
}
