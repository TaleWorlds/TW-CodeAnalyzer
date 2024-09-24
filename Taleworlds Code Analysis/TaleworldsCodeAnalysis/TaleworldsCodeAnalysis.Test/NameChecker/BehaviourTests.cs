using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TaleworldsCodeAnalysis.NameChecker;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis.Test.NameChecker
{
    [TestClass]
    public class BehaviourTests
    {
        [TestInitialize]
        public void Init()
        {
            WhiteListParser.Instance.EnableTesting();
            WhiteListParser.Instance.UpdateWhiteList();
        }

        [TestMethod]
        public void CamelFixThisTest()
        {
            Assert.IsTrue(CamelCaseBehaviour.Instance.FixThis("TaleWorlds")=="taleWorlds");
            Assert.IsTrue(CamelCaseBehaviour.Instance.FixThis("TALEWorlds") == "taleWorlds");
            Assert.IsTrue(CamelCaseBehaviour.Instance.FixThis("TaleTWorlds") == "taleTWorlds");
        }

        [TestMethod]
        public void CamelIsMatching()
        {
            Assert.IsTrue(!CamelCaseBehaviour.Instance.IsMatching("TaleWorlds"));
            Assert.IsTrue(CamelCaseBehaviour.Instance.IsMatching("codeFixTW"));
            Assert.IsTrue(CamelCaseBehaviour.Instance.IsMatching("user1Name"));
            Assert.IsTrue(!CamelCaseBehaviour.Instance.IsMatching("hello_world"));
            Assert.IsTrue(!CamelCaseBehaviour.Instance.IsMatching("user@name"));
            Assert.IsTrue(CamelCaseBehaviour.Instance.IsMatching("myVarName123"));
        }

        [TestMethod]
        public void CamelWhiteList()
        {
        }

        [TestMethod]
        public void PascalIsMatching()
        {
            Assert.IsTrue(PascalCaseBehaviour.Instance.IsMatching("HelloWorld"));
            Assert.IsTrue(PascalCaseBehaviour.Instance.IsMatching("User1Name"));
            Assert.IsTrue(!PascalCaseBehaviour.Instance.IsMatching("helloworld"));
            Assert.IsTrue(!PascalCaseBehaviour.Instance.IsMatching("HELLOWORLD"));
            Assert.IsTrue(!PascalCaseBehaviour.Instance.IsMatching("Hello_World"));
            Assert.IsTrue(!PascalCaseBehaviour.Instance.IsMatching("helloWorld"));
            Assert.IsTrue(PascalCaseBehaviour.Instance.IsMatching("MyVariableName"));
            Assert.IsTrue(!PascalCaseBehaviour.Instance.IsMatching("User@Name"));
            Assert.IsTrue(PascalCaseBehaviour.Instance.IsMatching("MyVarName123"));
        }
        
        [TestMethod]
        public void UnderScorIsMatching()
        {
            Assert.IsTrue(UnderScoreCaseBehaviour.Instance.IsMatching("_helloWorld"));
            Assert.IsTrue(UnderScoreCaseBehaviour.Instance.IsMatching("_user1Name"));
            Assert.IsTrue(UnderScoreCaseBehaviour.Instance.IsMatching("_helloworld"));
            Assert.IsTrue(!UnderScoreCaseBehaviour.Instance.IsMatching("_HELLOWORLD"));
            Assert.IsTrue(!UnderScoreCaseBehaviour.Instance.IsMatching("_hello@world"));
            Assert.IsTrue(!UnderScoreCaseBehaviour.Instance.IsMatching("_helloWorld_"));
            Assert.IsTrue(!UnderScoreCaseBehaviour.Instance.IsMatching("__helloWorld"));
            Assert.IsTrue(!UnderScoreCaseBehaviour.Instance.IsMatching("_My_Var_Name"));
        }

    }

}
