
namespace Common.fx
{
    public interface IDeleteOrders
    {
        void deleteOCOs(IFXManager fxManager);
        void deleteOrder(string sOrderID);
        void pendingDeleteOrderNotifyComplete();
    }
}