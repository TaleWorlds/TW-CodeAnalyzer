using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class NameCheckerLibraryTests
    {
        [TestMethod]
        public void CamelCaseTest()
        {
            var checkResults = new bool[]
            {
                NameCheckerLibrary.IsCamelCase("camelCase"),
                NameCheckerLibrary.IsCamelCase("camel"),
                NameCheckerLibrary.IsCamelCase("ca"),
                NameCheckerLibrary.IsCamelCase("_camelCase"),
                NameCheckerLibrary.IsCamelCase("CamelCase"),
            };
            var expectedResults = new bool[] {
                true,
                true,
                true,
                false,
                false
            };

            for (int i = 0; i < checkResults.Length; i++)
            {
                Assert.AreEqual(checkResults[i], expectedResults[i]);
            }
        }

        [TestMethod]
        public void PascalCaseTest()
        {
            var checkResults = new bool[]
            {
                NameCheckerLibrary.IsPascalCase("PASCAL"),
                NameCheckerLibrary.IsPascalCase("Pascal"),
                NameCheckerLibrary.IsPascalCase("PascalCase"),
                NameCheckerLibrary.IsPascalCase("PAscal"),
                NameCheckerLibrary.IsPascalCase("_pascal")
            };
            var expectedResults = new bool[] {
                false,
                true,
                true,
                false,
                false
            };

            for (int i = 0; i < checkResults.Length; i++)
            {
                Assert.AreEqual(checkResults[i], expectedResults[i]);
            }
        }

        [TestMethod]
        public void UnderScoreCase()
        {
            var checkResults = new bool[]
            {
                NameCheckerLibrary.IsUnderScoreCase("_uscorCase"),
                NameCheckerLibrary.IsUnderScoreCase("_uscorecase"),
                NameCheckerLibrary.IsUnderScoreCase("uscoreCase"),
                NameCheckerLibrary.IsUnderScoreCase("UscoreCase"),
                NameCheckerLibrary.IsUnderScoreCase("_uscore")
            };
            var expectedResults = new bool[] {
                true,
                true,
                false,
                false,
                true
            };

            for (int i = 0; i < checkResults.Length; i++)
            {
                Assert.AreEqual(checkResults[i], expectedResults[i]);
            }
        }
    }
}
