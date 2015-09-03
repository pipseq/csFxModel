using Microsoft.VisualStudio.TestTools.UnitTesting;
using fxCoreLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace fxCoreLink.Tests
{
    [TestClass()]
    public class AccumulatorMgrTests
    {
        [TestMethod()]
        public void rollTest()
        {
            AccumulatorMgr accumulatorMgr = new AccumulatorMgr();
            accumulatorMgr.Snapshot = "testdata";
            accumulatorMgr.read();  // load history

            string before = accumulatorMgr.writeString();
            accumulatorMgr.roll(new DateTime(2000,1,1,0,0,0));
            string after = accumulatorMgr.writeString();

            string beforeCmp = File.ReadAllText("testdata.json");
            string afterCmp = File.ReadAllText("dataMgtTestAfter.json");

            Assert.AreEqual(before.Trim(), beforeCmp.Trim(), "roll failed");

            Assert.AreEqual(after.Trim(), afterCmp.Trim(), "roll failed");

        }
    }
}