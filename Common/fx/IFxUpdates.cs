﻿using System.Collections.Generic;
using Common;

namespace Common.fx
{
    public interface IFxUpdates
    {
        IAccumulatorMgr AccumMgr { get; set; }
        Control Control { get; set; }
        bool Debug { get; set; }
        Display Display { get; }
        IFXManager FxManager { get; set; }
        string UniqueId { get; }

        void closePosition(string pair);
        void createStopEntryOrder(string pair, string accountId, string tradeId, string amount, string offerId, string buySell, double rate);
        void deleteOrder(string pair);
        void enterOcoEntry(string pair, int amount, int spread, int stopPips, int limitPips, double bid, double ask);
        void enterPosition(string pair, string buySell, double last, int stopPips, int amount, string customId);
        void enterPosition(string pair, string buySell, double last, int stopPips, int limitPips, int amount, string customId);
        Dictionary<string, string> getOrder(string pair);
        List<string> getOrders(string pair);
        List<string> getOrdersByType(string pair, string type);
        List<string> getOrdersByTypes(string pair, List<string> types);
        string getPairOfferId(string pair);
        double getPairPointSize(string pair);
        Dictionary<string, string> getTrade(string pair);
        bool hasOrderInPair(string pair);
        bool hasPosition(string tradeId);
        bool hasPositionInPair(string pair);
        void readRows();
        void run(object pairObj);
        void runClose();
        void runDeleteOrder();
        void runOpen();
        void runOpenOco();
        void setCanProcess(bool val);
    }
}