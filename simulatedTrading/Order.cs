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
        ListenerMgr orderListenerMgr = new ListenerMgr();
        int orderId = 1;

        public Order(ClosedTrade closedTrade, Position position)
        {
            this.closedTrade = closedTrade;
            this.position = position;
        }

        public void addOrderListener(OrderListener orderListener)
        {
            orderListenerMgr.addListener(orderListener);
        }

        public int getOrderCount(string pair)
        {
            if (!mapEntryOrder.ContainsKey(pair)) return 0;
            return mapEntryOrder[pair].Count();
        }

        public string  createMarketOrder(string pair, DateTime dt, string BorS, int amount, double price, string customId)
        {
            position.createPosition(pair, dt, BorS, amount, price, customId);
            return "orderId-" + (orderId++);
        }

        public string createMarketOrder(string pair, DateTime dt, string BorS, int amount, double price, string customId, int stopPips, int limitPips)
        {
            bool opened = position.createPosition(pair, dt, BorS, amount, price, customId);
            if (!opened)
                return "orderId-" + (orderId++);
            ;
            if (stopPips != 0)
            {
                string slEntry = "BUY";
                double entryPrice = price;
                if (BorS == "BUY")
                {
                    slEntry = "SELL";
                    entryPrice = price - (stopPips * TransactionManager.getPoiintSize(pair));
                }
                else
                {
                    entryPrice = price + (stopPips * TransactionManager.getPoiintSize(pair));
                }
                createEntryOrder(pair, dt, slEntry, "STOP", amount, entryPrice, customId);
            }
            if (limitPips != 0)
            {
                string slEntry = "BUY";
                double entryPrice = price;
                if (BorS == "BUY")
                {
                    slEntry = "SELL";
                    entryPrice = price + (limitPips * TransactionManager.getPoiintSize(pair));
                }
                else
                {
                    entryPrice = price - (limitPips * TransactionManager.getPoiintSize(pair));
                }
                createEntryOrder(pair, dt, slEntry, "LIMIT", amount, entryPrice, customId);
            }
            return "orderId-" + (orderId++);
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
            //mapEntryOrder[pair][ndx].Add("orderId", "" + (orderId++));
        }
        public void processOrders(string pair, DateTime dt, double currentPrice)
        {
            bool found = false;
            int ndx = 0;
            if (mapEntryOrder.ContainsKey(pair))
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
                if (n % 2 > 0)
                {
                    mapEntryOrder[pair].RemoveAt(n);
                    mapEntryOrder[pair].RemoveAt(n - 1);
                }
                else
                {
                    mapEntryOrder[pair].RemoveAt(n + 1);
                    mapEntryOrder[pair].RemoveAt(n);
                }

            }
        }
        public void closeOrders(string pair, string openEntry)
        {
            List<Dictionary<string, object>> removeList = new List<Dictionary<string, object>>();

            if (mapEntryOrder.ContainsKey(pair))
            foreach (Dictionary<string, object> map in mapEntryOrder[pair])
            {
                string entry = (string)map["entry"];
                string stopOrLimit = (string)map["stopOrLimit"];
                if ((stopOrLimit == "LIMIT" || stopOrLimit == "STOP")
                    && entry == "BUY" && openEntry == "SELL")
                {
                    removeList.Add(map);
                }
                else if ((stopOrLimit == "LIMIT" || stopOrLimit == "STOP")
                && entry == "SELL" && openEntry == "BUY")
                {
                    removeList.Add(map);
                }
            }
            foreach (Dictionary<string, object> map in removeList)
            {
                {
                    mapEntryOrder[pair].Remove(map);
                }
            }
        }
    }
}
