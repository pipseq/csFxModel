using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fxcore2;

namespace fxCoreLink
{
    class MarketTradeSimulated : IMarketTrade
    {
        public void GetAccount(O2GSession session)
        {
            throw new NotImplementedException();
        }

        public void GetOfferRate(O2GSession session, string sInstrument, string mBuySell, string mOrderType, int stopPips, int limitPips)
        {
            throw new NotImplementedException();
        }

        int cnt = 1;
        public string trade(string mInstrument, int mAmount, string mBuySell, int stopPips, int limitPips, double expectedPrice, string customId)
        {
            return "simTrade" + cnt++;
        }
    }
}
