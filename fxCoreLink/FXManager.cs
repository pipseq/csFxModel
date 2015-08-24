using Common;
using fxcore2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace fxCoreLink
{
    public class FXManager : IFXManager
    {
        Logger log = Logger.LogManager("FXManager");
        private O2GSession session;

        public O2GSession Session
        {
            get { return session; }
            set { session = value; }
        }
        private Display display;
        private SessionStatusListener statusListener;
        private static Dictionary<string, string> tableIdentities = new Dictionary<string, string>();
        private string user = "";
        private string pw = "";
        private string url = "";
        private string connection = "";
        private int sessionNestCounter = 0;
        private MailSender mailSender;
        private string accountID;

        public string AccountID
        {
            get { return accountID; }
            set { accountID = value; }
        }


        static FXManager()
        {
            int i = 0;

            foreach (string key in new string[] { "ACCOUNTS", "OFFERS", "ORDERS", "TRADES", "CLOSEDTRADES" })
            {
                tableIdentities[key] = new string[] { "AccountID", "Instrument", "OrderID", "TradeID", "ClosedTradeID" }[i++];
            }
        }

        public FXManager(Display display)
        {
            this.display = display;
        }

        public FXManager(Display display, string user, string pw, string url, string connection, MailSender mailSender)
        {
            this.display = display;
            this.user = user;
            this.pw = pw;
            this.url = url;
            this.connection = connection;
            this.mailSender = mailSender;
            initialize();
        }

        private void initialize()
        {
            getSession();
            GetAccount(Session);
            closeSession();
        }

        private void GetAccount(O2GSession session)
        {
            try
            {
                O2GLoginRules loginRules = session.getLoginRules();
                if (loginRules != null && loginRules.isTableLoadedByDefault(O2GTableType.Accounts))
                {
                    string sAccountID = string.Empty;
                    string sAccountKind = string.Empty;
                    O2GResponse accountsResponse = loginRules.getTableRefreshResponse(O2GTableType.Accounts);
                    O2GResponseReaderFactory responseFactory = session.getResponseReaderFactory();
                    O2GAccountsTableResponseReader accountsReader = responseFactory.createAccountsTableReader(accountsResponse);
                    for (int i = 0; i < accountsReader.Count; i++)
                    {
                        O2GAccountRow account = accountsReader.getRow(i);
                        sAccountID = account.AccountID;
                        sAccountKind = account.AccountKind;
                        if (sAccountKind == "32" || sAccountKind == "36")
                        {
                            accountID = sAccountID;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.debug("Exception in GetAccounts():\n\t " + e.Message);
            }
        }
        public void getSession()
        {
            if (sessionNestCounter++ > 0)
                return; // nested session

            //Create a session:
            session = O2GTransport.createSession();
            // NOTE: API change requires SessionStatusListener or null
            session.useTableManager(O2GTableManagerMode.Yes,null);
            //Create an object of a status listener class: 
            statusListener = new SessionStatusListener(display);
            //Subscribe the status listener object to the session status. It is important to subscribe before the login:
            session.subscribeSessionStatus(statusListener);
            CtrlTimer.getInstance().startTimer("Session");
            //Log in using the user ID, password, URL, and connection:
            session.login(user, pw, url, connection);
            statusListener.manualEvent.WaitOne();

            int maxTries = 3;
            for (int i=0; i< maxTries && !statusListener.Connected; i++)
            {
                Thread.Sleep(100);
            }
            if (statusListener.TradingClosed)
            {
                throw new System.ComponentModel.WarningException("Trading closed");
            }
            if (statusListener.Disconnected)
            {
                throw new System.ComponentModel.WarningException("Login failed");
            }
        }

        public void closeSession()
        {
            if (session != null
                && --sessionNestCounter == 0)
            {
                CtrlTimer.getInstance().startTimer("Session");
                session.logout();
                session.Dispose();
                session = null;
                statusListener.manualEvent.WaitOne();
            }
        }

        // assumes open session
        public bool hasMargin()
        {
            return hasMargin(0.01); // one pct
        }
        public bool hasMargin(double pct)
        {
            Dictionary<string, Dictionary<string, string>> accountMap = getTable("ACCOUNTS");
            if (accountMap.Keys.Count != 1)
                throw new Exception("More than one account");
            string acct = "";
            foreach (string key in accountMap.Keys)
                acct = key; // just one
            double balance = double.Parse(accountMap[acct]["Balance"]);
            double usedMargin = double.Parse(accountMap[acct]["UsedMargin"]);
            double pctUsed = usedMargin / balance;
            return pctUsed < pct;
        }

        private HistoricPrices historicPrices;
        private MarketTrade marketTrade;
        private ELSTrade elsTrade;
        private OCOTrade ocoTrade;
        private DeleteOrders deleteOrders;
        private ClosePositions closePositions;

        public HistoricPrices getHistoricPrices()
        {
            getSession();
            if (historicPrices == null)
                historicPrices = new HistoricPrices(session, display);
            return historicPrices;
        }

        public void closeHistoricPrices()
        {
            historicPrices = null;
            closeSession();
        }

        public MarketTrade getMarketTrade()
        {
            getSession();
            if (marketTrade == null)
                marketTrade = new MarketTrade(session, display, mailSender);
            return marketTrade;
        }

        public void closeMarketTrade()
        {
            marketTrade = null;
            closeSession();
        }

        public ELSTrade getElsTrade()
        {
            getSession();
            if (elsTrade == null)
                elsTrade = new ELSTrade(session, display);
            return elsTrade;
        }

        public void closeElsTrade()
        {
            elsTrade = null;
            closeSession();
        }

        public OCOTrade getOcoTrade()
        {
            getSession();
            if (ocoTrade == null)
                ocoTrade = new OCOTrade(session, display);
            return ocoTrade;
        }

        public void closeOcoTrade()
        {
            ocoTrade = null;
            closeSession();
        }

        public DeleteOrders getDeleteOrders()
        {
            getSession();
            if (deleteOrders == null)
                deleteOrders = new DeleteOrders(session, display);
            return deleteOrders;
        }

        public DeleteOrders getCurrentActiveDeleteOrders()
        {
            return deleteOrders;
        }

        public void closeDeleteOrders()
        {
            deleteOrders = null;
            closeSession();
        }

        public ClosePositions getClosePositions()
        {
            getSession();
            if (closePositions == null)
                closePositions = new ClosePositions(session, display);
            return closePositions;
        }

        public void closeClosePositions()
        {
            closePositions = null;
            closeSession();
        }


        private static string[] arrValidTables = { "OFFERS", "ACCOUNTS", "ORDERS", "TRADES", "CLOSEDTRADES", "MESSAGES", "SUMMARY" };

        private static O2GTableType tableType;

        public Dictionary<string, Dictionary<string, string>> getTable(string sTable)
        {
            Dictionary<string, Dictionary<string, string>> curMap = new Dictionary<string, Dictionary<string, string>>();

            try
            {
                CtrlTimer.getInstance().startTimer("getTable." + sTable);
                tableType = GetTableType(sTable);
                curMap = loadMap(sTable);
            }
            catch (Exception ex)
            {
                log.debug("getTable exception: " + sTable + ", " + ex.Message);
            }
            finally
            {
                CtrlTimer.getInstance().stopTimer("getTable." + sTable);
            }
            //putRecentTable(sTable, curMap);
            return curMap;
        }

        /// <summary>
        /// Get O2GTableType based on the tables's title
        /// </summary>
        /// <param name="sTableType">Name of the table (like "ORDERS")</param>
        /// <returns>Type of the table (like O2GTableType.Orders)</returns>
        static O2GTableType GetTableType(string sTableType)
        {
            if (String.Equals(sTableType, "OFFERS"))
            {
                return O2GTableType.Offers;
            }
            else if (String.Equals(sTableType, "ACCOUNTS"))
            {
                return O2GTableType.Accounts;
            }
            else if (String.Equals(sTableType, "ORDERS"))
            {
                return O2GTableType.Orders;
            }
            else if (String.Equals(sTableType, "TRADES"))
            {
                return O2GTableType.Trades;
            }
            else if (String.Equals(sTableType, "CLOSEDTRADES"))
            {
                return O2GTableType.ClosedTrades;
            }
            else if (String.Equals(sTableType, "MESSAGES"))
            {
                return O2GTableType.Messages;
            }
            else if (String.Equals(sTableType, "SUMMARY"))
            {
                return O2GTableType.Summary;
            }
            else // should not fall here after validaiton
            {
                return O2GTableType.TableUnknown;
            }

        }
        private Dictionary<string, Dictionary<string, string>> loadMap(string tableName)
        {
            Dictionary<string, Dictionary<string, string>> curMap = new Dictionary<string, Dictionary<string, string>>();
            O2GTableManager tableMgr = session.getTableManager();
            O2GTable aTable = tableMgr.getTable(tableType);

            int iCountRows = aTable.Count;
            int iCountCols = aTable.Columns.Count;
            if (iCountRows > 0)
            {
                for (int i = 0; i < iCountRows; i++)
                {
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    for (int j = 0; j < iCountCols; j++)
                        map.Add(aTable.Columns[j].ID, "" + aTable.getCell(i, j));
                    string pair = map[tableIdentities[tableName]];
                    curMap.Add(pair, map);
                }
            }
            return curMap;
        }
        public List<O2GTradeRow> loadTrades()
        {
            List<O2GTradeRow> lTrades = new List<O2GTradeRow>();
            O2GTableManager tableMgr = session.getTableManager();
            O2GTable aTable = tableMgr.getTable(O2GTableType.Trades);

            int iCountRows = aTable.Count;
            if (iCountRows > 0)
            {
                for (int i = 0; i < iCountRows; i++)
                {
                    O2GTradeRow tr = (O2GTradeRow)aTable.getGenericRow(i);
                    lTrades.Add(tr);
                }
            }
            return lTrades;
        }
        public List<O2GOrderRow> loadOrders()
        {
            List<O2GOrderRow> lOrders = new List<O2GOrderRow>();
            O2GTableManager tableMgr = session.getTableManager();
            O2GTable aTable = tableMgr.getTable(O2GTableType.Orders);

            int iCountRows = aTable.Count;
            if (iCountRows > 0)
            {
                for (int i = 0; i < iCountRows; i++)
                {
                    O2GOrderRow tr = (O2GOrderRow)aTable.getGenericRow(i);
                    lOrders.Add(tr);
                }
            }
            return lOrders;
        }
        public static void printMap(Logger log, string name, Dictionary<string, Dictionary<string, string>> curMap)
        {
            log.debug(name);
            foreach (string pair in curMap.Keys)
            {
                StringBuilder sb = new StringBuilder();
                Dictionary<string, string> map = curMap[pair];
                sb.AppendLine(string.Format(
                        "key={0}", pair
                        ));
                foreach (string key in map.Keys)
                {
                    sb.AppendLine(
                        string.Format(
                        "\t{0}={1}", key, map[key]
                        )
                        );
                }
                log.debug(sb.ToString());
            }
        }
        // NEED LIST OF MAP
        void PrintTheTable()
        {
            Console.Write("\n");
            O2GTableManager tableMgr = session.getTableManager();
            O2GTable aTable = tableMgr.getTable(tableType);

            int iCountRows = aTable.Count;
            int iCountCols = aTable.Columns.Count;
            if (iCountRows == 0)
                Console.WriteLine("The table is empty\n");
            else
            {
                for (int i = 0; i < iCountRows; i++)
                {
                    for (int j = 0; j < iCountCols; j++)
                        Console.Write(aTable.Columns[j].ID + "=" + aTable.getCell(i, j) + "; ");
                    Console.WriteLine("\n");
                }
                Console.WriteLine();
            }
        }


        /*
	OfferID=13
	Instrument=GBP/CHF
	QuoteID=FXCM_U100D3_DESK_00000000001124169600
	Bid=1.52958
	Ask=1.52991
	Low=1.52946
	High=1.53049
	Volume=1
	Time=10/30/2014 9:51:00 PM
	BidTradable=T
	AskTradable=T
	SellInterest=-0.33
	BuyInterest=0.13
	ContractCurrency=GBP
	Digits=5
	PointSize=0.0001
	SubscriptionStatus=D
	InstrumentType=1
	ContractMultiplier=1
	TradingStatus=O
	ValueDate=11042014
	PipCost=1.04638083030319

         */


    }

}
