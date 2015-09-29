using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using simulatedTrading;

namespace fxCoreLinkTests
{
    [TestClass]
    public class TxnSimulatorTests
    {
        string pairAU = "AUD/USD";
        string pairAJ = "AUD/JPY";
        double entryPriceAU = 0.6930;
        double entryPriceAJ = 82.50;

        TransactionManager txm = new TransactionManager();

        public TxnSimulatorTests()
        {
            txm.getOrder().createMarketOrder(pairAU, new DateTime(2015, 1, 1, 1, 1, 1, 0), "BUY", 1000, entryPriceAU, "id-1", 20, 25);
            txm.getOrder().createMarketOrder(pairAJ, new DateTime(2015, 1, 1, 1, 1, 1, 0), "SELL", 1000, entryPriceAJ, "id-1", 20, 25);
        }
        [TestMethod]
        public void TestHitLimit()
        {

            for (int i = 0; i < 59; i++)
            {
                DateTime nowDt = new DateTime(2015, 1, 1, 1, 2, i);
                double incr = (double)i / 10000;
                double price = entryPriceAU + incr;
                txm.priceUpdate(pairAU, nowDt, price);
            }

            double pips = (double)txm.getClosedTrade().getClosedPositionGross(pairAU)["pips"];
            Assert.IsTrue(24<pips && pips<26, "Limit close");
            Console.WriteLine("Gross for {0} is {1}", pairAU, pips);
        }
        [TestMethod]
        public void TestHitStop()
        {

            for (int i = 0; i < 59; i++)
            {
                DateTime nowDt = new DateTime(2015, 1, 1, 1, 2, i);
                double incr = (double)-i / 10000;
                double price = entryPriceAU + incr;
                txm.priceUpdate(pairAU, nowDt, price);
            }

            double pips = (double)txm.getClosedTrade().getClosedPositionGross(pairAU)["pips"];
            Assert.AreEqual(-20.0, Math.Round(pips, 5), "Stop close");
            Console.WriteLine("Gross for {0} is {1}", pairAU, pips);
        }

        [TestMethod]
        public void TestDelEntryAfterOpenOpposingLongPosition()
        {
            double entryPrice2 = 0.6970;

            txm.getOrder().createMarketOrder(pairAU, new DateTime(2015, 1, 1, 1, 3, 1, 0), "SELL", 1000, entryPrice2, "id-1");
            double pips = (double)txm.getClosedTrade().getClosedPositionGross(pairAU)["pips"];
            Assert.AreEqual(40.0, Math.Round(pips, 5), "Opposing long");
            Assert.AreEqual(0, txm.getOrder().getOrderCount(pairAU));
            Assert.AreEqual(2, txm.getOrder().getOrderCount(pairAJ));
            Console.WriteLine("Gross for {0} is {1}", pairAU, pips);
        }
        [TestMethod]
        public void TestDelEntryAfterOpenOpposingShortPosition()
        {
            double entryPrice2 = 82.10;

            txm.getOrder().createMarketOrder(pairAJ, new DateTime(2015, 1, 1, 1, 3, 1, 0), "BUY", 1000, entryPrice2, "id-1");
            double pips = (double)txm.getClosedTrade().getClosedPositionGross(pairAJ)["pips"];
            Assert.AreEqual(40.0, Math.Round(pips, 3), "Opposing short");
            Assert.AreEqual(0, txm.getOrder().getOrderCount(pairAJ));
            Assert.AreEqual(2, txm.getOrder().getOrderCount(pairAU));
            Console.WriteLine("Gross for {0} is {1}", pairAJ, pips);
        }
        [TestMethod]
        public void TestDelEntryAfterOpenOpposingShortPosition2()
        {
            double entryPrice2 = 82.10;

            txm.getOrder().createMarketOrder(pairAJ, new DateTime(2015, 1, 1, 1, 3, 1, 0), "BUY", 1000, entryPrice2, "id-1", 20, 0);
            double pips = (double)txm.getClosedTrade().getClosedPositionGross(pairAJ)["pips"];
            Assert.AreEqual(40.0, Math.Round(pips, 3), "Opposing short");
            Assert.AreEqual(0, txm.getOrder().getOrderCount(pairAJ));
            Assert.AreEqual(2, txm.getOrder().getOrderCount(pairAU));
            Console.WriteLine("Gross for {0} is {1}", pairAJ, pips);
        }
    }
}
