using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using simulatedTrading;

namespace fxCoreLinkTests
{
    [TestClass]
    public class SimulatorTests
    {
        ClosedTrade closedTrade = new ClosedTrade();
        Position position;
        Order order;
        string pairAU = "AUD/USD";
        string pairAJ = "AUD/JPY";
        double entryPriceAU = 0.6930;
        double entryPriceAJ = 82.50;
        public SimulatorTests()
        {
            position = new Position(closedTrade);
            order = new Order(closedTrade, position);

            position.createPosition(pairAU, new DateTime(2015, 1, 1, 1, 1, 1, 0), "BUY", 1000, entryPriceAU, "id-1", "custom-1");
            order.createEntryOrder(pairAU, new DateTime(2015, 1, 1, 1, 1, 1, 1), "SELL", "STOP", 1000, 0.6910, "id-1");
            order.createEntryOrder(pairAU, new DateTime(2015, 1, 1, 1, 1, 1, 2), "SELL", "LIMIT", 1000, 0.6955, "id-1");

            position.createPosition(pairAJ, new DateTime(2015, 1, 1, 1, 1, 1, 0), "SELL", 1000, entryPriceAJ, "id-1", "custom-1");
            order.createEntryOrder(pairAJ, new DateTime(2015, 1, 1, 1, 1, 1, 1), "BUY", "STOP", 1000, 82.70, "id-1");
            order.createEntryOrder(pairAJ, new DateTime(2015, 1, 1, 1, 1, 1, 2), "BUY", "LIMIT", 1000, 82.25, "id-1");
        }

        [TestMethod]
        public void TestHitLimit()
        {

            for (int i = 0; i < 27; i++)
            {
                DateTime nowDt = new DateTime(2015, 1, 1, 1, 2, i);
                double incr = (double)i / 10000;
                double price = entryPriceAU + incr;
                order.processOrders(pairAU, nowDt, price, price); // no spread for testing
            }

            double pips = closedTrade.getClosedPositionGross(pairAU);
            Assert.AreEqual( 26.0, Math.Round(pips, 5), "Limit close");
            Console.WriteLine("Gross for {0} is {1}", pairAU, pips);
        }
        [TestMethod]
        public void TestHitStop()
        {

            for (int i = 0; i < 21; i++)
            {
                DateTime nowDt = new DateTime(2015, 1, 1, 1, 2, i);
                double incr = (double)-i / 10000;
                double price = entryPriceAU + incr;
                order.processOrders(pairAU, nowDt, price, price); // no spread for testing
            }

            double pips = closedTrade.getClosedPositionGross(pairAU);
            Assert.AreEqual(-20.0, Math.Round(pips, 5), "Stop close");
            Console.WriteLine("Gross for {0} is {1}", pairAU, pips);
        }
        [TestMethod]
        public void TestOpenOpposingLongPosition()
        {
            double entryPrice2 = 0.6970;

            position.createPosition(pairAU, new DateTime(2015, 1, 1, 1, 3, 1, 0), "SELL", 1000, entryPrice2, "id-1", "custom-1");
            double pips = closedTrade.getClosedPositionGross(pairAU);
            Assert.AreEqual(40.0, Math.Round(pips, 5), "Opposing long");
            Console.WriteLine("Gross for {0} is {1}", pairAU, pips);
        }
        [TestMethod]
        public void TestOpenOpposingShortPosition()
        {
            double entryPrice2 = 82.10;

            position.createPosition(pairAJ, new DateTime(2015, 1, 1, 1, 3, 1, 0), "BUY", 1000, entryPrice2, "id-1", "custom-1");
            double pips = closedTrade.getClosedPositionGross(pairAJ);
            Assert.AreEqual(40.0, Math.Round(pips,3), "Opposing short");
            Console.WriteLine("Gross for {0} is {1}", pairAJ, pips);
        }
    }
}
