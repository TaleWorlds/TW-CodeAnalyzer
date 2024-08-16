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

        [TestMethod]
        public void ForbiddenPiecesTest()
        {
            Assert.IsTrue(NameCheckerLibrary.GetForbiddenPieces("TaleWorlds", ConventionType.PascalCase).Count == 0);
            Assert.IsTrue(NameCheckerLibrary.GetForbiddenPieces("AITaleworlds", ConventionType.PascalCase)[0] == "AI");
            Assert.IsTrue(NameCheckerLibrary.GetForbiddenPieces("ABTaleWorlds", ConventionType.PascalCase)[0] == "AB");
            Assert.IsTrue(NameCheckerLibrary.GetForbiddenPieces("TaleWorldsABTaleWorlds", ConventionType.PascalCase)[0] == "AB");
            Assert.IsTrue(NameCheckerLibrary.GetForbiddenPieces("TaleWorldsAB", ConventionType.PascalCase)[0] == "AB");
            Assert.IsTrue(NameCheckerLibrary.GetForbiddenPieces("TaleWorldsABTaleWorldsAI", ConventionType.PascalCase)[0] == "AB");
            Assert.IsTrue(NameCheckerLibrary.GetForbiddenPieces("TaleWorldsABTaleWorldsAI", ConventionType.PascalCase)[1] == "AI");
        }
    }
}
