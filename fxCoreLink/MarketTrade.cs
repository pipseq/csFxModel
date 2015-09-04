using Common;
using Common.fx;
using fxcore2;
using System;
using System.Text;
using System.Threading;

namespace fxCoreLink
{
    public class MarketTrade : IMarketTrade
    {
        Logger log = Logger.LogManager("MarketTrade");
        private O2GSession mSession;
        MailSender mailSender = new MailSender();

        string mOfferID = string.Empty;
        string mAccountID = string.Empty;
        bool mHasAccount = false;
        bool mHasOffer = true;
        double pegStopOffset = 0.0;
        double mRateLimit = 0.0;
        double mBid = 0.0;
        double mAsk = 0.0;
        double mPointSize = 0.0;
        int trailStepStop = 1;  // dynamic
        string cond_dist = "n/a";

		public MarketTrade(O2GSession session,Display display,MailSender mailSender, string cond_dist)
        {
            this.mSession = session;
            this.display = display;
            this.mailSender = mailSender;
            this.cond_dist = cond_dist;
        }
        private Display display;

        // returns OrderID
        public string trade(string mInstrument, int mAmount, 
            string mBuySell, int stopPips, int limitPips, double expectedPrice, string customId)
        {
            string returnId= null;
            string mOrderType= "OM";
            O2GRequestFactory requestFactory = null;
            //int entryPips = 0;

            MarketResponseListener responseListener = new MarketResponseListener(mSession, display);
            try
            {
                mSession.subscribeResponse(responseListener);

                GetAccount(mSession);
                if (mHasAccount)
                    GetOfferRate(mSession, mInstrument, mBuySell, mOrderType, stopPips, limitPips);
                if (mHasAccount && mHasOffer)
                {
                    requestFactory = mSession.getRequestFactory();
                    if (requestFactory != null)
                    {
                        O2GValueMap valueMap = requestFactory.createValueMap();
                        valueMap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
                        valueMap.setString(O2GRequestParamsEnum.OrderType, mOrderType);
                        valueMap.setString(O2GRequestParamsEnum.AccountID, mAccountID);
                        valueMap.setString(O2GRequestParamsEnum.OfferID, mOfferID);
                        valueMap.setString(O2GRequestParamsEnum.BuySell, mBuySell);
                        //valueMap.setDouble(O2GRequestParamsEnum.Rate, mRate);
                        valueMap.setInt(O2GRequestParamsEnum.Amount, mAmount);
                        valueMap.setString(O2GRequestParamsEnum.CustomID, customId);
                        //valueMap.setDouble(O2GRequestParamsEnum.RateStop, mRateStop);
                        valueMap.setString(O2GRequestParamsEnum.PegTypeStop, Constants.Peg.FromClose);
                        valueMap.setDouble(O2GRequestParamsEnum.PegOffsetStop, pegStopOffset);
                        if (limitPips != 0)
                            valueMap.setDouble(O2GRequestParamsEnum.RateLimit, mRateLimit);
                        // # of pips in notation of instrument (i.e. 0.005 for 5 pips on USD/JPY)
                        valueMap.setInt(O2GRequestParamsEnum.TrailStepStop, trailStepStop);
                        //valueMap.setDouble(O2GRequestParamsEnum.TrailStep, 0.1);
                        log.debug(string.Format("Market order request, {0} {1}, stop={2}, limPips={3}, expPrice={4}, customId={5}, cond_dist={6}",
                            mInstrument,mBuySell,pegStopOffset, limitPips, expectedPrice, customId, cond_dist));
                        O2GRequest request = requestFactory.createOrderRequest(valueMap);
                        CtrlTimer.getInstance().startTimer("MarketRequest");
                        mSession.sendRequest(request);
                        //Thread.Sleep(1000);
                        //responseListener.manualEvent.WaitOne(1000, false);
                        responseListener.manualEvent.WaitOne();
                        StringBuilder sb = new StringBuilder();
                        if (!responseListener.Error)
                        {
                            sb.Append("Created mkt order, " + mInstrument );
                            sb.Append(", orderID=" + responseListener.OrderId);
                            sb.Append(", customID=" + customId);
                            //orderId = responseListener.OrderId;
                            returnId = mOfferID;
                        }
                        else
                            sb.Append("Failed mkt order: " + mInstrument + "; "
                                + responseListener.ErrorDescription + "; "
                                + "limit=" + mRateLimit + ", trailStepStop=" + trailStepStop
                                + ", bid=" + mBid + ", ask=" + mAsk + ", pegStopOffset=" + pegStopOffset
                                + ", pointSize=" + mPointSize
                                + ", expectedPrice=" + expectedPrice
                                );
                        log.debug(sb.ToString());
                        mailSender.sendMail("mkt", sb.ToString());
                    }
                }

            }
            catch (Exception e)
            {
                log.debug("Exception: {0}", 
                    e.ToString()+"; "+ 
                requestFactory.getLastError());
            }
            finally
            {
                mSession.unsubscribeResponse(responseListener);
            }
            return returnId;
        }

        // Get current prices and calculate order price
        private void GetOfferRate(O2GSession session, string sInstrument, string mBuySell, string mOrderType, 
            int stopPips, int limitPips)
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
                            if (offer.InstrumentType != 1)
                            {  // validity check, TODO-what else?
                                log.debug("Instrument type = " + offer.InstrumentType + " for " + sInstrument);
                                //return;
                            }
                            // TODO--sometimes pointsize is zero, but since not doing limits, it doesn't matter
                            //      returning at this point is causing other failures!!!
                            //
                            if (offer.PointSize == 0.0)
                            {  // validity check, TODO-what else?
                                log.debug("PointSize = " + offer.PointSize + " for " + sInstrument);
                                //return;
                            }

                            mOfferID = offer.OfferID;
                            dBid = offer.Bid;
                            dAsk = offer.Ask;
                            dPointSize = offer.PointSize;
                            mBid = dBid;
                            mAsk = dAsk;
                            mPointSize = dPointSize;

                            if (mBuySell == Constants.Buy)
                            {
                                mRateLimit = dBid + limitPips * dPointSize;
                                pegStopOffset = -stopPips;
                            }
                            else // Sell
                            {
                                mRateLimit = dAsk - limitPips * dPointSize;
                                pegStopOffset = stopPips;
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
        private void GetAccount(O2GSession session)
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

    public class MarketResponseListener : IO2GResponseListener, IDisposable
    {
        Logger log = Logger.LogManager("MarketResponseListener");
        private Display display;
        O2GSession mSession;
        public ManualResetEvent manualEvent;
        public string OrderId;
        public MarketResponseListener(O2GSession session, Display display)
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
            manualEvent.Set();
            CtrlTimer.getInstance().stopTimer("MarketRequest");
        }

        // Implementation of IO2GResponseListener interface public method onRequestFailed
        public void onRequestFailed(String requestID, String error)
        {
            //log.debug("Request failed.\nrequestID= " + requestID + "; error= " + error);
            mError = true;
            mErrorDescription = error;
            manualEvent.Set();
            CtrlTimer.getInstance().stopTimer("MarketRequest");
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
