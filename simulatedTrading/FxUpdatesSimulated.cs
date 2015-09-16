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
        public Dictionary<string, Dictionary<string, object>> mapTrade = new Dictionary<string, Dictionary<string, object>>();
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
        public Dictionary<string, object> getTrade(string pair)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();

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
                Dictionary<string, object> tradeMap = new Dictionary<string, object>();
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
                        log.debug("trade opened\t{0}, amount={1}, entry={2}, orderId={3}, tradeId={4}, customId={5}, accountId={6}",
                             pair, map["amount"], map["entry"], map["orderId"], map["tradeId"], map["customId"], "simulated");
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
                log.debug("trade closed\t{0}, amount={1}, entry={2}, tradeId={3}, openOrderID={4}, customId={5}, accountId={6}",
                     pair, map["amount"], map["entry"], map["tradeId"], map["orderId"], map["customId"], "simulated");
            }
        }

        public void orderChangeNotification(string pair, Dictionary<string, object> map, StateEvent e)
        {
            if (StateEvent.Create == e)
            {
                Dictionary<string, string> orderMap = new Dictionary<string, string>();
                string orderId = "1";
                if (orderId != null)
                        //tradeMap["AccountID"] = otr.AccountID; ;
                        orderMap["Amount"] = "" + map["amount"];
                        //tradeMap["OfferID"] = otr.OfferID;
                        //tradeMap["OrderID"] = otr.OpenOrderID;
                        orderMap["BuySell"] = "" + map["entry"];
                        orderMap["OpenRate"] = "" + map["price"];
                        orderMap["Pair"] = pair;
                        orderId = "" + map["orderId"];
                log.debug("Order added\t{0}, amount={1}, entry={2}, orderId={3}, type={4}, contingencyType={5}, rate={6}, pegOffset={7}, customId={8}, accountId={9}",
                    pair, map["amount"], map["entry"], map["orderId"], map["type"], "n/a", map["price"], "n/a", map["customId"], "simulated");
            }
            else if (StateEvent.Delete == e)
            {
                string orderId = "" + map["orderId"];
                if (orderId != null)
                    log.debug("Order deleted\t{0}, amount={1}, entry={2}, orderId={3}, type={4}, contingencyType={5}, rate={6}, customId={7}, accountId={8}",
                    pair, map["amount"], map["entry"], map["orderId"], map["type"], "n/a", map["price"], map["customId"], "simulated");
            }
        }

        public double getLast(string pair, TimeFrame timeFrame, PriceComponent priceComponent)
        {
            return AccumMgr.getAccum(pair, timeFrame, priceComponent).getLast();
        }

        public double getLast(string pair)
        {
            return getLast(pair, TimeFrame.m1, PriceComponent.BidClose);
        }

        public double getSpotRate(string pair)
        {
            try
            {
                if (pair.EndsWith("USD"))
                {   // USD is base rate
                    return 1.0;
                }
                else
                {
                    string baseCurrency = pair.Substring(4);
                    if (baseCurrency == "AUD") return 1 / getLast("AUD/USD");
                    if (baseCurrency == "EUR") return 1 / getLast("EUR/USD");
                    if (baseCurrency == "GBP") return 1 / getLast("GBP/USD");
                    if (baseCurrency == "CAD") return getLast("USD/CAD");
                    if (baseCurrency == "JPY") return getLast("USD/JPY");
                    if (baseCurrency == "NZD") return getLast("NZD/USD");
                    if (baseCurrency == "CNH") return getLast("USD/CNH");
                    if (baseCurrency == "CHF") return getLast("USD/CHF");
                }
            }
            catch (Exception e)
            { // key not found in AccumMgr
              //  
            }
            return 0.0;
        }

        public void closedTradeChangeNotification(string pair, Dictionary<string, object> map, StateEvent e)
        {
            if (StateEvent.Create == e)
            {
                Dictionary<string, string> closedTradeMap = new Dictionary<string, string>();
                string closedTradeId = "1";
                if (closedTradeId != null)
                    //tradeMap["AccountID"] = otr.AccountID; ;
                    closedTradeMap["Amount"] = "" + map["amount"];
                //tradeMap["OfferID"] = otr.OfferID;
                //tradeMap["OrderID"] = otr.OpenOrderID;
                closedTradeMap["BuySell"] = "" + map["entry"];
                closedTradeMap["OpenRate"] = "" + map["openPrice"];
                closedTradeMap["Pair"] = pair;
                closedTradeId = "" + map["orderId"];

                string grossPL = "[" + map["grossPL"] + "]"; // init with base-currency PL
                double pips = (double)map["pips"];
                int amount = (int)map["amount"];
                double basePL = pips * amount * TransactionManager.getPoiintSize(pair);
                double rate = getSpotRate(pair);
                if (rate != 0.0)
                {
                    grossPL = ""+ basePL / rate;
                }

                log.debug("Close added\t{0}, amount={1}, entry={2}, orderID={3}, tradeID={4}, grossPL={5}, pips={6}, customId={7}, accountId={8}",
                pair, map["amount"], map["entry"], map["orderId"], map["tradeId"], grossPL, map["pips"], map["customId"], "simuated");
            }
            else if (StateEvent.Delete == e)
            {
                   // TODO--print a log entry

            }
        }

        public bool hasOrderInPair(string pair)
        {
            throw new NotImplementedException();
        }

        // gets only the first order (FIFO)
        public Dictionary<string, object> getOrder(string pair)
        {
            throw new NotImplementedException();
        }

        // gets the list of orders
        public List<string> getOrders(string pair)
        {
            throw new NotImplementedException();
        }

        // gets the list of orders by types
        public List<string> getOrdersByType(string pair, string type)
        {
            throw new NotImplementedException();
        }

        // gets the list of orders by types
        public List<string> getOrdersByTypes(string pair, List<string> types)
        {
            throw new NotImplementedException();
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
            TransactionManager.getInstance().getOrder().createMarketOrder(pair, DateTime.Now, buySell, amount, last, customId, stopPips, limitPips);
        }

        public void enterPosition(string pair, string buySell, double last, int stopPips, bool trailStop, int limitPips, int amount, string customId)
        {
            TransactionManager.getInstance().getOrder().createMarketOrder(pair, DateTime.Now, buySell, amount, last, customId, stopPips, limitPips, trailStop);
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
