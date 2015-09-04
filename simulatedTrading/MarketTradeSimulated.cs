using System;
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
        int cnt = 1;
        public string trade(string mInstrument, int mAmount, string mBuySell, int stopPips, int limitPips, double expectedPrice, string customId)
        {
            return "simTrade" + cnt++;
        }
    }
}
