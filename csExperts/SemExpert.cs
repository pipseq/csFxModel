using System;
using fxCoreLink;
using com = Common;
using Common;
using Common.fx;
using System.Collections.Generic;
using java.io;
using com.hp.hpl.jena.rdf.model;
using org.pipseq.rdf.jena.model;
using Newtonsoft.Json;
using org.pipseq.common.external;

namespace csExperts
{
    /*
    * This MACD-based examples enters positions with stop and limit entry orders.
    * During a timeUpdate() for the selected period
    * if these conditions are met, position entry occurs:
    * - Not currently in a position
    * - 50 period EMA > 0
    * - Current price is above 50 period Ema for buy, below for sell
    * - macd and signal are above 0 for sell, below for buy
    * - macd > signal and previous macd < previous signal for buy, reverse for sell
    */
    [Expert("SemExpert Strategy")]
    public class SemExpert : SemanticExpertBase, IExpert
    {
        private Common.Logger log = Common.Logger.LogManager("SemExpert");

        private static Common.Property props = new Common.Property(typeof (SemExpert));
        static string[] modelFiles = props.getDelimitedListProperty("models");
        static bool diagnostics = props.getBooleanProperty("ruleengine.diagnostics", true);
        static int loglevel = props.getIntProperty("ruleengine.logginglevel", 5);


        object signal; // not used but declared

        // move templates into model?
        private static readonly string emaQueryTemplate = @"
                select ?fep ?sep ?fi ?si ?sig {{
                    ?trig pip:hasSignal ?sig .
                    ?sig pip:hasFastIndicator ?fi .
                    ?sig pip:hasSlowIndicator ?si .
                    ?fi pip:hasInstrument pip:{0} .
                    ?fi pip:hasTimeFrame pip:{1} .
                    ?si pip:hasInstrument pip:{0} .
                    ?si pip:hasTimeFrame pip:{1} .
                    ?fi pip:hasIndicatorSetting ?fis .
                    ?si pip:hasIndicatorSetting ?sis .
                    ?fis pip:period ?fep.
                    ?sis pip:period ?sep.
                }}
            ";

        private static readonly string rsiQueryTemplate = @"
                select ?fep ?fi {{
                    ?trig pip:hasSignal ?sig .
                    ?sig pip:hasIndicator ?fi .
                    ?fi pip:hasInstrument pip:{0} .
                    ?fi pip:hasTimeFrame pip:{1} .
                    ?fi pip:hasIndicatorSetting ?fis .
                    ?fis pip:period ?fep.
                }}
            ";


       public SemExpert(string pair, TimeFrame timeframe) : base(pair, timeframe, modelFiles, diagnostics, loglevel)
        {
            StopLossActive = props.getBooleanProperty("stoploss.active", true);
            StopPips = props.getIntProperty("stoploss", 10);
            LimitActive = props.getBooleanProperty("limit.active", false);
            LimitPips = props.getIntProperty("limit", 10);
            Amount = props.getIntProperty("amount", 1000);
        }

        public override void start()
        {
            Factory.Display.appendLine("SemExpert starting for {0}", Pair);

        }

        public override void stop()
        {
            Factory.Display.appendLine("SemExpert stopping for {0}", Pair);

            //write(model, "test2.TTL");
            //dumpResultsModel(); 
        }
        string dtformat = "MM/dd/yyyy HH:mm:ss.fff";
        int sequence = 100;

