using Microsoft.VisualStudio.TestTools.UnitTesting;
using fxCoreLink;
using Common;
using Common.fx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simulatedTrading;

namespace fxCoreLinkTests
{

    public class ExpertTest : ExpertBase
    {
        public ExpertTest(string pair, TimeFrame timeframe):base(pair, timeframe) { }

    }

    [TestClass()]
    public class ExpertBaseTests:Common.Display,Common.Control
    {
        AccumulatorMgr accumulatorMgr = new AccumulatorMgr();
        ExpertTest expert = new ExpertTest("AUD/JPY", TimeFrame.m1);
        ExpertFactory expertFactory;
        PriceProcessor priceProcessor;
        IFXManager fxManager;
        FxUpdates fxUpdates;
        int periods = 26;
        double[,] hlcData = new double[,]{
                // high       low        close
                {119.793,   119.710,    119.785}, // newest
                {119.816,   119.697,    119.744},
                {119.904,   119.769,    119.802},
                {119.957,   119.853,    119.888},
                {119.953,   119.835,    119.891},
                {119.930,   119.750,    119.901},
                {119.973,   119.826,    119.930},
                {119.964,   119.819,    119.864},
                {120.136,   119.957,    119.964},
                {120.237,   120.069,    120.136},
                {120.215,   120.123,    120.215},
                {120.325,   120.174,    120.176},
                {120.283,   120.183,    120.252},
                {120.337,   120.084,    120.233},
                {120.126,   120.060,    120.108},
                {120.119,   120.052,    120.081},
                {120.120,   120.021,    120.078},
                {120.154,   120.038,    120.055},
                {120.166,   120.091,    120.142},
                {120.133,   120.034,    120.122},
                {120.111,   120.021,    120.066},
                {120.078,   119.996,    120.034},
                {120.143,   120.038,    120.074},
                {120.180,   120.099,    120.142},
                {120.139,   120.035,    120.101},
                {120.106,   120.009,    120.070},
                {120.207,   120.063,    120.088},
                {120.277,   120.196,    120.206},
                {120.277,   120.183,    120.266},
                {120.287,   120.172,    120.196},
                {120.377,   120.283,    120.285},
                {120.357,   120.271,    120.316},
                {120.406,   120.276,    120.277},
                {120.446,   120.293,    120.372},
                {120.350,   120.264,    120.301},
                {120.389,   120.182,    120.345},
                {120.307,   120.226,    120.276},
                {120.353,   120.284,    120.294},
                {120.314,   120.230,    120.290},
                {120.328,   120.235,    120.274},
                {120.295,   120.179,    120.292},
                {120.249,   120.165,    120.194},
                {120.303,   120.231,    120.249},
                {120.336,   120.222,    120.235},
                {120.309,   120.267,    120.306},
                {120.315,   120.254,    120.301},
                {120.403,   120.297,    120.315},
                {120.427,   120.351,    120.388},
                {120.376,   120.315,    120.360},
                {120.341,   120.294,    120.332},
                {120.349,   120.310,    120.333},
                {120.350,   120.275,    120.345},
                {120.346,   120.311,    120.320},
                {120.330,   120.309,    120.324},
                {120.377,   120.319,    120.320},
                {120.406,   120.366,    120.368},
                {120.386,   120.333,    120.377},
                {120.419,   120.356,    120.363},
                {120.443,   120.393,    120.400},
                {120.467,   120.413,    120.413}, // oldest
        };


        public ExpertBaseTests()
        {
            fxManager = new FXManagerSimulated();
            fxUpdates = new FxUpdates(this, this, fxManager, accumulatorMgr);

            priceProcessor = new PriceProcessor(this, this, fxManager, fxUpdates);
            expertFactory = new ExpertFactory(priceProcessor);

            accumulatorMgr.Snapshot = "testdata";
            accumulatorMgr.read();  // load history

            expertFactory.subscribe(expert);
            expertFactory.subscribePrice(expert);
        }

