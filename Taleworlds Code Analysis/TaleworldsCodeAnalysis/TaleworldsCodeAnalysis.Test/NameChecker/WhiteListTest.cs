using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class WhiteListTest
    {
        [TestMethod]
        public void WhiteListTestCases()
        {
            Assert.IsTrue(WhiteListParser.Instance.WhiteListWords.Contains("ASAP"));
            Assert.IsTrue(WhiteListParser.Instance.WhiteListWords.Contains("TW"));
            Assert.IsTrue(WhiteListParser.Instance.WhiteListWords.Contains("AI"));
        }
    }
}
