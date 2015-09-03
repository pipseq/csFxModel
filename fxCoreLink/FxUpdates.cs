using Common;
using fxcore2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace fxCoreLink
{
    public class FxUpdates
    {

        Logger log = Logger.LogManager("FxUpdates");
        private Display display;
        public Display Display
        {
            get
            {
                return display;
            }
        }
        private Control control;

        public Control Control
        {
            get { return control; }
            set { control = value; }
        }
        private IFXManager fxManager;

        public IFXManager FxManager
        {
            get { return fxManager; }
            set { fxManager = value; }
        }
        private AccumulatorMgr accumMgr;

        public AccumulatorMgr AccumMgr
        {
            get { return accumMgr; }
            set { accumMgr = value; }
        }
        private string journalFile;
        private bool debug = false;

        public bool Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                if (accumMgr != null)
                {
                    accumMgr.Debug = value;
                }
            }
        }


        public FxUpdates(Display display, Control control, IFXManager fxManager, AccumulatorMgr accumulatorMgr)
        {
            this.display = display;
            this.control = control;
            this.fxManager = fxManager;
            accumMgr = accumulatorMgr;
            initialize();
            log.debug("initialized()");
        }

        private int customIdCounter = 1;
        public string UniqueId
        {
            get
            {
                string s = string.Format("{0}{1}{2}{3}",
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    customIdCounter++.ToString("D6"));
                return s;
            }
        }

        private Dictionary<string, Dictionary<string, string>> mapPairParams = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> mapOfferIdPair = new Dictionary<string, string>();

        //public Dictionary<string, string> MapOfferId
        //{
        //    get { return mapPairOfferId; }
        //    set { mapPairOfferId = value; }
        //}

        public string getPairOfferId(string pair)
        {
            return mapPairParams[pair]["OfferID"];
        }

        public double getPairPointSize(string pair)
        {
            string sPS = mapPairParams[pair]["PointSize"];
            return double.Parse(sPS);
        }

        private void initialize()
        {
            mapPairParams = new Dictionary<string, Dictionary<string, string>>();
            mapOfferIdPair = new Dictionary<string, string>();
            fxManager.getSession();
            Dictionary<string, Dictionary<string, string>> map = fxManager.getTable("OFFERS");
            foreach (string pair in map.Keys)
            {
                Dictionary<string, string> omap = map[pair];
                mapPairParams.Add(pair, omap);
                mapOfferIdPair.Add(omap["OfferID"], pair);
            }
            fxManager.closeSession();

            //journalFile = Control.getProperty("journalFile",null);
        }
        #region offers/price update

        public void offersTable_RowChanged(object sender, RowEventArgs e)
        {
            O2GOfferTableRow otr = (O2GOfferTableRow)e.RowData;
            if (otr == null)
                return;
            double bid = otr.Bid;
            double ask = otr.Ask;
            string pair = otr.Instrument;
            if (bid != 0.0)
            {
                addData(otr.Instrument, otr.Time.ToLocalTime(), bid, ask);
            }
        }

        // price updates per pair are processed into the respective AccumMgr
        Dictionary<string, List<Dictionary<string, object>>> threadMap = new Dictionary<string, List<Dictionary<string, object>>>();
        public static Dictionary<string, Dictionary<string, object>> threadStats = new Dictionary<string, Dictionary<string, object>>();

        private void addData(string pair, DateTime dt, double bid, double ask)
        {
            if (!threadMap.ContainsKey(pair))
            {
                threadMap.Add(pair, new List<Dictionary<string, object>>());
                threadStats.Add(pair, new Dictionary<string, object>());
                threadStats[pair].Add("count", 0);
                threadStats[pair].Add("duration", new List<long>());
                threadStats[pair].Add("posnCount", 0);
                threadStats[pair].Add("posnDuration", new List<long>());
                threadStats[pair].Add("queueDepth", new List<int>());

                startThread(pair);
            }
            List<Dictionary<string, object>> list = threadMap[pair];
            Dictionary<string, object> dataMap = new Dictionary<string, object>();
            dataMap.Add("BID", bid);
            dataMap.Add("ASK", ask);
            dataMap.Add("DateTime", dt);
            list.Add(dataMap);
        }

        private void startThread(string pair)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(run));
            thread.Name = pair;
            threadStats[pair].Add("thread", thread);
            thread.Start(pair);
        }
        public bool canProcess = true;
        public void run(object pairObj)
        {
            string pair = "" + pairObj;
            List<Dictionary<string, object>> list = threadMap[pair];
            while (canProcess)
            {
                if (list.Count > 0) //lock(list)
                {
                    Dictionary<string, object> dataMap = list[0];
                    //Console.WriteLine("" + pair + "=" + dataMap["BID"] + " @ " + dataMap["DateTime"]);
                    //long ticks = DateTime.Now.Ticks;
                    accumMgr.add(pair, (DateTime)dataMap["DateTime"], (double)dataMap["BID"], (double)dataMap["ASK"]);
                    list.RemoveAt(0);
                    //ticks = DateTime.Now.Ticks - ticks;

                    // stats
                    //lock (threadStats)
                    //{
                    //    int cnt = (int)threadStats[pair]["count"];
                    //    threadStats[pair]["count"] = cnt + 1;
                    //    List<long> lduration = (List<long>)threadStats[pair]["duration"];
                    //    if (ticks > 0)
                    //        lduration.Add(ticks);
                    //    List<int> lqdep = (List<int>)threadStats[pair]["queueDepth"];
                    //    if (list.Count > 0)
                    //        lqdep.Add(list.Count);
                    //}
                }
                else Thread.Sleep(100);
            }
        }
        #endregion

        #region table management

        O2GOffersTable offersTable;
        O2GTradesTable tradesTable;
        O2GOrdersTable ordersTable;
        O2GClosedTradesTable closedTradesTable;
        public void readRows()
        {
            offersTable = (O2GOffersTable)fxManager.Session.getTableManager().getTable(O2GTableType.Offers);
            offersTable.RowChanged += new EventHandler<RowEventArgs>(offersTable_RowChanged);

            List<O2GTradeRow> listTrades = fxManager.loadTrades();
            loadTrades(listTrades);
            tradesTable = (O2GTradesTable)fxManager.Session.getTableManager().getTable(O2GTableType.Trades);
            tradesTable.RowAdded += new EventHandler<RowEventArgs>(tradesTable_RowAdded);
            tradesTable.RowDeleted += new EventHandler<RowEventArgs>(tradesTable_RowDeleted);

            List<O2GOrderRow> listOrders = fxManager.loadOrders();
            loadOrders(listOrders);
            ordersTable = (O2GOrdersTable)fxManager.Session.getTableManager().getTable(O2GTableType.Orders);
            ordersTable.RowAdded += new EventHandler<RowEventArgs>(ordersTable_RowAdded);
            ordersTable.RowDeleted += new EventHandler<RowEventArgs>(ordersTable_RowDeleted);

            closedTradesTable = (O2GClosedTradesTable)fxManager.Session.getTableManager().getTable(O2GTableType.ClosedTrades);
            closedTradesTable.RowAdded += new EventHandler<RowEventArgs>(closedTradesTable_RowAdded);
            closedTradesTable.RowDeleted += new EventHandler<RowEventArgs>(closedTradesTable_RowDeleted);
            log.debug("readRows() completed");

        }

        private object posnLockObject = new object();
        public Dictionary<string, Dictionary<string, string>> mapTrade = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, List<string>> mapPairTrade = new Dictionary<string, List<string>>();

        public bool hasPositionInPair(string pair)
        {
            return mapPairTrade.ContainsKey(pair);
        }

        public bool hasPosition(string tradeId)
        {
            return mapTrade.ContainsKey(tradeId);
        }

        // gets only the first trade (FIFO)
        public Dictionary<string, string> getTrade(string pair)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            lock (posnLockObject) if (hasPositionInPair(pair))
                {
                    List<string> lt = mapPairTrade[pair];
                    map = mapTrade[lt[0]];
                }
            return map;
        }
        void printPairTrade()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string pair in mapPairTrade.Keys)
            {
                sb.AppendFormat("Posn: pair={0}", pair);
                List<string> lt = mapPairTrade[pair];
                foreach (string offer in lt)
                {
                    sb.AppendFormat("\t{0}", offer);
                }
                sb.AppendFormat("; ");
            }
            log.debug(sb.ToString());
        }

        void tradesTable_RowDeleted(object sender, RowEventArgs e)
        {
            O2GTradeRow otr = (O2GTradeRow)e.RowData;
            if (otr == null)
                return;
            string tradeId = otr.TradeID;
            string offerId = otr.OfferID;
            string pair = "n/a";
            if (tradeId != null && offerId != null) lock (posnLockObject)
                {
                    pair = this.mapOfferIdPair[offerId];
                    if (mapTrade.ContainsKey(tradeId))
                    {
                        mapTrade.Remove(tradeId);
                    }
                    if (mapPairTrade.ContainsKey(pair))
                    {
                        List<string> lt = mapPairTrade[pair];
                        lt.Remove(tradeId);
                        if (lt.Count == 0)
                            mapPairTrade.Remove(pair);
                    }
                }
            log.debug(string.Format("closed, {0}, tradeId={1}, OpenOrderID={2}, {3}, {4}",
                pair, tradeId, otr.OpenOrderID, otr.BuySell, otr.Amount));
            if (Debug)
            {
                log.debug("Posn deleted");
                printPairTrade();
            }
        }

        void tradesTable_RowAdded(object sender, RowEventArgs e)
        {
            addTradeRow((O2GTradeRow)e.RowData);
        }

        void loadTrades(List<O2GTradeRow> lTrades)
        {
            foreach (O2GTradeRow otr in lTrades)
            {
                addTradeRow(otr);
            }
        }
        void addTradeRow(O2GTradeRow otr)
        {
            if (otr == null)
                return;
            string tradeId = otr.TradeID;
            string offerId = otr.OfferID;
            string pair = this.mapOfferIdPair[offerId];
            Dictionary<string, string> tradeMap = new Dictionary<string, string>();
            if (tradeId != null && offerId != null) lock (posnLockObject)
                {
                    tradeMap["AccountID"] = otr.AccountID; ;
                    tradeMap["Amount"] = "" + otr.Amount;
                    tradeMap["OfferID"] = otr.OfferID;
                    tradeMap["OrderID"] = otr.OpenOrderID;
                    tradeMap["BuySell"] = otr.BuySell;
                    tradeMap["OpenRate"] = "" + otr.OpenRate;
                    tradeMap["Pair"] = pair;
                    tradeMap["TradeID"] = tradeId;
                    mapTrade.Add(tradeId, tradeMap);
                    if (!mapPairTrade.ContainsKey(pair))
                    {
                        mapPairTrade.Add(pair, new List<string>());
                    }
                    List<string> lt = mapPairTrade[pair];
                    lt.Add(tradeId);
                }
            log.debug(string.Format("trade, {0}, tradeId={1}, orderId={2}, {3}, {4}, accountId={5}, time={6}",
                pair, tradeMap["TradeID"], tradeMap["OrderID"], tradeMap["BuySell"], 
                tradeMap["Amount"], tradeMap["AccountID"],
                CtrlTimer.getTimeNowFormatted()));
            if (Debug)
            {
                printPairTrade();
            }
        }


        public bool hasOrderInPair(string pair)
        {
            return mapPairOrder.ContainsKey(pair);
        }

        // gets only the first order (FIFO)
        public Dictionary<string, string> getOrder(string pair)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            lock (orderLockObject) if (hasOrderInPair(pair))
                {
                    List<string> lt = mapPairOrder[pair];
                    map = mapOrder[lt[0]];
                }
            return map;
        }

        // gets the list of orders
        public List<string> getOrders(string pair)
        {
            List<string> list = new List<string>();

            lock (orderLockObject) if (hasOrderInPair(pair))
                {
                    List<string> l0 = mapPairOrder[pair];
                    foreach (string oid in l0)
                    {
                        list.Add(oid);
                    }
                }
            return list;
        }

        // gets the list of orders by types
        public List<string> getOrdersByType(string pair, string type)
        {
            List<string> ls = new List<string>();
            ls.Add(type);
            return getOrdersByTypes(pair, ls);
        }

        // gets the list of orders by types
        public List<string> getOrdersByTypes(string pair,List<string> types)
        {
            List<string> list = new List<string>();

            lock (orderLockObject) if (hasOrderInPair(pair))
                {
                    List<string> l0 = mapPairOrder[pair];
                    foreach (string oid in l0) if (mapOrder.ContainsKey(oid))
                    {
                        Dictionary<string, string> m = mapOrder[oid];
                        string type = m["Type"];
                        if (types.Contains(type))
                            list.Add(oid);
                    }
                }
            return list;
        }

        private object orderLockObject = new object();
        public Dictionary<string, Dictionary<string, string>> mapOrder = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, List<string>> mapPairOrder = new Dictionary<string, List<string>>();


        void ordersTable_RowDeleted(object sender, RowEventArgs e)
        {
            O2GOrderRow otr = (O2GOrderRow)e.RowData;
            if (otr == null)
                return;
            DeleteOrders delOrds = FxManager.getCurrentActiveDeleteOrders();
            if (delOrds != null)
            {
                delOrds.manualEvent.Set();
            }
            string orderId = otr.OrderID;
            string offerId = otr.OfferID;
            string pair = "n/a";
            if (orderId != null && offerId != null) lock (orderLockObject)
                {
                    pair = this.mapOfferIdPair[offerId];
                    if (mapOrder.ContainsKey(orderId))
                    {
                        mapOrder.Remove(orderId);
                    }
                    if (mapPairOrder.ContainsKey(pair))
                    {
                        List<string> lt = mapPairOrder[pair];
                        lt.Remove(orderId);
                        if (lt.Count == 0)
                            mapPairOrder.Remove(pair);
                    }
                }
            log.debug(string.Format("Order deleted, {0}, orderId={1}, AccountID={2}, {3}, {4}",
                pair, orderId, otr.AccountID, otr.BuySell, otr.Amount));
            if (Debug)
            {
                log.debug("Order deleted");
                printPairTrade();
            }
        }

        void ordersTable_RowAdded(object sender, RowEventArgs e)
        {
            loadOrder((O2GOrderRow)e.RowData);
        }

        void loadOrders(List<O2GOrderRow> listOrders)
        {
            foreach (O2GOrderRow otr in listOrders)
            {
                loadOrder(otr);
            }
        }

        void loadOrder(O2GOrderRow otr)
        {
            if (otr == null)
                return;
            string orderId = otr.OrderID;
            string offerId = otr.OfferID;
            string pair = this.mapOfferIdPair[offerId];
            Dictionary<string, string> orderMap = new Dictionary<string, string>();
            if (orderId != null && offerId != null) lock (orderLockObject)
                {
                    orderMap["AccountID"] = otr.AccountID; ;
                    orderMap["Amount"] = "" + otr.Amount;
                    orderMap["OfferID"] = otr.OfferID;
                    orderMap["RequestID"] = otr.RequestID;
                    orderMap["BuySell"] = otr.BuySell;
                    orderMap["PegType"] = "" + otr.PegType;
                    orderMap["PegOffset"] = "" + otr.PegOffset;
                    orderMap["TrailStep"] = "" + otr.TrailStep;
                    orderMap["TradeID"] = "" + otr.TradeID;
                    orderMap["Type"] = "" + otr.Type;
                    orderMap["Pair"] = pair;
                    orderMap["OrderID"] = orderId;
                    mapOrder.Add(orderId, orderMap);
                    if (!mapPairOrder.ContainsKey(pair))
                    {
                        mapPairOrder.Add(pair, new List<string>());
                    }
                    List<string> lt = mapPairOrder[pair];
                    lt.Add(orderId);
                }
            log.debug(string.Format("Order added, {0}, orderId={1}, offerID={2}, {3}, {4}, accountId={5}",
                pair, orderMap["OrderID"], orderMap["OfferID"], orderMap["BuySell"], orderMap["Amount"], orderMap["AccountID"]));
            if (Debug)
            {
                printPairTrade();
            }
        }

        private object closedTradesLockObject = new object();
        public Dictionary<string, Dictionary<string, object>> mapClosedTrade = new Dictionary<string, Dictionary<string, object>>();


        void closedTradesTable_RowDeleted(object sender, RowEventArgs e)
        {
            O2GOrderRow otr = (O2GOrderRow)e.RowData;
            if (otr == null)
                return;
            string closedTradeId = otr.OrderID;
            string offerId = otr.OfferID;
            string pair = "n/a";
            if (closedTradeId != null && offerId != null) lock (closedTradesLockObject)
                {
                    pair = this.mapOfferIdPair[offerId];
                    if (mapClosedTrade.ContainsKey(closedTradeId))
                    {
                        mapClosedTrade.Remove(closedTradeId);
                    }
                }
            log.debug(string.Format("ClosedTrade deleted, {0}, closedTradeId={1}, AccountID={2}, {3}, {4}",
                pair, closedTradeId, otr.AccountID, otr.BuySell, otr.Amount));
        }

        void closedTradesTable_RowAdded(object sender, RowEventArgs e)
        {
            O2GClosedTradeRow otr = (O2GClosedTradeRow)e.RowData;
            if (otr == null)
                return;
            string closeOrderId = otr.CloseOrderID;
            string offerId = otr.OfferID;
            string pair = this.mapOfferIdPair[offerId];
            Dictionary<string, object> closedTradeMap = new Dictionary<string, object>();

            if (closeOrderId != null && offerId != null) lock (closedTradesLockObject)
                {
                    closedTradeMap["AccountID"] = otr.AccountID; ;
                    closedTradeMap["Amount"] = otr.Amount;
                    closedTradeMap["OfferID"] = otr.OfferID;
                    closedTradeMap["BuySell"] = otr.BuySell;
                    closedTradeMap["OpenTime"] = otr.OpenTime;
                    closedTradeMap["CloseTime"] = otr.CloseTime;
                    closedTradeMap["OpenRate"] = otr.OpenRate;
                    closedTradeMap["CloseRate"] = otr.CloseRate;
                    closedTradeMap["GrossPL"] = otr.GrossPL;
                    closedTradeMap["Commission"] = otr.Commission;
                    closedTradeMap["TradeID"] = otr.TradeID;
                    closedTradeMap["Pair"] = pair;
                    closedTradeMap["OrderID"] = closeOrderId;
                    //mapClosedTrade.Add(closeOrderId, closedTradeMap);
                }
            log.debug(string.Format("ClosedTrade added, {0}, closedTradeId={1}, offerID={2}, {3}, {4}, accountId={5}",
                pair, closedTradeMap["OrderID"], closedTradeMap["OfferID"], closedTradeMap["BuySell"], closedTradeMap["Amount"], closedTradeMap["AccountID"]));
        }


        #endregion
        #region OCO open/close

        // open OCO transaction
        List<Dictionary<string, object>> openOcoQueue = new List<Dictionary<string, object>>();
        Thread openOcoThread;
        public void enterOcoEntry(string pair, int amount, int spread, int stopPips, int limitPips, double bid, double ask)
        {

            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("Pair", pair);
            map.Add("Spread", spread);
            map.Add("StopPips", stopPips);
            map.Add("LimitPips", limitPips);
            map.Add("Amount", amount);
            map.Add("Bid", bid);
            map.Add("Ask", ask);
            //map.Add("PointSize", "" + pointSize);

            lock (openOcoQueue)   // potential race across pair-threads
            {
                openOcoQueue.Add(map);
                if (openOcoThread == null)
                {
                    openOcoThread = new Thread(runOpenOco);
                    openOcoThread.Name = "openOcoThread";
                    FxUpdates.threadStats.Add("enterOco", new Dictionary<string, object>());
                    FxUpdates.threadStats["enterOco"].Add("thread", openOcoThread);
                    openOcoThread.Start();
                }
            }
        }

        public void runOpenOco()
        {

            while (canProcess)
            {
                if (openOcoQueue.Count > 0)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    lock (openOcoQueue)
                    {
                        map = openOcoQueue[0];
                        openOcoQueue.RemoveAt(0);
                    }
                    if (map.Count > 0)
                    {
                        string pair = (string)map["Pair"];
                        //string customId = map["CustomId"];
                        int stopPips = (int)map["StopPips"];
                        int limitPips = (int)map["LimitPips"];
                        int amount = (int)map["Amount"];
                        int spread = (int)map["Spread"];
                        double bid = (double)map["Bid"];
                        double ask = (double)map["Ask"];

                        if (Control.ordersArmed())
                        {
                            OCOTrade ocoTrade = FxManager.getOcoTrade();
                            string OcoOrderId = ocoTrade.tradeOCO(FxManager, this, pair, amount, spread, stopPips, limitPips, bid, ask);
                            FxManager.closeOcoTrade();
                        }
                    }
                }
                else Thread.Sleep(100);
            }
        }

        // open transaction

        List<Dictionary<string, string>> openQueue = new List<Dictionary<string, string>>();
        Thread openThread;
        public void enterPosition(string pair, string buySell, double last, int stopPips, int amount, string customId)
        {
            enterPosition(pair, buySell, last, stopPips, 0, amount, customId);
        }

        public void enterPosition(string pair, string buySell, double last, int stopPips, int limitPips, int amount, string customId)
        {

            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("Pair", pair);
            map.Add("BuySell", buySell);
            map.Add("Last", "" + last);
            map.Add("StopPips", "" + stopPips);
            map.Add("LimitPips", "" + limitPips);
            map.Add("Amount", "" + amount);
            map.Add("CustomId", customId);
            lock (openQueue)   // potential race across pair-threads
            {
                openQueue.Add(map);
                if (openThread == null)
                {
                    openThread = new Thread(runOpen);
                    openThread.Name = "openThread";
                    FxUpdates.threadStats.Add("enterPosition", new Dictionary<string, object>());
                    FxUpdates.threadStats["enterPosition"].Add("thread", openThread);
                    openThread.Start();
                }
            }
        }

        public void runOpen()
        {

            while (canProcess)
            {
                if (openQueue.Count > 0)
                {
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    lock (openQueue)
                    {
                        map = openQueue[0];
                        openQueue.RemoveAt(0);
                    }
                    if (map.Count > 0)
                    {
                        string pair = map["Pair"];
                        string customId = map["CustomId"];
                        string sStopPips = map["StopPips"];
                        int stopPips = int.Parse(sStopPips);
                        string sAmount = map["Amount"];
                        int amount = int.Parse(sAmount);
                        string sLimitPips = map["LimitPips"];
                        int limitPips = int.Parse(sLimitPips);
                        string sLast = map["Last"];
                        double last = double.Parse(sLast);    // does this make sense to return?
                        string buySell = map["BuySell"];
                        string sBuySell = "";
                        if (buySell.ToUpper().Equals("BUY"))
                            sBuySell = "B";
                        else if (buySell.ToUpper().Equals("SELL"))
                            sBuySell = "S";
                        else continue;

                        if (Control.ordersArmed())
                        {
                            string offerId = FxManager.getMarketTrade() // TODO - manage offerId returned
                                .trade(pair, amount, sBuySell,
                                stopPips,
                                limitPips,
                                last,
                                customId);
                            FxManager.closeMarketTrade();
                        }
                    }
                }
                else Thread.Sleep(100);
            }
        }
        //Pair, accountId, tradeId, amount, offerId, buySell
        public void createStopEntryOrder(string pair, string accountId, string tradeId, string amount, string offerId, string buySell, double rate)
        {
            CreateOrder co = new CreateOrder(this.FxManager.Session, this.Display);
            double dRate = rate;
            int iAmount = int.Parse(amount);
            string sBuySell = "";
            if (buySell == "B") sBuySell = "S";
            else sBuySell = "B";
            co.CreateEntryOrder(accountId, pair, offerId, dRate, iAmount, sBuySell, tradeId);
        }

        List<Dictionary<string, string>> closeQueue = new List<Dictionary<string, string>>();
        Thread closeThread;
        public void closePosition(string pair)
        {

            Dictionary<string, string> map = getTrade(pair);
            lock (closeQueue)   // potential race across pair-threads
            {
                closeQueue.Add(map);
                if (closeThread == null)
                {
                    closeThread = new Thread(runClose);
                    closeThread.Name = "closeThread";
                    FxUpdates.threadStats.Add("close", new Dictionary<string, object>());
                    FxUpdates.threadStats["close"].Add("thread", closeThread);
                    closeThread.Start();
                }
            }
        }

        public void runClose()
        {

            while (canProcess)
            {
                if (closeQueue.Count > 0)
                {
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    lock (closeQueue)   // potential race across pair-threads
                    {
                        map = closeQueue[0];
                        closeQueue.RemoveAt(0);
                    }
                    if (map.Count > 0)
                    {
                        string pair = map["Pair"];
                        Dictionary<string, string> omap = getOrder(pair);
                        string sOrderID = null;
                        if (omap.ContainsKey("OrderID"))    // may not exist, some SL orders get lost!!
                            sOrderID = omap["OrderID"];

                        string offerId = map["OfferID"];
                        string tradeId = map["TradeID"];
                        string accountId = map["AccountID"];
                        string sAmount = map["Amount"];
                        int amount = int.Parse(sAmount);    // does this make sense to return?
                        string buySell = map["BuySell"];
                        string sBuySell = "";
                        if (buySell.Equals("B"))      // switch "BuySell" 
                            sBuySell = "S";           // (if the position in "Buy", close position will be "Sell" and vice versa) 
                        else
                            sBuySell = "B";
                        log.debug("Attempt close: " + pair + ", " + tradeId + " (queue depth=" + closeQueue.Count + ")");
                        ClosePositions cp = FxManager.getClosePositions();
                        cp.closePosition(tradeId, offerId, accountId, amount, sBuySell, sOrderID);
                        FxManager.closeClosePositions();
                        //log.debug("Completed close: " + pair + ", " + tradeId);
                        log.debug("close request, {0}, tradeId={1}, {2}, {3}",
                        pair, map["TradeID"], map["BuySell"], map["Amount"]);

                    }
                }
                else Thread.Sleep(100);
            }
        }

        private List<Dictionary<string, string>> deleteOrderQueue = new List<Dictionary<string, string>>();
        private Thread deleteOrderThread;
        public void deleteOrder(string pair)
        {

            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("Pair", pair);
            lock (deleteOrderQueue)   // potential race across pair-threads
            {
                deleteOrderQueue.Add(map);
                if (deleteOrderThread == null)
                {
                    deleteOrderThread = new Thread(runDeleteOrder);
                    deleteOrderThread.Name = "deleteOrderThread";
                    FxUpdates.threadStats.Add("deleteOrder", new Dictionary<string, object>());// obj with same name already added!!
                    FxUpdates.threadStats["deleteOrder"].Add("thread", deleteOrderThread);
                    deleteOrderThread.Start();
                }
            }
        }

        public void runDeleteOrder()
        {

            while (canProcess)
            {
                if (deleteOrderQueue.Count > 0)
                {

                    // attempt to delete all orders for a pair
                    // with pending OCO, that's four orders
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    lock (deleteOrderQueue)   // potential race across pair-threads
                    {
                        map = deleteOrderQueue[0];
                        deleteOrderQueue.RemoveAt(0);
                    }
                    if (map.Count > 0)
                    {
                        string pair = map["Pair"];

                        DeleteOrders delOrds = FxManager.getDeleteOrders();

                        foreach (string orderId in getOrdersByType(pair, "SE"))
                        {
                            delOrds.deleteOrder(orderId);
                        }
                        FxManager.closeDeleteOrders();


                       log.debug("deleteOrders request, {0}",
                        pair);

                    }
                }
                else Thread.Sleep(100);
            }
        }

        #endregion

    }
}