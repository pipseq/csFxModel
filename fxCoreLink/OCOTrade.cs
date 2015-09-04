using Common;
using Common.fx;
using fxcore2;
using System;
using System.Collections.Generic;
using System.Threading;

namespace fxCoreLink
{
    enum Action
    {
        DeleteOrder,
        SetStopLimit,
        EditOrder
    }


    public class OCOTrade : IOCOTrade
    {
        Logger log = Logger.LogManager("OCOTrade");
        public OCOTrade(O2GSession session, Display display)
        {
            this.session = session;
            this.display = display;
        }
        private Display display;
        private O2GSession session;
        double bidRate;
        double askRate;
        int trailStepStop = 1;
        private Dictionary<string, Action> mActions = new Dictionary<string, Action>();
        ELSResponseListener responseListener;

        public string tradeOCO(string accountId, string offerId, double pointSize, string uniqueId, string pair, int amount, int entryPips, int stopPips, int limitPips, double bid, double ask)
        {
            getOfferRate(session, pair, entryPips, bid, ask, pointSize);

            responseListener = new ELSResponseListener(session, display);
            session.subscribeResponse(responseListener);

            int mAmount = amount * 1000;
            createOCO(accountId, pair, mAmount, entryPips, stopPips, limitPips, uniqueId, offerId);

            session.unsubscribeResponse(responseListener);

            return "";// orderId;
        }



        /** Create OCO orders. */
        public void createOCO(string accountId, string pair, int amount, int entryPips, int stopPips, int limitPips, string uniqueId, string offerId)
        {
            string orderId = "";
            O2GRequestFactory factory = session.getRequestFactory();

            O2GValueMap mainValueMap = factory.createValueMap();
            mainValueMap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOCO);

               {    // buy entry
                O2GValueMap valuemap = factory.createValueMap();
                valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
                valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.StopEntry);
                valuemap.setString(O2GRequestParamsEnum.AccountID, accountId);          // The identifier of the account the order should be placed for.
                valuemap.setString(O2GRequestParamsEnum.OfferID, offerId);              // The identifier of the instrument the order should be placed for.
                valuemap.setString(O2GRequestParamsEnum.BuySell, Constants.Buy);              // The order direction (Constants.Buy for buy, Constants.Sell for sell)
                valuemap.setDouble(O2GRequestParamsEnum.Rate, askRate);                    // The dRate at which the order must be filled (below current dRate for Buy, above current dRate for Sell)
                valuemap.setInt(O2GRequestParamsEnum.Amount, amount);                   // The quantity of the instrument to be bought or sold.

                valuemap.setString(O2GRequestParamsEnum.CustomID, "OCOBuy" + uniqueId);
                valuemap.setString(O2GRequestParamsEnum.PegTypeStop, Constants.Peg.FromClose);
                valuemap.setInt(O2GRequestParamsEnum.PegOffsetStop, -stopPips);
                valuemap.setInt(O2GRequestParamsEnum.TrailStepStop, trailStepStop);
                if (limitPips > 0)
                {
                    valuemap.setString(O2GRequestParamsEnum.PegTypeLimit, Constants.Peg.FromClose);
                    valuemap.setInt(O2GRequestParamsEnum.PegOffsetLimit, limitPips);
                }
                mainValueMap.appendChild(valuemap);
            }

            {   // sell entry
                O2GValueMap valuemap = factory.createValueMap();
                valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
                valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Orders.StopEntry);
                valuemap.setString(O2GRequestParamsEnum.AccountID, accountId);          // The identifier of the account the order should be placed for.
                valuemap.setString(O2GRequestParamsEnum.OfferID, offerId);              // The identifier of the instrument the order should be placed for.
                valuemap.setString(O2GRequestParamsEnum.BuySell, Constants.Sell);              // The order direction (Constants.Buy for buy, Constants.Sell for sell)
                valuemap.setDouble(O2GRequestParamsEnum.Rate, bidRate);                    // The dRate at which the order must be filled (below current dRate for Buy, above current dRate for Sell)
                valuemap.setInt(O2GRequestParamsEnum.Amount, amount);                   // The quantity of the instrument to be bought or sold.

