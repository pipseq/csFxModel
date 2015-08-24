using fxcore2;
using System;
using System.Collections.Generic;

namespace fxCoreLink
{
    public class SimFXManager : IFXManager
    {
        public string AccountID
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public O2GSession Session
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void closeClosePositions()
        {
            throw new NotImplementedException();
        }

        public void closeDeleteOrders()
        {
            throw new NotImplementedException();
        }

        public void closeElsTrade()
        {
            throw new NotImplementedException();
        }

        public void closeHistoricPrices()
        {
            throw new NotImplementedException();
        }

        public void closeMarketTrade()
        {
            throw new NotImplementedException();
        }

        public void closeOcoTrade()
        {
            throw new NotImplementedException();
        }

        public void closeSession()
        {
            //throw new NotImplementedException();
        }

        public ClosePositions getClosePositions()
        {
            throw new NotImplementedException();
        }

        public DeleteOrders getCurrentActiveDeleteOrders()
        {
            throw new NotImplementedException();
        }

        public DeleteOrders getDeleteOrders()
        {
            throw new NotImplementedException();
        }

        public ELSTrade getElsTrade()
        {
            throw new NotImplementedException();
        }

        public HistoricPrices getHistoricPrices()
        {
            throw new NotImplementedException();
        }

        public MarketTrade getMarketTrade()
        {
            throw new NotImplementedException();
        }

        public OCOTrade getOcoTrade()
        {
            throw new NotImplementedException();
        }

        public void getSession()
        {
            //throw new NotImplementedException();
        }

        public Dictionary<string, Dictionary<string, string>> getTable(string sTable)
        {
            return new Dictionary<string, Dictionary<string, string>>();
        }

        public bool hasMargin()
        {
            throw new NotImplementedException();
        }

        public bool hasMargin(double pct)
        {
            throw new NotImplementedException();
        }

        public List<O2GOrderRow> loadOrders()
        {
            throw new NotImplementedException();
        }

        public List<O2GTradeRow> loadTrades()
        {
            throw new NotImplementedException();
        }
    }
}