        IList<Dictionary<string, object>> m1EmaListMap;
        IList<Dictionary<string, object>> m5EmaListMap;
        IList<Dictionary<string, object>> rsiListMap;
        public override void timeUpdate(TimeFrame timeFrame)
        {
            Dictionary < string, object> map;
            if (timeFrame <= TimeFrame.m5)
            {
                //scope();
                //var a = 1;
                if (Pair == "AUD/JPY")
                {
                    var a = 1;
                    //return;
                }
                if (Pair == "AUD/USD")
                {
                    var a = 1;
                    //return;
                }
                if (m1EmaListMap == null)
                {
                    string query = queryProlog + string.Format(emaQueryTemplate,
                    normalizePair(this.Pair), "m1");// TimePriceComponents.timeFrameMap[timeFrame]);
                    m1EmaListMap = queryListMap(RuleEngine.getTbox().get(), query);
                    if (m1EmaListMap.Count != 1) throw new Exception("Query should have one row result only");
                }
                map = m1EmaListMap[0];
                Int64 fep = (Int64)map["fep"];
                object fastInd = map["fi"];
                Int64 sep = (Int64)map["sep"];
                object slowInd = map["si"];
                object signal = map["sig"];

                double emaFast = getEMA(timeFrame, PriceComponent.BidClose, (int)fep);
                double emaSlow = getEMA(timeFrame, PriceComponent.BidClose, (int)sep);
                double ask = this.getLast(timeFrame, PriceComponent.AskClose);
                double bid = this.getLast(timeFrame, PriceComponent.BidClose);

                // create ema results
                sequence++;
                putResult(emaFast, fastInd, "pip:EMAResult", "pip:hasIndicatorResult", "f_" + sequence);
                putResult(emaSlow, slowInd, "pip:EMAResult", "pip:hasIndicatorResult", "s_" + sequence);
                putResult(ask, signal, "pip:Ask", "pip:hasAsk", sequence);
                putResult(bid, signal, "pip:Bid", "pip:hasBid", sequence);

                //writeData(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
                //    "EMA", Pair, timeFrame, ask, bid, emaFast, emaSlow, Clock.Now().ToString(dtformat)));
                //base.timeUpdate(timeFrame, null);
            }
            if (timeFrame == TimeFrame.m5)
            {
                if (m5EmaListMap == null)
                {
                    string query = queryProlog + string.Format(emaQueryTemplate,
                    normalizePair(this.Pair), "m5");// TimePriceComponents.timeFrameMap[timeFrame]);
                    m5EmaListMap = queryListMap(RuleEngine.getTbox().get(), query);
                    if (m5EmaListMap.Count != 1) throw new Exception("Query should have one row result only");
                }
                map = m5EmaListMap[0];
                Int64 fep = (Int64)map["fep"];
                object fastInd = map["fi"];
                Int64 sep = (Int64)map["sep"];
                object slowInd = map["si"];
                object signal = map["sig"];

                double emaFast = getEMA(timeFrame, PriceComponent.BidClose, (int)fep);
                double emaSlow = getEMA(timeFrame, PriceComponent.BidClose, (int)sep);
                double ask = this.getLast(timeFrame, PriceComponent.AskClose);
                double bid = this.getLast(timeFrame, PriceComponent.BidClose);

                // create ema results
                sequence++;
                putResult(emaFast, fastInd, "pip:EMAResult", "pip:hasIndicatorResult", "f_" + sequence);
                putResult(emaSlow, slowInd, "pip:EMAResult", "pip:hasIndicatorResult", "s_" + sequence);
                putResult(ask, signal, "pip:Ask", "pip:hasAsk", sequence);
                putResult(bid, signal, "pip:Bid", "pip:hasBid", sequence);

                //writeData(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
                //    "EMA", Pair, timeFrame, ask, bid, emaFast, emaSlow, Clock.Now().ToString(dtformat)));
                //base.timeUpdate(timeFrame, null);
                if (rsiListMap == null)
                {
                    string query = queryProlog + string.Format(rsiQueryTemplate,
                        normalizePair(this.Pair), TimePriceComponents.timeFrameMap[timeFrame]);
                    rsiListMap = queryListMap(RuleEngine.getTbox().get(), query);
                    if (rsiListMap.Count != 1) throw new Exception("Query should have one row result only");
                }
                map = rsiListMap[0];
                Int64 rp = (Int64)map["fep"];
                object rInd = map["fi"];
                double rsi = getRSI(timeFrame, PriceComponent.BidClose, (int)rp);

                // create rsi results
                sequence++;
                putResult(rsi, rInd, "pip:RSIResult", "pip:hasIndicatorResult", sequence);

                //writeData(string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                //    "RSI", Pair, timeFrame, rsi, Clock.Now().ToString(dtformat)
                //    ));
            }

            base.timeUpdate(timeFrame, null);

        }

        public override void priceUpdate(DateTime datetime, double bid, double ask)
        {
            LastBid = bid;
            LastAsk = ask;
        }

    }

}
