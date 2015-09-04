using System;
using fxCoreLink;
using Common;
using Common.fx;
using System.Collections.Generic;

namespace csExperts
{
    /*
    * This ADX-RSI-based examples enters positions with stop and limit entry orders.
    * The strategy attempts to catch a bounce during a reversal in a trend
    * During a timeUpdate() for the selected period
    * if these conditions are met, position entry occurs:
    * - Not currently in a position
    * - 50 period EMA > 0
    * - Current price is above 50 period Ema for buy, below for sell at m15
    * - RSI at m15 is in overbought or oversold territory at m15
    * - ADX indicates the pair is currently trending at m1
    * - The DMI shows m1 trend is reverse minus above plus
    * - The m1 reverse ends with a bounce signal,
    *   the stochastic %K below the %D for sell, above for buy
    */
    [Expert("Adx Rsi Expert Strategy")]
    public class AdxRsiExpert : ExpertBase, IExpert
    {
        Logger log = Logger.LogManager("AdxRsiExpert");

        private static Property props = new Property(typeof(AdxRsiExpert));
        private bool stopLossActive = props.getBooleanProperty("stoploss.active", true);
        private int stopPips = props.getIntProperty("stoploss", 10);
        private bool limitActive = props.getBooleanProperty("limit.active", false);
        private int limitPips = props.getIntProperty("limit", 10);
        private int amount = props.getIntProperty("amount", 1000);
        private bool riskmanagement = props.getBooleanProperty("riskmanagement", true);
        private bool expertCantrade = props.getBooleanProperty("expertCantrade", false);
        private int emaPeriods = props.getIntProperty("ema.periods", 50);
        private int stochPeriods = props.getIntProperty("stoch.periods", 5); // %K periods (%D slowing and %D periods are 3)
        private int rsiPeriods = props.getIntProperty("rsi.periods", 14);
        private int adxPeriods = props.getIntProperty("adx.periods", 14);
        private int RSIThresholdOverBought = 70;
        private int RSIThresholdOverSold = 30;
        private int ADXThreshold = props.getIntProperty("adx.periods", 50);

        public AdxRsiExpert(string pair, TimeFrame timeframe) : base(pair, timeframe)
        {
        }

        public override void start()
        {
            Factory.Display.appendLine("AdxRsiExpert starting");
        }

        public override void stop()
        {
            Factory.Display.append("AdxRsiExpert stopping");
        }

        public override void timeUpdate(TimeFrame timeFrame)
        {
            //if (timeFrame != TimeFrame.m5)
            //    return;

            double last = getLast(timeFrame, PriceComponent.BidClose);
            //log.debug("timeUpdate() Last=" + last);

            // note m15 timeframe for EMA
            double ema = getEMA(TimeFrame.m15, PriceComponent.BidClose, emaPeriods);
            //log.debug("timeUpdate() EMA=" + ema);

            // note m15 timeframe for RSI
            double rsi = getRSI(TimeFrame.m15, PriceComponent.BidClose, rsiPeriods);
            //log.debug("timeUpdate() RSI=" + rsi);

            double[] stoch = getStoch(timeFrame, PriceComponent.BidClose, stochPeriods);
            double k = stoch[0];
            double d = stoch[1];
            //log.debug("timeUpdate() SSD, K=" + k + ", D=" + d);

            double diMinus = getMinusDI(timeFrame, adxPeriods);
            double diPlus = getPlusDI(timeFrame, adxPeriods);

            double adx = getADX(TimeFrame.m1, PriceComponent.BidHigh,
                PriceComponent.BidLow, PriceComponent.BidClose, adxPeriods);

            double lastBid = getLast(timeFrame, PriceComponent.BidClose);
            double lastAsk = getLast(timeFrame, PriceComponent.AskClose);

            if (!InPosition
                && ema > 0)
            {
                if (rsi != 0.0 && rsi > RSIThresholdOverBought // in m15 overbought context
                    && last > ema  // in m15 time
                    && adx != 0.0 && adx > ADXThreshold // trending at m1
                    && diMinus > diPlus // trending is a pullback at m1
                    && k > d    // reverse of pullback signal at m1
                    ) // in bull context, reverse ends in bounce
                {
                    string entry = "BUY";
                    Factory.Display.appendLine(String.Format("Open, {0}, {1}, price={2}", entry, Pair, lastAsk));
                    enterPosition(entry, amount, lastAsk, stopPips, limitPips);

                }
                else if (rsi != 0.0 && rsi < RSIThresholdOverSold
                    && last < ema
                    && adx != 0.0 && adx > ADXThreshold
                    && diMinus < diPlus
                    && k < d    // reverse signal
                    ) // in bear context, reverse ends in bounce
                {
                    string entry = "SELL";
                    Factory.Display.appendLine(String.Format("Open, {0}, {1}, price={2}", entry, Pair, lastBid));
                    enterPosition(entry, amount, lastBid, stopPips, limitPips);
                }
            }

        }

        public override void priceUpdate(DateTime datetime, double bid, double ask)
        {
            //throw new NotImplementedException();
        }
    }
}
