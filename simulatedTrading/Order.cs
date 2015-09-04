using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulatedTrading
{

    // TODO--need entry order validation 
    public class Order
    {
        public Dictionary<string, List<Dictionary<string, object>>> mapEntryOrder
            = new Dictionary<string, List<Dictionary<string, object>>>();
        ClosedTrade closedTrade;
        Position position;

        public Order(ClosedTrade closedTrade, Position position)
        {
            this.closedTrade = closedTrade;
            this.position = position;
        }

        public void createEntryOrder(string pair, DateTime dt, string BorS, string SorL, int amount, double price, string customId)
        {
            if (!mapEntryOrder.ContainsKey(pair))
            {
                mapEntryOrder.Add(pair, new List<Dictionary<string, object>>());
            }
            mapEntryOrder[pair].Add(new Dictionary<string, object>());
            int ndx = mapEntryOrder[pair].Count - 1;
            mapEntryOrder[pair][ndx].Add("entry", BorS);
            mapEntryOrder[pair][ndx].Add("stopOrLimit", SorL);
            mapEntryOrder[pair][ndx].Add("amount", amount);
            mapEntryOrder[pair][ndx].Add("price", price);
            mapEntryOrder[pair][ndx].Add("customId", customId);
        }
        public void processOrders(string pair, DateTime dt, double currentPrice)
        {
            bool found = false;
            int ndx = 0;
            foreach (Dictionary<string, object> map in mapEntryOrder[pair])
            {
                double price = (double)map["price"];
                string entry = (string)map["entry"];
                string stopOrLimit = (string)map["stopOrLimit"];
                if (stopOrLimit == "LIMIT" && entry == "BUY" && currentPrice <= price)
                {
                    Dictionary<string, object> mapData = position.closePosition(pair);
                    closedTrade.createClosedPosition(pair, (DateTime)mapData["datetime"], dt, (string)mapData["entry"], (int)mapData["amount"], (double)mapData["price"], currentPrice, (string)mapData["customId"]);
                    found = true;
                }
                else if (stopOrLimit == "STOP" && entry == "BUY" && currentPrice >= price)
                {
                    Dictionary<string, object> mapData = position.closePosition(pair);
                    closedTrade.createClosedPosition(pair, (DateTime)mapData["datetime"], dt, (string)mapData["entry"], (int)mapData["amount"], (double)mapData["price"], currentPrice, (string)mapData["customId"]);
                    found = true;
                }
                else if (stopOrLimit == "LIMIT" && entry == "SELL" && currentPrice >= price)
                {
                    Dictionary<string, object> mapData = position.closePosition(pair);
                    closedTrade.createClosedPosition(pair, (DateTime)mapData["datetime"], dt, (string)mapData["entry"], (int)mapData["amount"], (double)mapData["price"], currentPrice, (string)mapData["customId"]);
                    found = true;
                }
                else if (stopOrLimit == "STOP" && entry == "SELL" && currentPrice <= price)
                {
                    Dictionary<string, object> mapData = position.closePosition(pair);
                    closedTrade.createClosedPosition(pair, (DateTime)mapData["datetime"], dt, (string)mapData["entry"], (int)mapData["amount"], (double)mapData["price"], currentPrice, (string)mapData["customId"]);
                    found = true;
                }
                ndx++;
            }
            if (found)
            {
                int n = ndx - 1;
                if (n%2 >0)
                {
                    mapEntryOrder[pair].RemoveAt(n);
                    mapEntryOrder[pair].RemoveAt(n-1);
                } else
                {
                    mapEntryOrder[pair].RemoveAt(n + 1);
                    mapEntryOrder[pair].RemoveAt(n);
                }

            }
        }
    }
}
