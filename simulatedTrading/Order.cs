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
        int OrderId = 1;

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
            string orderId = "orderId-" + (OrderId++);
            position.createPosition(pair, dt, BorS, amount, price, orderId, customId);
            return orderId;
        }

        public string createMarketOrder(string pair, DateTime dt, string BorS, int amount, double price, string customId, int stopPips, int limitPips)
        {
            return createMarketOrder(pair, dt, BorS, amount, price, customId, stopPips, limitPips, false);
        }
        public string createMarketOrder(string pair, DateTime dt, string BorS, int amount, double price, string customId, int stopPips, int limitPips, bool trailStop)
        {
            string sOrderId = "orderId-" + (OrderId++);
            bool createdPosn = position.createPosition(pair, dt, BorS, amount, price, sOrderId, customId);

            Dictionary<string, object> omap = new Dictionary<string, object>();
            omap.Add("entry", "mkt");
            omap.Add("type", "OM");
            omap.Add("amount", amount);
            omap.Add("price", price);
            omap.Add("orderId", sOrderId);
            omap.Add("customId", customId);
            foreach (Listener l in orderListenerMgr.getListeners())
            {
                ((OrderListener)l).orderChangeNotification(pair, omap, StateEvent.Create);
            }

            if (createdPosn)   // if true, creating new position
            {
                 // different omap values due to mkt entry order?
            }
            foreach (Listener l in orderListenerMgr.getListeners())
            {
                ((OrderListener)l).orderChangeNotification(pair, omap, StateEvent.Delete);
            }
            if (!createdPosn)   // if false, previous opposing posn closed by mkt order
            {
                return sOrderId;
            }

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
                createEntryOrder(pair, dt, slEntry, "STOP", amount, entryPrice, customId, trailStop, price);
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
            return sOrderId;
        }

        public void createEntryOrder(string pair, DateTime dt, string BorS, string SorL, int amount, double price, string customId)
        {
            createEntryOrder(pair, dt, BorS, SorL, amount, price, customId, false, 0.0);
        }
        public void createEntryOrder(string pair, DateTime dt, string BorS, string SorL, int amount, double price, string customId, bool trailStop, double openPrice)
        {
            string sOrderId = "orderId-" + (OrderId++);
            if (!mapEntryOrder.ContainsKey(pair))
            {
                mapEntryOrder.Add(pair, new List<Dictionary<string, object>>());
            }
            mapEntryOrder[pair].Add(new Dictionary<string, object>());
            int ndx = mapEntryOrder[pair].Count - 1;
            mapEntryOrder[pair][ndx].Add("entry", BorS);
            mapEntryOrder[pair][ndx].Add("stopOrLimit", SorL);
            mapEntryOrder[pair][ndx].Add("type", SorL=="S"?"STE":"LE");
            mapEntryOrder[pair][ndx].Add("amount", amount);
            mapEntryOrder[pair][ndx].Add("price", price);
            mapEntryOrder[pair][ndx].Add("orderId", sOrderId);
            mapEntryOrder[pair][ndx].Add("customId", customId);
            mapEntryOrder[pair][ndx].Add("trailStop", trailStop);
            mapEntryOrder[pair][ndx].Add("openPrice", openPrice);

            foreach (Listener l in orderListenerMgr.getListeners())
            {
                ((OrderListener)l).orderChangeNotification(pair, mapEntryOrder[pair][ndx], StateEvent.Create);
            }
        }

        /*
        The Two Types of trailing stops are Fixed and Dynamic:

        Fixed Trailing Stops trail your stop by a fixed amount of pips as the market 
        moves in your favor. This results in a slower trailing stop that waits for a 
        certain number of pips to be accrued before moving that amount of pips. The 
        minimum Fixed distance is 10 Pips.

        For example,  let’s say we had an initial -50 pip stop loss that we set to 
        trail with a fixed-step of 10. Our stop will stay at -50 until the price moves 
        in our favor a full 10 pips. Once +10 pips of floating profit is reached on 
        the trade, our fixed-step stop would jump from -50 to -40. Our stop would then 
        stay at -40 until the price moved in our favor another 10 pips up to +20 pips total.

        Dynamic Trailing Stops move your stop every 0.1 pips the market moves in 
        your favor 0.1 pips. The 0.1 Pip move cannot be changed.

            For example, let’s say we set our dynamic stop initially at -10 pips and 
            then the trade moves in our favor 1 pip. Our stop would move 1 pip from 
            -10 pips to -9 pips.
    */
        public void processOrders(string pair, DateTime dt, double bid, double ask)
        {
            bool found = false;
            string orderId = "n/a";
            double currentPrice = 0.0;
            if (mapEntryOrder.ContainsKey(pair))
                foreach (Dictionary<string, object> map in mapEntryOrder[pair])
                {
                    string type = (string)map["type"];
                    if (type == "OM") continue;
                    orderId = (string)map["orderId"];
                    double price = (double)map["price"];
                    string entry = (string)map["entry"];
                    string stopOrLimit = (string)map["stopOrLimit"];
                    bool trailStop = (bool)map["trailStop"];
                    if (stopOrLimit == "LIMIT" && entry == "BUY" && ask <= price)
                    {
                        found = true;
                        currentPrice = ask;
                    }
                    else if (stopOrLimit == "STOP" && entry == "BUY" && ask >= price)
                    {
                        found = true;
                        currentPrice = ask;
                    }
                    else if (stopOrLimit == "LIMIT" && entry == "SELL" && bid >= price)
                    {
                        found = true;
                        currentPrice = bid;
                    }
                    else if (stopOrLimit == "STOP" && entry == "SELL" && bid <= price)
                    {
                        found = true;
                        currentPrice = bid;
                    }
                    else if (stopOrLimit == "STOP" && entry == "BUY" && trailStop)
                    {
                        // dynamic TS recalc
                        double openPrice = (double)map["openPrice"];
                        double priceDiff = ask - openPrice;
                        if (priceDiff < 0)
                        {
                            price += priceDiff;
                            map["price"] = price;
                            map["openPrice"] = ask;
                        }
                    }
                    else if (stopOrLimit == "STOP" && entry == "SELL" && trailStop)
                    {
                        // dynamic TS recalc
                        double openPrice = (double)map["openPrice"];
                        double priceDiff = bid - openPrice;
                        if (priceDiff > 0)
                        {
                            price += priceDiff;
                            map["price"] = price;
                            map["openPrice"] = bid;
                        }
                    }
                    if (found)
                    {
                        Dictionary<string, object> mapData = position.closePosition(pair);
                        closedTrade.createClosedPosition(pair, (DateTime)mapData["datetime"], dt, (string)mapData["entry"], (int)mapData["amount"], (double)mapData["price"], currentPrice, orderId, (string)mapData["tradeId"], (string)mapData["customId"]);
                        break;
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
                mapEntryOrder[pair].Remove(map);
                foreach (Listener l in orderListenerMgr.getListeners())
                {
                    ((OrderListener)l).orderChangeNotification(pair, map, StateEvent.Delete);
                }
            }
        }
    }
}
