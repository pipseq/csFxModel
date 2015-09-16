using System;
using fxCoreLink;
using Common;
using Common.fx;
using System.Collections.Generic;

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
    [Expert("MACD Expert Strategy")]
    public class MacdExpert : ExpertBase, IExpert
    {
        private Logger log = Logger.LogManager("MacdExpert");

        private static Property props = new Property(typeof (MacdExpert));
        private bool stopLossActive = props.getBooleanProperty("stoploss.active", true);
        private int stopPips = props.getIntProperty("stoploss", 10);
        private bool limitActive = props.getBooleanProperty("limit.active", false);
        private int limitPips = props.getIntProperty("limit", 10);
        private int amount = props.getIntProperty("amount", 1000);
        private bool riskmanagement = props.getBooleanProperty("riskmanagement", true);
        private bool expertCantrade = props.getBooleanProperty("expertCantrade", false);
        private int macdShort = props.getIntProperty("macd.short", 12);
        private int macdLong = props.getIntProperty("macd.long", 26);
        private int macdSignal = props.getIntProperty("macd.signal", 9);
        private int emaPeriods = props.getIntProperty("ema.periods", 50);

        private double macdPrev = 0.0;
        private double signalPrev = 0.0;

        public MacdExpert(string pair, TimeFrame timeframe) : base(pair, timeframe)
        {
        }

        public override void start()
        {
            Factory.Display.appendLine("MacdSample starting for {0}",Pair);
      }

        public override void stop()
        {
            Factory.Display.append("MacdSample stopping");
        }

        public override void timeUpdate(TimeFrame timeFrame)
        {
            //if (timeFrame != TimeFrame.m15)
            //    return;

            double lastBid = getLast(timeFrame, PriceComponent.BidClose);
            double lastAsk = getLast(timeFrame, PriceComponent.AskClose);
            double ema = getEMA(timeFrame, PriceComponent.BidClose, emaPeriods);
            double[] macdArray = getMACD(timeFrame, PriceComponent.BidClose, macdShort, macdLong, macdSignal);
            double macd = macdArray[0];
            double signal = macdArray[1];

            if (!InPosition
                && ema > 0)
            {
                if (macd < signal
                && macdPrev > signalPrev
                && macd > 0
                && signal > 0
                && lastBid < ema)
                {
                    string entry = "SELL";
                    Factory.Display.appendLine(String.Format("Open, {0}, {1}, price={2}", entry, Pair, lastBid));
                    enterPosition(entry, amount, lastBid, stopPips, false, limitPips);

                }
                else if (macd > signal
                && macdPrev < signalPrev
                && macd < 0
                && signal < 0
                && lastAsk > ema)
                {
                    string entry = "BUY";
                    Factory.Display.appendLine(String.Format("Open, {0}, {1}, price={2}", entry, Pair, lastAsk));
                    enterPosition(entry, amount, lastAsk, stopPips, false, limitPips);
                }
            }
            macdPrev = macd;
            signalPrev = signal;

        }

        public override void priceUpdate(DateTime datetime, double bid, double ask)
        {
            //do nothing
        }
    }
}
