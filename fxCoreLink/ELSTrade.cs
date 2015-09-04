using Common;
using Common.fx;
using fxcore2;
using System;
using System.Threading;

namespace fxCoreLink
{
    public class ELSTrade : IELSTrade
    {
        private O2GSession mSession;

        Logger log = Logger.LogManager("ELSTrade");
        string mOfferID = string.Empty;
        string mAccountID = string.Empty;
        bool mHasAccount = false;
        bool mHasOffer = false;
        double mRate = 0.0;
        double mRateStop = 0.0;
        double pegStopOffset = 0.0;
        int trailStepStop = 1;  // dynamic

		public ELSTrade(O2GSession session,Display display)
        {
            this.mSession = session;
            this.display = display;
        }
        private Display display;

        // returns OrderID
        public string trade(string mInstrument, int mAmount, string mBuySell, string mOrderType, int entryPips, int stopPips)
        {
            string orderId = null;

            ELSResponseListener responseListener = new ELSResponseListener(mSession, display);
            try
            {
                mSession.subscribeResponse(responseListener);

                GetAccount(mSession);
                if (mHasAccount)
                    GetOfferRate(mSession, mInstrument, mBuySell, mOrderType, entryPips, stopPips);

                if (mHasAccount && mHasOffer)
                {
                    O2GRequestFactory requestFactory = mSession.getRequestFactory();
                    if (requestFactory != null)
                    {
                        O2GValueMap valueMap = requestFactory.createValueMap();
                        valueMap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
                        valueMap.setString(O2GRequestParamsEnum.OrderType, mOrderType);
                        valueMap.setString(O2GRequestParamsEnum.AccountID, mAccountID);
                        valueMap.setString(O2GRequestParamsEnum.OfferID, mOfferID);
                        valueMap.setString(O2GRequestParamsEnum.BuySell, mBuySell);
                        valueMap.setDouble(O2GRequestParamsEnum.Rate, mRate);
                        valueMap.setInt(O2GRequestParamsEnum.Amount, mAmount);
                        valueMap.setString(O2GRequestParamsEnum.CustomID, "EntryOrderWithStopLimit");
                        //valueMap.setDouble(O2GRequestParamsEnum.RateStop, mRateStop);
                        valueMap.setString(O2GRequestParamsEnum.PegTypeStop, Constants.Peg.FromClose);
                        valueMap.setDouble(O2GRequestParamsEnum.PegOffsetStop, pegStopOffset);
                        // # of pips in notation of instrument (i.e. 0.005 for 5 pips on USD/JPY)
                        valueMap.setInt(O2GRequestParamsEnum.TrailStepStop, trailStepStop);
                        //valueMap.setDouble(O2GRequestParamsEnum.TrailStep, 0.1);
                        log.debug(string.Format("Order request {0} {1}, at {2}, stop={3}",
                            mInstrument, mBuySell, mRate, pegStopOffset));
                        O2GRequest request = requestFactory.createOrderRequest(valueMap);
                        CtrlTimer.getInstance().startTimer("ELSRequest");
                        mSession.sendRequest(request);
                        //Thread.Sleep(1000);
                        //responseListener.manualEvent.WaitOne(1000, false);
                        responseListener.manualEvent.WaitOne();
                        if (!responseListener.Error)
                        {
                            log.debug("Created an entry order with stop and limit orders for " + mInstrument + ".\n");
                            log.debug("OrderID=" + responseListener.OrderId);
                            orderId = responseListener.OrderId;
                        }
                        else
                            log.debug("Failed to create an entry order for " + mInstrument + "; "
                                + responseListener.ErrorDescription + "; "
                                + requestFactory.getLastError()
                                );
                    }
                }

            }
            catch (Exception e)
            {
                log.debug("Exception: {0}", e.ToString());
            }
            finally
            {
                mSession.unsubscribeResponse(responseListener);
            }
            return orderId;
        }

