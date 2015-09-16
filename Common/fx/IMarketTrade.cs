using fxcore2;

namespace Common.fx
{
    public interface IMarketTrade
    {
        string trade(string mInstrument, int mAmount, string mBuySell, int stopPips, bool trailingStop, int limitPips, double expectedPrice, string customId);
        string trade(string mInstrument, int mAmount, string mBuySell, int stopPips, int limitPips, double expectedPrice, string customId);
    }
}