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
                NameCheckerLibrary.IsMatchingConvention("camelCase",ConventionType.camelCase),
                NameCheckerLibrary.IsMatchingConvention("camel", ConventionType.camelCase),
                NameCheckerLibrary.IsMatchingConvention("ca", ConventionType.camelCase),
                NameCheckerLibrary.IsMatchingConvention("_camelCase", ConventionType.camelCase),
                NameCheckerLibrary.IsMatchingConvention("CamelCase", ConventionType.camelCase),
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
                NameCheckerLibrary.IsMatchingConvention("PASCAL", ConventionType.PascalCase),
                NameCheckerLibrary.IsMatchingConvention("Pascal", ConventionType.PascalCase),
                NameCheckerLibrary.IsMatchingConvention("PascalCase", ConventionType.PascalCase),
                NameCheckerLibrary.IsMatchingConvention("PAscal", ConventionType.PascalCase),
                NameCheckerLibrary.IsMatchingConvention("_pascal", ConventionType.PascalCase)
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
                NameCheckerLibrary.IsMatchingConvention("_uscorCase", ConventionType._uscoreCase),
                NameCheckerLibrary.IsMatchingConvention("_uscorecase", ConventionType._uscoreCase),
                NameCheckerLibrary.IsMatchingConvention("uscoreCase", ConventionType._uscoreCase),
                NameCheckerLibrary.IsMatchingConvention("UscoreCase", ConventionType._uscoreCase),
                NameCheckerLibrary.IsMatchingConvention("_uscore", ConventionType._uscoreCase),
                NameCheckerLibrary.IsMatchingConvention("_uscoreAI", ConventionType._uscoreCase),
                NameCheckerLibrary.IsMatchingConvention("_uscoreAITaleworlds", ConventionType._uscoreCase),
                NameCheckerLibrary.IsMatchingConvention("_xabASDAS", ConventionType.camelCase)

            };
            var expectedResults = new bool[] {
                true,
                true,
                false,
                false,
                true,
                true,
                true,
                false
            };

            for (int i = 0; i < checkResults.Length; i++)
            {
                Assert.AreEqual(expectedResults[i], checkResults[i]);
            }
        }
    }
}