        enum DataType { hi = 0, lo = 1, close = 2 };
        private List<double> getTestData(DataType dt)
        {
            List<double> ld = new List<double>();
            for (int i = 60 - 1; i >= 0; i--)
            {
                ld.Add(hlcData[i, (int)dt]);
            }
            return ld;
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
            double data = expert.getEMA(TimeFrame.m1, PriceComponent.BidClose, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getEMATest2()
        {
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double result = expert.getEMA(periods, ldc);
            Assert.AreEqual(119.92, Math.Round(result, 2));
        }

        [TestMethod()]
        public void getSMATest()
        {
            double data = expert.getSMA(TimeFrame.m1, PriceComponent.BidClose, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getSMA2Test()
        {
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double result = expert.getSMA(periods, ldc);
            Assert.AreEqual(119.98, Math.Round(result, 2));
        }

        [TestMethod()]
        public void getATRTest()
        {
            double data = expert.getATR(TimeFrame.m1, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getATRTest2()
        {
            List<double> ldh = getTestData(DataType.hi);
            List<double> ldl = getTestData(DataType.lo);
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double result = expert.getATR(periods, ldh, ldl, ldc);
            Assert.AreEqual(0.12047, Math.Round(result,5));
        }

        [TestMethod()]
        public void getMinusDITest()
        {
            double data = expert.getMinusDI(TimeFrame.m1, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getMinusDITest2()
        {
            List<double> ldh = getTestData(DataType.hi);
            List<double> ldl = getTestData(DataType.lo);
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double result = expert.getMinusDI(periods, ldh, ldl, ldc);
            Assert.AreEqual(31.17, Math.Round(result,2));
        }

        [TestMethod()]
        public void getPlusDITest()
        {
            double data = expert.getPlusDI(TimeFrame.m1, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getPlusDITest2()
        {
            List<double> ldh = getTestData(DataType.hi);
            List<double> ldl = getTestData(DataType.lo);
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double result = expert.getPlusDI(periods, ldh, ldl, ldc);
            Assert.AreEqual(11.19, Math.Round(result,2));
        }

        [TestMethod()]
        public void getRSITest()
        {
            double data = expert.getRSI(TimeFrame.m1, PriceComponent.BidClose, periods);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void getRSITest2()
        {
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double result = expert.getRsi(periods, ldc);
            Assert.AreEqual(32.44, Math.Round(result,2));
        }

        [TestMethod()]
        public void getMACDasListTest()
        {
            List<double> data = expert.getMACDasList(TimeFrame.m1, PriceComponent.BidClose, 12, 26, 9);
            Assert.IsNotNull(data);
            Assert.AreEqual(3, data.Count);
        }

        [TestMethod()]
        public void getMACDasListTest2()
        {
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double[] result = expert.getMACD(12, 26, 9, ldc);
            Assert.AreEqual(-0.1044, Math.Round(result[0],4));
            Assert.AreEqual(-0.0780, Math.Round(result[1],4));
            Assert.AreEqual(-0.0264, Math.Round(result[2],4));
        }

        [TestMethod()]
        public void getADXTest()
        {
            double data = expert.getADX(TimeFrame.m1, PriceComponent.BidClose, 13);
            Assert.IsNotNull(data);
        }


        [TestMethod()]
        public void getADXTest2()
        {

            List<double> ldh = getTestData(DataType.hi);
            List<double> ldl = getTestData(DataType.lo);
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double result = expert.getADX(periods, ldh, ldl, ldc);
            Assert.AreEqual(27, Math.Truncate(result));
        }

        [TestMethod()]
        public void getStochTest()
        {
            double[] data = expert.getStoch(TimeFrame.m1, PriceComponent.BidClose, periods);
            Assert.IsNotNull(data);
            Assert.AreEqual(2, data.Count());
        }

        [TestMethod()]
        public void getStochTest2()
        {
            List<double> ldh = getTestData(DataType.hi);
            List<double> ldl = getTestData(DataType.lo);
            List<double> ldc = getTestData(DataType.close);
            int periods = 14;

            double[] result = expert.getStoch(periods, ldh, ldl, ldc);
            Assert.AreEqual(9.98, Math.Round(result[0],2));
            Assert.AreEqual(14.01, Math.Round(result[1],2));
        }

        #region interface implementation
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
        #endregion
    }
}