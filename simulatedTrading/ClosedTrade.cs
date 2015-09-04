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

        public ClosedTrade() { }

        public void createClosedPosition(string pair, DateTime openDt, DateTime closeDt, string BorS, int amount, double openPrice, double closePrice, string customId)
        {
            if (!mapClosedPosition.ContainsKey(pair))
            {
                mapClosedPosition.Add(pair, new List<Dictionary<string, object>>());
            }
            double pointSize = 0.0001;
            if (pair.Contains("JPY"))
                pointSize = 0.01;
            double pips = 0;
            if (BorS == "BUY")
            {
                pips = (closePrice - openPrice) / pointSize;
            }
            else if (BorS == "SELL")
            {
                pips = (openPrice - closePrice) / pointSize;
            }
            mapClosedPosition[pair].Add(new Dictionary<string, object>());
            int ndx = mapClosedPosition[pair].Count - 1;
            mapClosedPosition[pair][ndx].Add("entry", BorS);
            mapClosedPosition[pair][ndx].Add("amount", amount);
            mapClosedPosition[pair][ndx].Add("openPrice", openPrice);
            mapClosedPosition[pair][ndx].Add("closePrice", closePrice);
            mapClosedPosition[pair][ndx].Add("pips", pips);
            mapClosedPosition[pair][ndx].Add("customId", customId);
            mapClosedPosition[pair][ndx].Add("openDt", openDt);
            mapClosedPosition[pair][ndx].Add("closeDt", closeDt);
        }

        public double getClosedPositionGross(string pair)
        {
            double pips = 0.0;

            foreach (Dictionary<string, object> map in mapClosedPosition[pair])
            {
                pips += (double) map["pips"];
            }

            return pips;
        }
    }
}
