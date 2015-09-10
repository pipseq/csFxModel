using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulatedTrading
{
    public class Position
    {
        public Dictionary<string, List<Dictionary<string, object>>> mapPosition
            = new Dictionary<string, List<Dictionary<string, object>>>();
        ClosedTrade closedTrade = new ClosedTrade();
        ListenerMgr positionListenerMgr = new ListenerMgr();

        public Position(ClosedTrade closedTrade)
        {
            this.closedTrade = closedTrade;
        }

        public Position(ClosedTrade closedTrade, PositionListener positionListener)
        {
            this.closedTrade = closedTrade;
            addPositionListener(positionListener);
        }

        public void addPositionListener(PositionListener positionListener)
        {
            positionListenerMgr.addListener(positionListener);
        }

        int tradeId = 1;
        public bool createPosition(string pair, DateTime dt, string BorS, int amount, double price, string customId)
        {
            // TODO -- need to handle partial position closes
            // Check if this new posn closes a previous opposite posn
            if (canClosePosition(pair, dt, BorS, amount, price, customId))
            {
                return false;   // no new posn created due to previous opposing posn closed
            }

            if (!mapPosition.ContainsKey(pair))
            {
                mapPosition.Add(pair, new List<Dictionary<string, object>>());
            }
            mapPosition[pair].Add(new Dictionary<string, object>());
            int ndx = mapPosition[pair].Count - 1;
            mapPosition[pair][ndx].Add("entry", BorS);
            mapPosition[pair][ndx].Add("amount", amount);
            mapPosition[pair][ndx].Add("price", price);
            mapPosition[pair][ndx].Add("customId", customId);
            mapPosition[pair][ndx].Add("datetime", dt);
            mapPosition[pair][ndx].Add("tradeId", ""+(tradeId++));

            foreach (Listener l in positionListenerMgr.getListeners())
            {
                ((PositionListener)l).positionChangeNotification(pair, mapPosition[pair][ndx], StateEvent.Create);
            }

            return true;    // creates new position
        }

        public bool canClosePosition(string pair, DateTime dt, string BorS, int amount, double currentPrice, string customId)
        {
            bool closed = false;
            int ndx = 0;

            if (mapPosition.ContainsKey(pair))
            {
                foreach (Dictionary<string, object> mapData in mapPosition[pair])
                {
                    string entry = (string)mapData["entry"];
                    int entryAmount = (int)mapData["amount"];
                    double entryPrice = (double)mapData["price"];

                    if (entry == "BUY" && BorS == "SELL")
                    {
                        // TODO handle partial closes (amount)
                        closedTrade.createClosedPosition(pair, (DateTime)mapData["datetime"], dt, (string)mapData["entry"], (int)mapData["amount"], (double)mapData["price"], currentPrice, (string)mapData["customId"]);
                        closed = true;
                        break;
                    }
                    else if (entry == "SELL" && BorS == "BUY")
                    {
                        // TODO handle partial closes (amount)
                        closedTrade.createClosedPosition(pair, (DateTime)mapData["datetime"], dt, (string)mapData["entry"], (int)mapData["amount"], (double)mapData["price"], currentPrice, (string)mapData["customId"]);
                        closed = true;
                        break;
                    }
                    ndx++;
                }
                if (closed)
                {
                    foreach (Listener l in positionListenerMgr.getListeners())
                    {
                        ((PositionListener)l).positionChangeNotification(pair, mapPosition[pair][ndx], StateEvent.Delete);
                    }
                    mapPosition[pair].RemoveAt(ndx);
                }
            }

            return closed;
        }

        public Dictionary<string, object> closePosition(string pair)
        {
            List<Dictionary<string, object>> list = mapPosition[pair];
            Dictionary<string, object> mapData = new Dictionary<string, object>();

            if (list.Count > 0)
            {
                mapData = list[0];
                list.RemoveAt(0);
            }
            return mapData;
        }
    }
}
