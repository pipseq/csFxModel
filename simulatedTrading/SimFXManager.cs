using fxcore2;
using Common.fx;
//using fxCoreLink;
using System;
using System.Collections.Generic;

namespace simulatedTrading
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
            //throw new NotImplementedException();
        }

        public void closeOcoTrade()
        {
            throw new NotImplementedException();
        }

        public void closeSession()
        {
            //throw new NotImplementedException();
        }

        public IClosePositions getClosePositions()
        {
            throw new NotImplementedException();
        }

        public IDeleteOrders getCurrentActiveDeleteOrders()
        {
            throw new NotImplementedException();
        }

        public IDeleteOrders getDeleteOrders()
        {
            throw new NotImplementedException();
        }

        public IELSTrade getElsTrade()
        {
            throw new NotImplementedException();
        }

        public IHistoricPrices getHistoricPrices()
        {
            throw new NotImplementedException();
        }

        public IMarketTrade getMarketTrade()
        {
            return new MarketTradeSimulated();
        }

        public IOCOTrade getOcoTrade()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> getOfferForPair(string pair)
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
