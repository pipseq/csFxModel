using Microsoft.VisualStudio.TestTools.UnitTesting;
using fxCoreLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;

namespace fxCoreLink.Tests
{
    [TestClass()]
    public class AccumulatorMgrTests
    {

        Logger log = Logger.LogManager("AccumulatorMgrTests");
        string pair = "AUD/JPY";

        [TestMethod()]
        public void rollTest()
        {
            AccumulatorMgr accumulatorMgr = new AccumulatorMgr();
            accumulatorMgr.Debug = false;
            accumulatorMgr.Snapshot = "..\\..\\testdata1";
            accumulatorMgr.read();  // load history

            for (int m = 1; m <= 2; m++)    // days
                for (int k = 0; k < 24; k++)    // hours
                    for (int j = 0; j < 60; j++)    // minutes
                    {
                        DateTime now = new DateTime();
                        for (int i = 1; i < 3; i++) // two data elements per minute period
                        {
                            now = new DateTime(2000, 1, m, k, j, i);
                            accumulatorMgr.accumulate(pair, now,
                                m * 1000000 + k * 10000 + j * 100 + i + 0.1,    // bid
                                m * 1000000 + k * 10000 + j * 100 + i + 0.2);   // ask
                        }
                        accumulatorMgr.roll(now);
                    }

            accumulatorMgr.roll(new DateTime(2000, 1, 1, 0, 1, 0));
            string after = accumulatorMgr.writeString();
            string afterCmp = File.ReadAllText("..\\..\\testdata1Result.json");
            Assert.AreEqual(after.Trim(), afterCmp.Trim(), "roll failed");
        }
    }
}