﻿using System;
using Common.fx;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fxcore2;
using fxCoreLink;

namespace simulatedTrading
{
    class MarketTradeSimulated : IMarketTrade
    {
        // TODO--is this going to be used??!!
        public string trade(string mInstrument, int mAmount, string mBuySell, int stopPips, bool trailingStop, int limitPips, double expectedPrice, string customId)
        {
            string orderid = TransactionManager.getInstance().getOrder()
                .createMarketOrder(mInstrument, DateTime.Now, mBuySell, mAmount, expectedPrice, customId, stopPips, limitPips,trailingStop);
            return orderid;
        }

        public string trade(string mInstrument, int mAmount, string mBuySell, int stopPips, int limitPips, double expectedPrice, string customId)
        {
            string orderid = TransactionManager.getInstance().getOrder()
                .createMarketOrder(mInstrument, DateTime.Now, mBuySell, mAmount, expectedPrice, customId, stopPips, limitPips);
            return orderid;
        }
    }
}
