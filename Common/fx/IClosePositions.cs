
namespace Common.fx
{
    public interface IClosePositions
    {
        void closePosition(string sTradeID, string sOfferID, string sAccountID, int iAmount, string sBuySell, string seOrderId);
        void closePositions(IFXManager fxManager);
        void DeleteOrder(string sOrderID);
    }
}