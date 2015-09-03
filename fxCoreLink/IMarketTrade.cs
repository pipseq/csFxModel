using fxcore2;

namespace fxCoreLink
{
    public interface IMarketTrade
    {
        void GetAccount(O2GSession session);
        void GetOfferRate(O2GSession session, string sInstrument, string mBuySell, string mOrderType, int stopPips, int limitPips);
        string trade(string mInstrument, int mAmount, string mBuySell, int stopPips, int limitPips, double expectedPrice, string customId);
    }
}