using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulatedTrading
{
    public class ClosedTrade
    {
        public Dictionary<string, List<Dictionary<string, object>>> mapClosedPosition
            = new Dictionary<string, List<Dictionary<string, object>>>();
        ListenerMgr closedTradeListenerMgr = new ListenerMgr();

        public ClosedTrade() { }

        public void addClosedTradeListener(ClosedTradeListener listener)
        {
            closedTradeListenerMgr.addListener(listener);
        }

        public void createClosedPosition(string pair, DateTime openDt, DateTime closeDt, string BorS, int amount, double openPrice, double closePrice, string orderId, string tradeId, string customId)
        {
            if (!mapClosedPosition.ContainsKey(pair))
            {
                mapClosedPosition.Add(pair, new List<Dictionary<string, object>>());
            }
            double pips = 0;
            if (BorS == "BUY")
            {
                pips = (closePrice - openPrice) / TransactionManager.getPoiintSize(pair);
            }
            else if (BorS == "SELL")
            {
                pips = (openPrice - closePrice) / TransactionManager.getPoiintSize(pair);
            }
            mapClosedPosition[pair].Add(new Dictionary<string, object>());
            int ndx = mapClosedPosition[pair].Count - 1;
            mapClosedPosition[pair][ndx].Add("entry", BorS);
            mapClosedPosition[pair][ndx].Add("amount", amount);
            mapClosedPosition[pair][ndx].Add("openPrice", openPrice);
            mapClosedPosition[pair][ndx].Add("closePrice", closePrice);
            mapClosedPosition[pair][ndx].Add("pips", pips);
            mapClosedPosition[pair][ndx].Add("grossPL", 0.0);
            mapClosedPosition[pair][ndx].Add("customId", customId);
            mapClosedPosition[pair][ndx].Add("orderId", orderId);
            mapClosedPosition[pair][ndx].Add("tradeId", tradeId);
            mapClosedPosition[pair][ndx].Add("openDt", openDt);
            mapClosedPosition[pair][ndx].Add("closeDt", closeDt);

            foreach (Listener l in closedTradeListenerMgr.getListeners())
            {
                ((ClosedTradeListener)l).closedTradeChangeNotification(pair, mapClosedPosition[pair][ndx], StateEvent.Create);
            }
        }

        public double getClosedPositionGross(string pair)
        {
            double pips = 0.0;

            if (mapClosedPosition.ContainsKey(pair))
            foreach (Dictionary<string, object> map in mapClosedPosition[pair])
            {
                pips += (double) map["pips"];
            }

            return pips;
        }
    }
}
