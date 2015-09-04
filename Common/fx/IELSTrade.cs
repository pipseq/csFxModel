using fxcore2;

namespace Common.fx
{
    public interface IELSTrade
    {
        void GetAccount(O2GSession session);
        void GetOfferRate(O2GSession session, string sInstrument, string mBuySell, string mOrderType, int entryPips, int stopPips);
        string trade(string mInstrument, int mAmount, string mBuySell, string mOrderType, int entryPips, int stopPips);
    }
}