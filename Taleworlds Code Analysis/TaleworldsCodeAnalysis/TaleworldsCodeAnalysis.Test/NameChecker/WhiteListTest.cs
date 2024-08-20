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

        [TestMethod]
        public void ForbiddenPiecesTest()
        {
            WhiteListParser.Instance.UpdateWhiteList(new System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.AdditionalText>());
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("TaleWorlds", ConventionType.PascalCase).Count == 0);
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("AITaleworlds", ConventionType.PascalCase).Count==0);
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("ABTaleWorlds", ConventionType.PascalCase)[0] == "AB");
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("TaleWorldsABTaleWorlds", ConventionType.PascalCase)[0] == "AB");
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("TaleWorldsAB", ConventionType.PascalCase)[0] == "AB");
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("TaleWorldsABTaleWorldsAI", ConventionType.PascalCase)[0] == "AB");
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("TaleWorldsABTaleWorldsAC", ConventionType.PascalCase)[1] ==  "AC");
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("my_variable_name", ConventionType.camelCase).Count==0);
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("ootable", ConventionType.PascalCase).Count==0);
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("MyAITW", ConventionType.PascalCase).Count==0);
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("MyAITW", ConventionType.PascalCase).Count == 0);
            Assert.IsTrue(NameCheckerLibrary.GetNewWhiteListItemsToFix("IPCClass", ConventionType.IPascalCase).Count() == 1);
        }
    }
}
