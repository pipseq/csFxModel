using Common;
using fxcore2;
using System;
using System.Threading;

namespace fxCoreLink
{
    public class CreateOrder : IO2GResponseListener, IDisposable
    {
        Logger log = Logger.LogManager("CreateOrder");
        private Display display;
        private O2GSession session;
        public ManualResetEvent manualEvent;
        private string OrderRequestId;
        private string requestId;
        private string tradeId;

        public CreateOrder(O2GSession session, Display display)
        {
            this.session = session;
            this.display = display;
            session.subscribeResponse(this);
        }
        public void CreateEntryOrder(string sAccountID, string pair, string sOfferID, double dRate, int iAmount, string sBuySell, string sTradeID)
        {
            string sOrderType = "SE";
            string customId = sTradeID;
            tradeId = sTradeID;
            int dynamicTrailingMode = 1;
            CtrlTimer.getInstance().startTimer("CreateEntryOrder");

            manualEvent = new ManualResetEvent(false);

            try
            {
                O2GRequestFactory factory = session.getRequestFactory();
                O2GValueMap valuemap = factory.createValueMap();
                valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
                valuemap.setString(O2GRequestParamsEnum.OrderType, sOrderType);
                valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);
                valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);
                valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);
                valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);
                valuemap.setDouble(O2GRequestParamsEnum.Rate, dRate);

                valuemap.setString(O2GRequestParamsEnum.CustomID, customId);
                valuemap.setInt(O2GRequestParamsEnum.TrailStep, dynamicTrailingMode);

                log.debug("Create STE Order, {0} {1}, stop={2}, tradeId={3}",
                    pair, sBuySell, dRate, tradeId);
 
                O2GRequest request = factory.createOrderRequest(valuemap);
                requestId = request.RequestID;
                session.sendRequest(request);

                bool signal = manualEvent.WaitOne(); // n seconds wait

                if (!signal)
                {
                    log.debug("CreateEntryOrder request failed for " + sTradeID );
                }
            }
            finally
            {
                CtrlTimer.getInstance().stopTimer("CreateEntryOrder");
            }
        }


        #region IO2GResponseListener Members

        public void onRequestCompleted(string requestId, O2GResponse response)
        {
            if (this.requestId == requestId)
            {
                manualEvent.Set();
                log.debug("Ordering STE for tradeId=" + tradeId);
            }
        }

        public void onRequestFailed(string requestId, string error)
        {
            if (this.requestId == requestId)
            {
                manualEvent.Set();
                log.debug("Ordering STE for tradeId="+tradeId+", request failed: " + error);
            }
        }

        public void onTablesUpdates(O2GResponse data)
        {
        }

        #endregion

        public void Dispose()
        {
            manualEvent.Dispose();
        }
    }

}
