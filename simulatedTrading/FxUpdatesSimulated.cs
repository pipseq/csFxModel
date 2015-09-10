using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.fx;
using fxcore2;

namespace simulatedTrading
{
    public class FxUpdatesSimulated : IFxUpdates, PositionListener, OrderListener, ClosedTradeListener
    {
        Logger log = Logger.LogManager("FxUpdatesSimulated");
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
        private IAccumulatorMgr accumMgr;

        public IAccumulatorMgr AccumMgr
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
        public FxUpdatesSimulated(Display display, Control control, IFXManager fxManager, IAccumulatorMgr accumulatorMgr)
        {
            this.display = display;
            this.control = control;
            this.fxManager = fxManager;
            accumMgr = accumulatorMgr;
            initialize();
            log.debug("initialized()");
        }

        private void initialize()
        {
            TransactionManager.getInstance().FxUpdates = this;

            //offersTable = (O2GOffersTable)fxManager.Session.getTableManager().getTable(O2GTableType.Offers);
            //offersTable.RowChanged += new EventHandler<RowEventArgs>(offersTable_RowChanged);

            TransactionManager.getInstance().getPosition().addPositionListener(this);

            TransactionManager.getInstance().getOrder().addOrderListener(this);

            TransactionManager.getInstance().getClosedTrade().addClosedTradeListener(this);

            log.debug("initialize() completed");

        }

        #region same as FxUpdates, mostly
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

        private Dictionary<string, string> mapOfferIdPair = new Dictionary<string, string>();

        public void positionChangeNotification(string pair, Dictionary<string, object> map, StateEvent e)
        {
            if (StateEvent.Create == e)
            {
                Dictionary<string, string> tradeMap = new Dictionary<string, string>();
                string tradeId = "1";
                if (tradeId != null)
                    lock (posnLockObject)
                    {
                        //tradeMap["AccountID"] = otr.AccountID; ;
                        tradeMap["Amount"] = "" + map["amount"];
                        //tradeMap["OfferID"] = otr.OfferID;
                        //tradeMap["OrderID"] = otr.OpenOrderID;
                        tradeMap["BuySell"] = "" + map["entry"];
                        tradeMap["OpenRate"] = "" + map["price"];
                        tradeMap["Pair"] = pair;
                        tradeId = "" + map["tradeId"];
                        mapTrade.Add(tradeId, tradeMap);
                        if (!mapPairTrade.ContainsKey(pair))
                        {
                            mapPairTrade.Add(pair, new List<string>());
                        }
                        List<string> lt = mapPairTrade[pair];
                        lt.Add(tradeId);
                    }
            } else if (StateEvent.Delete == e)
            {
                string tradeId = "" + map["tradeId"];
                if (tradeId != null)
                    lock (posnLockObject)
                    {
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

            }
        }

        public void orderChangeNotification(string pair, Dictionary<string, object> map, StateEvent e)
        {
            throw new NotImplementedException();
        }

        public void closedTradeChangeNotification(string pair, Dictionary<string, object> map, StateEvent e)
        {
            throw new NotImplementedException();
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
        public List<string> getOrdersByTypes(string pair, List<string> types)
        {
            List<string> list = new List<string>();

            lock (orderLockObject) if (hasOrderInPair(pair))
                {
                    List<string> l0 = mapPairOrder[pair];
                    foreach (string oid in l0)
                        if (mapOrder.ContainsKey(oid))
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
            IDeleteOrders delOrds = FxManager.getCurrentActiveDeleteOrders();
            if (delOrds != null)
            {
                delOrds.pendingDeleteOrderNotifyComplete();
            }
            string orderId = otr.OrderID;
            string offerId = otr.OfferID;
            string pair = "n/a";
            if (orderId != null && offerId != null)
                lock (orderLockObject)
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
            if (orderId != null && offerId != null)
                lock (orderLockObject)
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
            if (closedTradeId != null && offerId != null)
                lock (closedTradesLockObject)
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

            if (closeOrderId != null && offerId != null)
                lock (closedTradesLockObject)
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
        int uid = 1;
        public string UniqueId
        {
            get
            {
                return "uid" + (uid++);
            }
        }

        public void closePosition(string pair)
        {
            throw new NotImplementedException();
        }

        public void createStopEntryOrder(string pair, string accountId, string tradeId, string amount, string offerId, string buySell, double rate)
        {
            throw new NotImplementedException();
        }

        public void deleteOrder(string pair)
        {
            throw new NotImplementedException();
        }

        public void enterOcoEntry(string pair, int amount, int spread, int stopPips, int limitPips, double bid, double ask)
        {
            throw new NotImplementedException();
        }

        public void enterPosition(string pair, string buySell, double last, int stopPips, int amount, string customId)
        {
            throw new NotImplementedException();
        }

        public void enterPosition(string pair, string buySell, double last, int stopPips, int limitPips, int amount, string customId)
        {
            TransactionManager.getInstance().getOrder().createMarketOrder(pair, DateTime.Now, buySell, amount, last, customId,stopPips,limitPips);
        }

        public string getPairOfferId(string pair)
        {
            throw new NotImplementedException();
        }

        public double getPairPointSize(string pair)
        {
            throw new NotImplementedException();
        }

        public void readRows()
        {
            throw new NotImplementedException();
        }

        public void setCanProcess(bool val)
        {
            throw new NotImplementedException();
        }
    }
}
