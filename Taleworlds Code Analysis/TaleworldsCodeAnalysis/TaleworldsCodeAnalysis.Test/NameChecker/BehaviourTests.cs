﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        }

        [TestMethod]
        public void CamelWhiteList()
        {
        }

    }

}
