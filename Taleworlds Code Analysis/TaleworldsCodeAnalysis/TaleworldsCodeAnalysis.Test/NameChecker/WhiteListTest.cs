using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class WhiteListTest
    {
        [TestMethod]
        public void WhiteListTestCases()
        {
            WhiteListParser.Instance.UpdateWhiteList(new System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.AdditionalText>());
            Assert.IsTrue(WhiteListParser.Instance.WhiteListWords.Contains("ASAP"));
            Assert.IsTrue(WhiteListParser.Instance.WhiteListWords.Contains("TW"));
            Assert.IsTrue(WhiteListParser.Instance.WhiteListWords.Contains("AI"));
        }

       
    }
}
