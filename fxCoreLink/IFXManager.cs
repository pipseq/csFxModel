﻿using System.Collections.Generic;
using fxcore2;

namespace fxCoreLink
{
    public interface IFXManager
    {
        string AccountID { get; set; }
        O2GSession Session { get; set; }

        void closeClosePositions();
        void closeDeleteOrders();
        void closeElsTrade();
        void closeHistoricPrices();
        void closeMarketTrade();
        void closeOcoTrade();
        void closeSession();
        ClosePositions getClosePositions();
        DeleteOrders getCurrentActiveDeleteOrders();
        DeleteOrders getDeleteOrders();
        ELSTrade getElsTrade();
        HistoricPrices getHistoricPrices();
        IMarketTrade getMarketTrade();
        OCOTrade getOcoTrade();
        void getSession();
        Dictionary<string, Dictionary<string, string>> getTable(string sTable);
        Dictionary<string, string> getOfferForPair(string pair);
        bool hasMargin();
        bool hasMargin(double pct);
        List<O2GOrderRow> loadOrders();
        List<O2GTradeRow> loadTrades();
    }
}