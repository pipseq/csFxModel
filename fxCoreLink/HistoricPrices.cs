using Common;
using Common.fx;
using fxcore2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace fxCoreLink
{
    public class HistoricPrices : IHistoricPrices
    {
        Logger log = Logger.LogManager("HistoricPrices");
        private O2GSession mSession;
        private Display display;
        private bool debugWrite = false;
        private bool debugRead = false;

        public HistoricPrices(O2GSession session, Display display)
        {
            this.mSession = session;
            this.display = display;
        }

        HistoryResponseListener responseListener;
        // EUR/USD, "m1", 
        public void getHistory(string pair, TimeFrame timeframe, int bars)
        {
            O2GRequestFactory requestFactory = null;
            if (debugRead) return;

            responseListener = new HistoryResponseListener(mSession, display);
            try
            {
                mSession.subscribeResponse(responseListener);

                requestFactory = mSession.getRequestFactory();
                O2GTimeframeCollection timeframes = requestFactory.Timeframes;
                O2GTimeframe tfo = timeframes[ExpertFactory.timeFrameMap[timeframe]];

                O2GRequest request = requestFactory.createMarketDataSnapshotRequestInstrument(pair, tfo, bars);
                requestFactory.fillMarketDataSnapshotRequestTime(request, requestFactory.ZERODATE, requestFactory.ZERODATE, false); 
                CtrlTimer.getInstance().startTimer("HistoryRequest");
                mSession.sendRequest(request);

                responseListener.manualEvent.WaitOne();
                if (!responseListener.Error)
                {
                    //log.debug("You have successfully retrieved history for " + pair );
                    //log.debug("OrderID=" + responseListener.OrderId);
                }
                else
                    log.debug("Your request failed to retrieve data for " + pair );

            }
            catch (Exception e)
            {
                log.debug("getHistory(), exception: "+ e.Message );
                if (requestFactory != null)
                    log.debug("last error = " + requestFactory.getLastError());
                else log.debug("requestFactory is null");
                throw new Exception("Problem with getHistory()");
            }
            finally
            {
                mSession.unsubscribeResponse(responseListener);
            }

        }
        private static string testfile = @"testData.json";
        public Dictionary<DateTime, Dictionary<PriceComponent, object>> getMap()
        {
            Dictionary<DateTime, Dictionary<PriceComponent, object>> map = null;

            if (debugRead)
            {
                map = JsonConvert.DeserializeObject<Dictionary<DateTime, Dictionary<PriceComponent, object>>>(File.ReadAllText(testfile));
            }
            else
            {
                map = responseListener.getMap();
            }
            if (debugWrite)
            {
                string json = JsonConvert.SerializeObject(map, Formatting.Indented);
                File.WriteAllText(testfile, json);
            }
            return map;
        }
    }

    public class HistoryResponseListener : IO2GResponseListener,IDisposable
    {
        private Display display;
        O2GSession mSession;
        public ManualResetEvent manualEvent;
        public string OrderId;
        public HistoryResponseListener(O2GSession session, Display display)
        {
            this.mSession = session;
            this.display = display;
            manualEvent = new ManualResetEvent(false);
        }
        private Dictionary<DateTime, Dictionary<PriceComponent, object>> map = new Dictionary<DateTime, Dictionary<PriceComponent, object>>();
        public Dictionary<DateTime, Dictionary<PriceComponent, object>> getMap()
        {
            return map;
        }
        public bool Error
        {
            get { return mError; }
        }
        private bool mError = false;

        // Implementation of IO2GResponseListener interface public method onRequestCompleted
        public void onRequestCompleted(String requestID, O2GResponse response)
        {
            if (response.Type != O2GResponseType.MarketDataSnapshot)
                return;
                // if need to capture response, send to log!
                //log.debug("Request completed.\nrequestID= " + requestID);
            mError = false;
            O2GResponseReaderFactory factory = mSession.getResponseReaderFactory();
            O2GMarketDataSnapshotResponseReader mReader = factory.createMarketDataSnapshotReader(response);
            for (int i = 0; i < mReader.Count; i++)
            {
                map.Add(mReader.getDate(i), new Dictionary<PriceComponent, object>());
                map[mReader.getDate(i)].Add(PriceComponent.BidClose, mReader.getBidClose(i));
                map[mReader.getDate(i)].Add(PriceComponent.AskClose, mReader.getAskClose(i));
                map[mReader.getDate(i)].Add(PriceComponent.BidHigh, mReader.getBidHigh(i));
                map[mReader.getDate(i)].Add(PriceComponent.BidLow, mReader.getBidLow(i));
                map[mReader.getDate(i)].Add(PriceComponent.BidOpen, mReader.getBidOpen(i));
                map[mReader.getDate(i)].Add(PriceComponent.AskHigh, mReader.getAskHigh(i));
                map[mReader.getDate(i)].Add(PriceComponent.AskLow, mReader.getAskLow(i));
                map[mReader.getDate(i)].Add(PriceComponent.AskOpen, mReader.getAskOpen(i));

                // information like reader.getDate(i), reader.getBidOpen(i), reader.getBidHigh(i), reader.getBidLow(i), reader.getBidClose(i), reader.getVolume(i) is now available
            }


            manualEvent.Set();
            CtrlTimer.getInstance().stopTimer("HistoryRequest");
        }

        // Implementation of IO2GResponseListener interface public method onRequestFailed
        public void onRequestFailed(String requestID, String error)
        {
            //log.debug("Request failed.\nrequestID= " + requestID + "; error= " + error);
            mError = true;
            manualEvent.Set();
            CtrlTimer.getInstance().stopTimer("HistoryRequest");
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