                valuemap.setString(O2GRequestParamsEnum.CustomID, "OCOSell"+uniqueId);
                valuemap.setString(O2GRequestParamsEnum.PegTypeStop, Constants.Peg.FromClose);
                valuemap.setInt(O2GRequestParamsEnum.PegOffsetStop, stopPips);
                valuemap.setInt(O2GRequestParamsEnum.TrailStepStop, trailStepStop);
                if (limitPips > 0)
                {
                    valuemap.setString(O2GRequestParamsEnum.PegTypeLimit, Constants.Peg.FromClose);
                    valuemap.setInt(O2GRequestParamsEnum.PegOffsetLimit, -limitPips);
                }
                mainValueMap.appendChild(valuemap);
            }

            O2GRequest request = factory.createOrderRequest(mainValueMap);
            session.sendRequest(request);
            responseListener.manualEvent.WaitOne();
            if (!responseListener.Error)
            {
                log.debug("Created OCO, OrderID={0} at {1}",
                    responseListener.OrderId,
                    CtrlTimer.getTimeNowFormatted());
                orderId = responseListener.OrderId;
            }
            else
                log.debug("Failed to create OCO"
                    + responseListener.ErrorDescription + "; "
                    + factory.getLastError()
                    );
        }

        // Get current prices and calculate order price
        private void getOfferRate(O2GSession session, string sInstrument, int entryPips, double bid, double ask, double pointSize)
        {
            double dBid = bid;
            double dAsk = ask;
            double dPointSize = pointSize;
            try
            {
                bidRate = dBid - entryPips * dPointSize;
                askRate = dAsk + entryPips * dPointSize;

            }
            catch (Exception e)
            {
                log.debug("Exception in GetOfferRate().\n\t " + e.Message);
            }
        }
    }

    class OCOResponseListener : IO2GResponseListener, IDisposable
    {
        Logger log = Logger.LogManager("OCOResponseListener");
        public OCOResponseListener(O2GSession session, Display display)
        {
            this.session = session;
            this.display = display;
            manualEvent = new ManualResetEvent(true);
        }
        private Display display;
        private O2GSession session;
        //private object mEvent = new object();
        public ManualResetEvent manualEvent;
        private int iNewOCOOrders = 0;                                      // number of unprocessed requests (add order to new OCO). Continue after all requests are responded.
        private string sContingencyID = "n/a";                                 // Contingency ID for the new order we just created

        //request IDs
        private List<string> lstCreateNewOCORequestID = new List<string>(); // RequestIDs to create new OCO - in this example several orders added to form new OCO
        private string sJoinExistingOCORequestID = "";                      // RequestID to join existing OCO - in this example just one order joins existing OCO
        private string sRemoveFromOCORequestID = "";                        // RequestID to remove order from OCO - in this example just one order is removed from OCO
        private string sOrdersTableRefreshRequestID = "";                   // requestID to refresh Orders table

        private List<string> lstOrdersInNewOCO;                             // order IDs of orders added to new OCO order

        #region IO2GResponseListener Members

        public string getOrderId()
        {
            return sContingencyID;
        }

        public void onRequestCompleted(string requestId, O2GResponse response)
        {
            bool bLetGo = false;        // do not have to wait if this value is true
            if (iNewOCOOrders > 0)
            {
                foreach (string sCreateNewOCORequestID in lstCreateNewOCORequestID)
                {
                    if (requestId.Equals(sCreateNewOCORequestID))
                    {
                        display.logger("Order added to new OCO");
                        iNewOCOOrders--;
                        break;
                    }
                }
                if (iNewOCOOrders == 0)
                    bLetGo = true;
            }
            else if (requestId.Equals(sRemoveFromOCORequestID))
            {
                display.logger("Order removed from OCO");
                bLetGo = true;
            }
            else if (requestId.Equals(sJoinExistingOCORequestID))
            {
                display.logger("Order added to existing OCO");
                bLetGo = true;
            }
            else if (requestId.Equals(sOrdersTableRefreshRequestID))
            {
                sContingencyID = GetContingencyID(response);
                bLetGo = true;
            }

            if (bLetGo)
            {
                manualEvent.Set();
            }
            CtrlTimer.getInstance().stopTimer("OCORequest");
        }

        public void onRequestFailed(string requestId, string error)
        {
            bool bLetGo = false;
            if (iNewOCOOrders > 0)
            {
                foreach (string sCreateNewOCORequestID in lstCreateNewOCORequestID)
                {
                    if (requestId.Equals(sCreateNewOCORequestID))
                    {
                        display.logger("Cannot add order to new OCO, error=" + error);
                        iNewOCOOrders--;
                        break;
                    }
                }
                if (iNewOCOOrders == 0)
                    bLetGo = true;
            }
            else if (requestId.Equals(sJoinExistingOCORequestID))
            {
                display.logger("Cannot join existing OCO, error=" + error);
                bLetGo = true;
            }
            else if (requestId.Equals(sRemoveFromOCORequestID))
            {
                display.logger("Cannot remove from OCO, error=" + error);
                bLetGo = true;
            }
            else if (requestId.Equals(sOrdersTableRefreshRequestID))
            {
                display.logger("Cannot get refreshed Orders table, error=" + error);
                bLetGo = true;
            }

            if (bLetGo)
            {
                manualEvent.Set();

            }
            CtrlTimer.getInstance().stopTimer("OCORequest");
        }

        public void onTablesUpdates(O2GResponse data)
        {
        }

        #endregion


        public void RemoveFromOCO(string sAccountID, string sOrderRemoveFromOCO)
        {
            O2GRequestFactory factory = session.getRequestFactory();
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.RemoveFromContingencyGroup);

            O2GValueMap valueRemove = createValueMapForExistingOrder(sOrderRemoveFromOCO, sAccountID);
            valueRemove.setString(O2GRequestParamsEnum.Command, Constants.Commands.RemoveFromContingencyGroup);
            valuemap.appendChild(valueRemove);

            // create request from valueMap
            O2GRequest request = factory.createOrderRequest(valuemap);
            if (request != null)
            {
                log.debug("Remove order {0} from OCO", sOrderRemoveFromOCO);
                sRemoveFromOCORequestID = request.getChildRequest(0).RequestID; // just one order to remove in this example
                session.sendRequest(request);
                manualEvent.Reset();
                manualEvent.WaitOne();

            }
            else
            {
                log.debug("Request in \'RemoveFromOCO\' is null, most likely some arguments are mssing or incorrect");
            }
        }

        public void JoinExistingOCO(string sAccountID, string sOrderJoinToOCO)
        {
            if (String.IsNullOrEmpty(sContingencyID))
            {
                log.debug("Cannot join existing OCO - ContingencyID is missing");
                return;
            }
            O2GRequestFactory factory = session.getRequestFactory();
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.JoinToExistingContingencyGroup);
            valuemap.setInt(O2GRequestParamsEnum.ContingencyGroupType, 1); // 1 means OCO
            valuemap.setString(O2GRequestParamsEnum.ContingencyID, sContingencyID);
            valuemap.appendChild(createValueMapForExistingOrder(sOrderJoinToOCO, sAccountID));
            // create request from valueMap
            O2GRequest request = factory.createOrderRequest(valuemap);
            if (request != null)
            {
                log.debug("Add order {0} to existing OCO", sOrderJoinToOCO);
                sJoinExistingOCORequestID = request.getChildRequest(0).RequestID; // only one order joins existing OCO in this sample
                session.sendRequest(request);
                manualEvent.Reset();
                manualEvent.WaitOne();
            }
            else
            {
                log.debug("Request in \'JoinExistingOCO\' is null, most likely some arguments are mssing or incorrect");
            }
        }

        /// <summary>
        /// Create new OCO orders from entry orders
        /// </summary>
        /// <param name="sAccountID">Account ID</param>
        /// <param name="lstOrderIDsCreateNewOCO">list of entry orders to create OCO</param>
        /// <param name="sOrderIDsCreateNewOCO">string with all order IDs to create OCO (for log purposes)</param>
        public void JoinNewOCO(string sAccountID, List<string> lstOrderIDsCreateNewOCO)
        {
            lstOrdersInNewOCO = lstOrderIDsCreateNewOCO;
            O2GRequestFactory factory = session.getRequestFactory();
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.JoinToNewContingencyGroup);
            //set contingencyType for created group
            valuemap.setInt(O2GRequestParamsEnum.ContingencyGroupType, 1);
            string sOrderIDsCreateNewOCO = "";
            int ji = 0;
            foreach (string sOrderID in lstOrderIDsCreateNewOCO)
            {
                valuemap.appendChild(createValueMapForExistingOrder(sOrderID, sAccountID));
                if (ji++ > 0) sOrderIDsCreateNewOCO += ",";
                sOrderIDsCreateNewOCO += sOrderID;
            }

            // create request from valueMap
            O2GRequest request = factory.createOrderRequest(valuemap);
            if (request != null)
            {
                for (int i = 0; i < request.ChildrenCount; i++)
                {
                    O2GRequest childRequest = request.getChildRequest(i);
                    string sChildRequestID = childRequest.RequestID;
                    lstCreateNewOCORequestID.Add(sChildRequestID);
                }
                iNewOCOOrders = request.ChildrenCount;
                log.debug("Create new OCO for orders {0}", sOrderIDsCreateNewOCO);
                CtrlTimer.getInstance().startTimer("OCORequest");

                session.sendRequest(request);
                manualEvent.Reset();
                manualEvent.WaitOne();
            }
            else
            {
                log.debug("Request in \'JoinNewOCO\' is null, most likely some arguments are mssing or incorrect");
            }
        }

        private O2GValueMap createValueMapForExistingOrder(string sOrderID, string sAccountID)
        {
            O2GRequestFactory factory = session.getRequestFactory();
            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.OrderID, sOrderID);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);
            return valuemap;
        }

        /// <summary>
        /// Used to get refreshed Orders table so we can extract Contingency ID for newly created OCO on response
        /// </summary>
        public void GetOrdersTable(string sAccountID)
        {
            O2GRequestFactory requestFactory = session.getRequestFactory();
            O2GRequest request = requestFactory.createRefreshTableRequestByAccount(O2GTableType.Orders, sAccountID);
            if (request != null)
            {
                sOrdersTableRefreshRequestID = request.RequestID;
                session.sendRequest(request);
                manualEvent.Reset();
                manualEvent.WaitOne();
            }
            else
            {
                log.debug("Request in \'GetOrdersTable\' is null, most likely some arguments are mssing or incorrect");
            }
        }

        /// <summary>
        /// Get Contingency ID for newly created OCO order
        /// </summary>
        private string GetContingencyID(O2GResponse response)
        {
            string sContingencyID = "";
            O2GResponseReaderFactory readerFactory = session.getResponseReaderFactory();
            if (readerFactory != null)
            {
                O2GOrdersTableResponseReader reader = readerFactory.createOrdersTableReader(response);
                int i;
                bool bOrderFromNewOCOFound = false;
                for (i = 0; i < reader.Count; i++)
                {
                    O2GOrderRow row = reader.getRow(i);
                    string sOrderID = row.OrderID;
                    foreach (string sOrderInNewOCO in lstOrdersInNewOCO) // find the order from the list and get ContingencyID for that order
                    {
                        if (sOrderID.Equals(sOrderInNewOCO))
                        {
                            sContingencyID = row.ContingentOrderID;
                            bOrderFromNewOCOFound = true;
                            break;
                        }
                    }
                    if (bOrderFromNewOCOFound)
                        break;
                }
            }
            return sContingencyID;
        }


        public void Dispose()
        {
            manualEvent.Dispose();
        }
    }

}
