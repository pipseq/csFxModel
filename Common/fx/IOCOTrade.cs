namespace Common.fx
{
    public interface IOCOTrade
    {
        void createOCO(string accountId, string pair, int amount, int entryPips, int stopPips, int limitPips, string uniqueId, string offerId);
        string tradeOCO(string accountId, string offerId, double pointSize, string uniqueId, string pair, int amount, int entryPips, int stopPips, int limitPips, double bid, double ask);
    }
}