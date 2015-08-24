using Common;
using fxcore2;
using System;
using System.Collections.Generic;
using System.Threading;

namespace fxCoreLink
{
    public class ClosePositions : IO2GResponseListener, IDisposable
    {
        Logger log = Logger.LogManager("ClosePositions");
        public ClosePositions(O2GSession session, Display display)
        {
            this.session = session;
            this.display = display;
            session.subscribeResponse(this);
        }
        private Display display;
        private O2GSession session;
        public ManualResetEvent manualEvent;
        private string OrderDeleteRequestId;
        private string requestId;
        private string tradeId;
        private string orderId;

        Dictionary<string, Dictionary<string, string>> positionMap = new Dictionary<string, Dictionary<string, string>>();
        public void closePositions(IFXManager fxManager)
        {
            string tn = "TRADES";
            positionMap = fxManager.getTable(tn);
            //fxManager.printMap(tn, orderMap);

            foreach (Dictionary<string, string> map in positionMap.Values)
            {
                string tradeId = map["TradeID"];
                string offerId = map["OfferID"];
                string accountId = map["AccountID"];
                string sAmount = map["Amount"];
                int amount = int.Parse(sAmount);
                string buySell = map["BuySell"];
                string sBuySell = "";
                if (buySell.Equals("B"))      // switch "BuySell" 
                    sBuySell = "S";                 // (if the position in "Buy", close position will be "Sell" and vice versa) 
                else
                    sBuySell = "B";

                closePosition(tradeId, offerId, accountId, -1, sBuySell, null);
            }
        }
        public void DeleteOrder(string sOrderID)
        {
            this.orderId = sOrderID;
            manualEvent = new ManualResetEvent(false);
            O2GRequestFactory factory = session.getRequestFactory();
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.DeleteOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderID, sOrderID);
            O2GRequest request = factory.createOrderRequest(valuemap);
            if (request == null)
            {
                log.error("Delete SLE Order 'request' was null for " + sOrderID);
                return;
            }

            OrderDeleteRequestId = request.RequestID;
            session.sendRequest(request);
            bool signal = manualEvent.WaitOne(); // n seconds wait
            if (!signal)
            {
                log.error("Delete SLE Order request failed for " + sOrderID );
            }
            
        }


        public void closePosition(string sTradeID, string sOfferID, string sAccountID, int iAmount, string sBuySell, string seOrderId)
        {
            CtrlTimer.getInstance().startTimer("ClosePosition");

            if (seOrderId != null)
                DeleteOrder(seOrderId);

            manualEvent = new ManualResetEvent(false);
            //session.subscribeResponse(this);
            bool signal = false;
            int attempts = 3;
            try
            {
                for (int i = 0; !signal && i < attempts; i++)
                {
                    O2GRequestFactory factory = session.getRequestFactory();
                    O2GValueMap valuemap = factory.createValueMap();
                    valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
                    valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.TrueMarketClose);
                    valuemap.setString(O2GRequestParamsEnum.TradeID, sTradeID);             // The indentifier of the trade.
                    valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);         // The identifier of the account .
                    valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);             // The identifier of the instrument.
                    valuemap.setString(O2GRequestParamsEnum.BuySell, sBuySell);             // The order direction: Constants.Sell for "Sell", Constants.Buy for "Buy".
                    if (iAmount != -1)
                        valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                  // The quantity of the instrument to be bought or sold.
                    valuemap.setString(O2GRequestParamsEnum.NetQuantity, "Y");
                    O2GRequest request = factory.createOrderRequest(valuemap);
                    requestId = request.RequestID;
                    session.sendRequest(request);
                    //log.debug("Close position " + sTradeID);
                    tradeId = sTradeID;
                    signal = manualEvent.WaitOne(); // n seconds wait
                }
                // handle 
                if (!signal)
                {
                    log.debug("Close position request failed for "+sTradeID+": " + "no response after "+ attempts+ " attempts");
                }
            }
            finally
            {
                CtrlTimer.getInstance().stopTimer("ClosePosition");
                //session.unsubscribeResponse(this);
            }
        }

        #region IO2GResponseListener Members

        public void onRequestCompleted(string requestId, O2GResponse response)
        {
            if (this.requestId == requestId)
            {
                manualEvent.Set();
                log.debug("Closing, tradeId=" + tradeId);
            }
            if (this.OrderDeleteRequestId == requestId)
            {
                manualEvent.Set();
                log.debug("SLE order deleted, orderId=" + orderId);
            }
        }

        public void onRequestFailed(string requestId, string error)
        {
            if (this.requestId == requestId)
            {
                manualEvent.Set();
                log.debug("Close position request failed: " + error);
            }
            if (this.OrderDeleteRequestId == requestId)
            {
                manualEvent.Set();
                log.debug("Delete SLE order request failed: " + error);
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
