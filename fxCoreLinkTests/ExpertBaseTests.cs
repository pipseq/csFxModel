using Microsoft.VisualStudio.TestTools.UnitTesting;
using fxCoreLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fxCoreLink.Tests
{

    class ExpertTest : ExpertBase
    {
        public ExpertTest(string pair, TimeFrame timeframe):base(pair, timeframe) { }

    }

    [TestClass()]
    public class ExpertBaseTests:fxCoreLink.Display,fxCoreLink.Control
    {
        AccumulatorMgr accumulatorMgr = new AccumulatorMgr();
        ExpertTest expert = new ExpertTest("AUD/JPY", TimeFrame.m1);
        ExpertFactory expertFactory;
        PriceProcessor priceProcessor;
        IFXManager fxManager;
        FxUpdates fxUpdates;
        int periods = 26;

        public ExpertBaseTests()
        {
            fxManager = new SimFXManager();
            fxUpdates = new FxUpdates(this, this, fxManager, accumulatorMgr);

            priceProcessor = new PriceProcessor(this, this, fxManager, fxUpdates);
            expertFactory = new ExpertFactory(priceProcessor);

            accumulatorMgr.Snapshot = "testdata";
            accumulatorMgr.read();  // load history

            expertFactory.subscribe(expert);
            expertFactory.subscribePrice(expert);
        }

        [TestMethod()]
        public void isEnoughDataTest()
        {
            bool result = expert.isEnoughData(TimeFrame.m1,PriceComponent.BidClose, periods);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void getLastTest()
        {
            double data = expert.getLast(TimeFrame.m1, PriceComponent.BidClose);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getListTest()
        {
            List<double> data = expert.getList(TimeFrame.m1, PriceComponent.BidClose);
            Assert.IsNotNull(data);
            Assert.AreEqual(40, data.Count);
        }

        [TestMethod()]
        public void getEMATest()
        {
            double data = expert.getEMA(TimeFrame.m1, PriceComponent.BidClose,periods);
            Assert.IsNotNull(data);
        }
        [TestMethod()]
        public void getSMATest()
        {
            double data = expert.getSMA(TimeFrame.m1, PriceComponent.BidClose, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getATRTest()
        {
            double data = expert.getATR(TimeFrame.m1, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getMinusDITest()
        {
            double data = expert.getMinusDI(TimeFrame.m1, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getPlusDITest()
        {
            double data = expert.getPlusDI(TimeFrame.m1, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getRSITest()
        {
            double data = expert.getRSI(TimeFrame.m1, PriceComponent.BidClose, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getMACDasListTest()
        {
            List<double> data = expert.getMACDasList(TimeFrame.m1, PriceComponent.BidClose, 12,26,9);
            Assert.IsNotNull(data);
            Assert.AreEqual(3, data.Count);
        }

        [TestMethod()]
        public void getADXTest()
        {
            double data = expert.getADX(TimeFrame.m1, PriceComponent.BidClose, 13);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getStochTest()
        {
            double[] data = expert.getStoch(TimeFrame.m1, PriceComponent.BidClose, periods);
            Assert.IsNotNull(data);
            Assert.AreEqual(2, data.Count());
        }

        public void listUpdate(List<string> list)
        {
            throw new NotImplementedException();
        }

        public void display(string s)
        {
            throw new NotImplementedException();
        }

        public void append(string s)
        {
            throw new NotImplementedException();
        }

        public void appendLine(string s)
        {
            throw new NotImplementedException();
        }

        public void appendLine(string s, params object[] values)
        {
            throw new NotImplementedException();
        }

        public void setStatus(string s)
        {
            throw new NotImplementedException();
        }

        public void logger(string s)
        {
            throw new NotImplementedException();
        }

        public bool ordersArmed()
        {
            throw new NotImplementedException();
        }

        public bool isExcludedPair(string pair)
        {
            throw new NotImplementedException();
        }

        public bool isProcessingAllowed()
        {
            throw new NotImplementedException();
        }

        public bool isErrorState()
        {
            throw new NotImplementedException();
        }

        public bool isValidPair(string pair)
        {
            throw new NotImplementedException();
        }

        public bool isJournalRead()
        {
            throw new NotImplementedException();
        }

        public bool isJournalWrite()
        {
            throw new NotImplementedException();
        }

        public string getProperty(string name, string dflt)
        {
            throw new NotImplementedException();
        }

        public bool getBoolProperty(string name, bool dflt)
        {
            throw new NotImplementedException();
        }

        public int getIntProperty(string name, int dflt)
        {
            throw new NotImplementedException();
        }

        public double getDoubleProperty(string name, double dflt)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> getPairParams(string pair)
        {
            throw new NotImplementedException();
        }

        public void journalReadDone()
        {
            //throw new NotImplementedException();
        }
    }
}