        // Get current prices and calculate order price
        public void GetOfferRate(O2GSession session, string sInstrument, string mBuySell, string mOrderType, int entryPips,int stopPips)
        {
            double dBid = 0.0;
            double dAsk = 0.0;
            double dPointSize = 0.0;
            try
            {
                O2GLoginRules loginRules = session.getLoginRules();
                if (loginRules != null && loginRules.isTableLoadedByDefault(O2GTableType.Offers))
                {
                    O2GResponse offersResponse = loginRules.getTableRefreshResponse(O2GTableType.Offers);
                    O2GResponseReaderFactory responseFactory = session.getResponseReaderFactory();
                    O2GOffersTableResponseReader offersReader = responseFactory.createOffersTableReader(offersResponse);
                    for (int i = 0; i < offersReader.Count; i++)
                    {
                        O2GOfferRow offer = offersReader.getRow(i);
                        if (sInstrument == offer.Instrument)
                        {
                            mOfferID = offer.OfferID;
                            dBid = offer.Bid;
                            dAsk = offer.Ask;
                            dPointSize = offer.PointSize;

                            // For the purpose of this example we will place entry order 8 pips from the current market price
                            // and attach stop and limit orders 10 pips from an entry order price
                            if (mOrderType == Constants.Orders.LimitEntry)
                            {
                                if (mBuySell == Constants.Buy)
                                {
                                    mRate = dAsk - entryPips * dPointSize;
                                   // mRateLimit = mRate + 10 * dPointSize;
                                    mRateStop = mRate - stopPips * dPointSize;
                                    pegStopOffset = stopPips;
                                }
                                else
                                {
                                    mRate = dBid + entryPips * dPointSize;
                                   // mRateLimit = mRate - 10 * dPointSize;
                                    mRateStop = mRate + stopPips * dPointSize;
                                    pegStopOffset = -stopPips;
                                }
                            }
                            else // use StopEntry!
                            {
                                if (mBuySell == Constants.Buy)
                                {
                                    mRate = dAsk + entryPips * dPointSize;
                                    //mRateLimit = mRate + 10 * dPointSize;
                                    mRateStop = mRate - stopPips * dPointSize;
                                    pegStopOffset = -stopPips;
                                }
                                else // Sell
                                {
                                    mRate = dBid - entryPips * dPointSize;
                                    //mRateLimit = mRate - 10 * dPointSize;
                                    mRateStop = mRate + stopPips * dPointSize;
                                    pegStopOffset = stopPips;
                                }
                            }
                            mHasOffer = true;
                            break;
                        }
                    }
                    if (!mHasOffer)
                        log.debug("You specified invalid instrument. No action will be taken.");
                }
            }
            catch (Exception e)
            {
                log.debug("Exception in GetOfferRate().\n\t " + e.Message);
            }
        }
        // Get account for trade
        public void GetAccount(O2GSession session)
        {
            try
            {
                O2GLoginRules loginRules = session.getLoginRules();
                if (loginRules != null && loginRules.isTableLoadedByDefault(O2GTableType.Accounts))
                {
                    string sAccountID = string.Empty;
                    string sAccountKind = string.Empty;
                    O2GResponse accountsResponse = loginRules.getTableRefreshResponse(O2GTableType.Accounts);
                    O2GResponseReaderFactory responseFactory = session.getResponseReaderFactory();
                    O2GAccountsTableResponseReader accountsReader = responseFactory.createAccountsTableReader(accountsResponse);
                    for (int i = 0; i < accountsReader.Count; i++)
                    {
                        O2GAccountRow account = accountsReader.getRow(i);
                        sAccountID = account.AccountID;
                        sAccountKind = account.AccountKind;
                        if (sAccountKind == "32" || sAccountKind == "36")
                        {
                            mAccountID = sAccountID;
                            mHasAccount = true;
                            break;
                        }
                    }
                    if (!mHasAccount)
                        log.debug("You don't have any accounts available for trading. No action will be taken.");
                }
            }
            catch (Exception e)
            {
                log.debug("Exception in GetAccounts():\n\t " + e.Message);
            }
        }
    }

    public class ELSResponseListener : IO2GResponseListener, IDisposable
    {
        Logger log = Logger.LogManager("ELSResponseListener");
        private Display display;
        O2GSession mSession;
        public ManualResetEvent manualEvent;
        public string OrderId;
        public ELSResponseListener(O2GSession session, Display display)
        {
            this.mSession = session;
            this.display = display;
            manualEvent = new ManualResetEvent(false);
        }
        public bool Error
        {
            get { return mError; }
        }
        private bool mError = false;

        public string ErrorDescription
        {
            get { return mErrorDescription; }
        }
        private string mErrorDescription;

        // Implementation of IO2GResponseListener interface public method onRequestCompleted
        public void onRequestCompleted(String requestID, O2GResponse response)
        {
            // if need to capture response, send to log!
            //log.debug("Request completed.\nrequestID= " + requestID);
            mError = false;
            mErrorDescription = "";
            O2GOrderResponseReader reader = mSession.getResponseReaderFactory().createOrderResponseReader(response);
            this.OrderId = reader.OrderID;
            // TODO: API change dropped these properties
            //mError = !reader.isSuccessful;
            //if (mError)
            //    mErrorDescription = reader.ErrorDescription;
            manualEvent.Set();
            CtrlTimer.getInstance().stopTimer("ELSRequest");
        }

        // Implementation of IO2GResponseListener interface public method onRequestFailed
        public void onRequestFailed(String requestID, String error)
        {
            //log.debug("Request failed.\nrequestID= " + requestID + "; error= " + error);
            mError = true;
            mErrorDescription = error;
            manualEvent.Set();
            CtrlTimer.getInstance().stopTimer("ELSRequest");
        }

        // Implementation of IO2GResponseListener interface public method onTablesUpdates
        public void onTablesUpdates(O2GResponse response)
        {

        }

        public void Dispose()
        {
            manualEvent.Dispose();
        }
    }

}
