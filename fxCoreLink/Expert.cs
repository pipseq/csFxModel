using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TicTacTec.TA.Library;
using Timing = System.Timers.Timer;

namespace fxCoreLink
{
    // callback interface
    public interface IExpert
    {
        void start();   // called at startup for initialization
        void stop();    // called at shutdown
        void timeUpdate(TimeFrame timeFrame);  // called once every subscribed period
        void priceUpdate(DateTime datetime, double bid, double ask); // called on price update event
    }

    // Experts extend class for convenience methods
    public class ExpertBase : IExpert
    {

        private bool debug = false;
        public bool Debug
        {
            get { return debug; }
            set
            {
                debug = value;
            }
        }

        public bool InPosition
        {
            get
            {
                return Factory.FxUpdates.hasPositionInPair(pair);
            }
        }

        private ExpertFactory factory;

        public ExpertFactory Factory
        {
            get { return factory; }
            set { factory = value; }
        }
        private string pair;
        public string Pair
        {
            get { return pair; }
            set { pair = value; }
        }
        private TimeFrame timeframe;
        public TimeFrame Timeframe
        {
            get { return timeframe; }
            set { timeframe = value; }
        }
        private double pointsize = 0.0001;

        public double PointSize
        {
            get { return pointsize; }
            set { pointsize = value; }
        }

        private int limitPips = 0;

        public int LimitPips
        {
            get { return limitPips; }
            set { limitPips = value; }
        }
        private int stopPips = 0;

        public int StopPips
        {
            get { return stopPips; }
            set { stopPips = value; }
        }
        private int amount = 0;

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public ExpertBase(string pair, TimeFrame timeframe)
        {
            this.pair = pair;
            this.timeframe = timeframe;
            if (pair.EndsWith("JPY"))
                pointsize = 0.01;
        }

        public bool hasSufficientFunds()
        {
            return Factory.FxManager.hasMargin();
        }

        public bool hasSufficientFunds(double percentThreshold)
        {
            return Factory.FxManager.hasMargin(percentThreshold);
        }

        public string getPosition()
        {
            string posn = "";
            Dictionary<string, string> map = Factory.FxUpdates.getTrade(pair);

            if (map.Count > 0)
            {
                posn = map["BuySell"];
            }
            return posn;
        }

        public Dictionary<string, string> getPositionMap()
        {
            return Factory.FxUpdates.getTrade(pair);
        }

        public void enterPosition(string buySell, double last, string customId)
        {
            Factory.FxUpdates.enterPosition(Pair, buySell, last, StopPips, Amount, customId);

        }

        public void enterPosition(string buySell, double last, int stopPips, string customId)
        {
            Factory.FxUpdates.enterPosition(Pair, buySell, last, stopPips, Amount, customId);
        }

        public void closePosition()
        {
            Factory.FxUpdates.closePosition(pair);
        }

        public bool isEnoughData(TimeFrame timeFrame, PriceComponent priceComponent, int periods)
        {
            return Factory.FxUpdates.AccumMgr.getAccum(pair, timeFrame, priceComponent).isEnoughData(periods);
        }

        public double getLast(TimeFrame timeFrame, PriceComponent priceComponent)
        {
            return Factory.FxUpdates.AccumMgr.getAccum(pair, timeFrame, priceComponent).getLast();
        }

        public double getLast(TimeFrame timeFrame, PriceComponent priceComponent, int offset)
        {
            List<double> ld = getList(timeFrame, priceComponent);
            if (offset > 0)
            {
                ld = getCopy(ld);
                ld.RemoveRange(ld.Count - offset, offset);
            }
            return ld.Last();
        }

        private List<double> getCopy(List<double> ld)
        {
            double[] da = new double[ld.Count];
            ld.CopyTo(da);
            return da.ToList<double>();
        }

        public List<double> getList(TimeFrame timeFrame, PriceComponent priceComponent)
        {
            return Factory.FxUpdates.AccumMgr.getAccum(pair, timeFrame, priceComponent).getList();
        }

        public List<double> getAdjustedList(TimeFrame timeFrame, PriceComponent priceComponent, int offset)
        {
            List<double> ld = getList(timeFrame, priceComponent);
            if (offset > 0)
            {
                ld = getCopy(ld);
                ld.RemoveRange(ld.Count - offset, offset);
            }
            return ld;
        }

        public double getEMA(TimeFrame timeFrame, PriceComponent priceComponent, int periods)
        {
            return getEMA(timeFrame, priceComponent, periods, 0);
        }
        public double getEMA(TimeFrame timeFrame, PriceComponent priceComponent, int periods, int offset)
        {
            if (!isEnoughData(timeFrame, priceComponent, periods + offset))
                return 0.0;
            List<double> ld = getAdjustedList(timeFrame, priceComponent, offset);
            return getEMA(periods, ld);
        }

        public double getEMA(int periods, List<double> ld)
        {
            int begin;
            int length;
            double[] output = new double[ld.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.Ema(ld.Count - periods, ld.Count - 1, ld.ToArray(), periods, out begin, out length, output);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return output[length - 1];
            else return 0.0;
        }

        public double getSMA(TimeFrame timeFrame, PriceComponent priceComponent, int periods)
        {
            return getSMA(timeFrame, priceComponent, periods, 0);
        }
        public double getSMA(TimeFrame timeFrame, PriceComponent priceComponent, int periods, int offset)
        {
            if (!isEnoughData(timeFrame, priceComponent, periods + offset))
                return 0.0;
            List<double> ld = getAdjustedList(timeFrame, priceComponent, offset);
            return getSMA(periods, ld);
        }

        public double getSMA(int periods, List<double> ld)
        {
            int begin;
            int length;
            double[] output = new double[ld.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.Sma(ld.Count - periods, ld.Count - 1, ld.ToArray(), periods, out begin, out length, output);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return output[length - 1];
            else return 0.0;
        }

        public double getATR(TimeFrame timeFrame, int periods)
        {
            return getATR(timeFrame, periods, 0);
        }
        public double getATR(TimeFrame timeFrame, int periods, int offset)
        {
            if (!isEnoughData(timeFrame, PriceComponent.BidClose, periods + offset))
                return 0.0;
            List<double> ldh = getAdjustedList(timeFrame, PriceComponent.BidHigh, offset);
            List<double> ldl = getAdjustedList(timeFrame, PriceComponent.BidLow, offset);
            List<double> ldc = getAdjustedList(timeFrame, PriceComponent.BidClose, offset);
            return getATR(periods, ldh, ldl, ldc);
        }

        public double getATR(int periods, List<double> ldh, List<double> ldl, List<double> ldc)
        {
            int begin;
            int length;
            double[] output = new double[ldh.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.Atr(ldh.Count - periods, ldh.Count - 1,
                ldh.ToArray(),
                ldl.ToArray(),
                ldc.ToArray(),
                periods, out begin, out length, output);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return output[length - 1];
            else return 0.0;
        }

        public double getMinusDI(TimeFrame timeFrame, int periods)
        {
            return getMinusDI(timeFrame, periods, 0);
        }
        public double getMinusDI(TimeFrame timeFrame, int periods, int offset)
        {
            if (!isEnoughData(timeFrame, PriceComponent.BidClose, periods + offset))
                return 0.0;
            List<double> ldh = getAdjustedList(timeFrame, PriceComponent.BidHigh, offset);
            List<double> ldl = getAdjustedList(timeFrame, PriceComponent.BidLow, offset);
            List<double> ldc = getAdjustedList(timeFrame, PriceComponent.BidClose, offset);
            return getMinusDI(periods, ldh, ldl, ldc);
        }

        public double getMinusDI(int periods, List<double> ldh, List<double> ldl, List<double> ldc)
        {
            int begin;
            int length;
            double[] output = new double[ldh.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.MinusDI(ldh.Count - periods, ldh.Count - 1,
                ldh.ToArray(),
                ldl.ToArray(),
                ldc.ToArray(),
                periods, out begin, out length, output);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return output[length - 1];
            else return 0.0;
        }

        public double getPlusDI(TimeFrame timeFrame, int periods)
        {
            return getPlusDI(timeFrame, periods, 0);
        }
        public double getPlusDI(TimeFrame timeFrame, int periods, int offset)
        {
            if (!isEnoughData(timeFrame, PriceComponent.BidClose, periods + offset))
                return 0.0;
            List<double> ldh = getAdjustedList(timeFrame, PriceComponent.BidHigh, offset);
            List<double> ldl = getAdjustedList(timeFrame, PriceComponent.BidLow, offset);
            List<double> ldc = getAdjustedList(timeFrame, PriceComponent.BidClose, offset);
            return getPlusDI(periods, ldh, ldl, ldc);
        }

        public double getPlusDI(int periods, List<double> ldh, List<double> ldl, List<double> ldc)
        {
            int begin;
            int length;
            double[] output = new double[ldh.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.PlusDI(ldh.Count - periods, ldh.Count - 1,
                ldh.ToArray(),
                ldl.ToArray(),
                ldc.ToArray(),
                periods, out begin, out length, output);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return output[length - 1];
            else return 0.0;
        }

        public double getRSI(TimeFrame timeFrame, PriceComponent priceComponent, int periods)
        {
            return getRSI(timeFrame, priceComponent, periods, 0);
        }
        public double getRSI(TimeFrame timeFrame, PriceComponent priceComponent, int periods, int offset)
        {
            if (!isEnoughData(timeFrame, priceComponent, periods + offset))
                return 0.0;
            List<double> ld = getAdjustedList(timeFrame, priceComponent, offset);
            return getRsi(periods, ld);
        }

        public double getRsi(int periods, List<double> ld)
        {
            int begin;
            int length;
            double[] output = new double[ld.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.Rsi(ld.Count - periods, ld.Count - 1, ld.ToArray(), periods, out begin, out length, output);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return output[length - 1];
            else return 0.0;
        }

        public List<double> getMACDasList(TimeFrame timeFrame, PriceComponent priceComponent, int fastPeriod, int slowPeriod, int signalPeriod)
        {
            double[] ad = getMACD(timeFrame, priceComponent, fastPeriod, slowPeriod, signalPeriod, 0);
            List<double> ld = new List<double>(ad);
            return ld;
        }
        public double[] getMACD(TimeFrame timeFrame, PriceComponent priceComponent, int fastPeriod, int slowPeriod, int signalPeriod)
        {
            return getMACD(timeFrame, priceComponent, fastPeriod, slowPeriod, signalPeriod, 0);
        }
        public double[] getMACD(TimeFrame timeFrame, PriceComponent priceComponent, int fastPeriod, int slowPeriod, int signalPeriod, int offset)
        {
            if (!isEnoughData(timeFrame, priceComponent, fastPeriod + offset))
                return new double[] { 0.0, 0.0, 0.0 };
            List<double> ld = getAdjustedList(timeFrame, priceComponent, offset);
            return getMACD(fastPeriod, slowPeriod, signalPeriod, ld);
        }

        public double[] getMACD(int fastPeriod, int slowPeriod, int signalPeriod, List<double> ld)
        {
            int begin;
            int length;
            double[] outMACD = new double[ld.Count * 2];
            double[] outSignal = new double[ld.Count * 2];
            double[] outHist = new double[ld.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.Macd(
                ld.Count - slowPeriod, 
                ld.Count - 1, 
                ld.ToArray(),
                fastPeriod, 
                slowPeriod,
                signalPeriod,
                out begin, 
                out length,
                outMACD,
                outSignal,
                outHist
                );

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return new double[] { outMACD[length - 1], outSignal[length - 1], outHist[length - 1] };
            else return new double[] {0.0, 0.0, 0.0};
        }

        public double getADX(TimeFrame timeFrame, PriceComponent priceComponentClose, int periods)
        {
            return getADX(timeFrame, priceComponentClose, periods, 0);
        }
        public double getADX(TimeFrame timeFrame, PriceComponent priceComponentClose, int periods, int offset)
        {
            if (priceComponentClose == PriceComponent.BidClose)
                return getADX(timeFrame, PriceComponent.BidHigh, PriceComponent.BidLow, priceComponentClose, periods, offset);
            else if (priceComponentClose == PriceComponent.AskClose)
                return getADX(timeFrame, PriceComponent.AskHigh, PriceComponent.AskLow, priceComponentClose, periods, offset);
            else throw new Exception("Bad params in getADX");
        }
        public double getADX(TimeFrame timeFrame, PriceComponent priceComponentHi, PriceComponent priceComponentLow, PriceComponent priceComponentClose, int periods)
        {
            return getADX(timeFrame, priceComponentHi, priceComponentLow, priceComponentClose, periods, 0);
        }
        public double getADX(TimeFrame timeFrame, PriceComponent priceComponentHi, PriceComponent priceComponentLow, PriceComponent priceComponentClose, int periods, int offset)
        {
            if (!isEnoughData(timeFrame, priceComponentClose, periods + offset))
                return 0.0;
            List<double> ldh = getAdjustedList(timeFrame, PriceComponent.BidHigh, offset);
            List<double> ldl = getAdjustedList(timeFrame, PriceComponent.BidLow, offset);
            List<double> ldc = getAdjustedList(timeFrame, PriceComponent.BidClose, offset);
            return getADX(periods, ldh, ldl, ldc);
        }

        public double getADX(int periods, List<double> ldh, List<double> ldl, List<double> ldc)
        {
            int begin;
            int length;
            // check
            if (true)
            {
                if (periods < 0 || periods > 100)
                {
                    throw new Exception("getADX():periods = " + periods + " out of range");
                }
                if (ldh.Count <= 0 || ldh.Count > 250)
                {
                    throw new Exception("getADX():ldh.Count = " + ldh.Count + " out of range");
                }
                if (ldl.Count <= 0 || ldl.Count > 250)
                {
                    throw new Exception("getADX():ldl.Count = " + ldh.Count + " out of range");
                }
                if (ldc.Count <= 0 || ldc.Count > 250)
                {
                    throw new Exception("getADX():ldc.Count = " + ldh.Count + " out of range");
                }
            }
            double[] output = new double[ldc.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.Adx(
                0, ldc.Count - 1,
                //ldc.Count - periods, ldc.Count - 1,
                ldh.ToArray(), ldl.ToArray(), ldc.ToArray(),
                periods, out begin, out length, output);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return output[length - 1];
            else
                return 0.0;
        }

        public double[] getStoch(TimeFrame timeFrame, PriceComponent priceComponentClose, int periods)
        {
            return getStoch(timeFrame, priceComponentClose, periods, 0);
        }
        public double[] getStoch(TimeFrame timeFrame, PriceComponent priceComponentClose, int periods, int offset)
        {
            if (priceComponentClose == PriceComponent.BidClose)
                return getStoch(timeFrame, PriceComponent.BidHigh, PriceComponent.BidLow, priceComponentClose, periods, offset);
            else if (priceComponentClose == PriceComponent.AskClose)
                return getStoch(timeFrame, PriceComponent.AskHigh, PriceComponent.AskLow, priceComponentClose, periods, offset);
            else throw new Exception("Bad params in getADX");
        }
        public double[] getStoch(TimeFrame timeFrame, PriceComponent priceComponentHi, PriceComponent priceComponentLow, PriceComponent priceComponentClose, int periods)
        {
            return getStoch(timeFrame, priceComponentHi, priceComponentLow, priceComponentClose, periods, 0);
        }
        public double[] getStoch(TimeFrame timeFrame, PriceComponent priceComponentHi, PriceComponent priceComponentLow, PriceComponent priceComponentClose, int periods, int offset)
        {
            if (!isEnoughData(timeFrame, priceComponentClose, periods + offset))
                return new double[] { 0.0, 0.0 };
            List<double> ldh = getAdjustedList(timeFrame, PriceComponent.BidHigh, offset);
            List<double> ldl = getAdjustedList(timeFrame, PriceComponent.BidLow, offset);
            List<double> ldc = getAdjustedList(timeFrame, PriceComponent.BidClose, offset);
            return getStoch(periods, ldh, ldl, ldc);
        }

        public double[] getStoch(int periods, List<double> ldh, List<double> ldl, List<double> ldc)
        {
            int begin;
            int length;
            // check
            if (true)
            {
                if (periods < 0 || periods > 100)
                {
                    throw new Exception("getStoch():periods = " + periods + " out of range");
                }
                if (ldh.Count <= 0 || ldh.Count > 250)
                {
                    throw new Exception("getStoch():ldh.Count = " + ldh.Count + " out of range");
                }
                if (ldl.Count <= 0 || ldl.Count > 250)
                {
                    throw new Exception("getStoch():ldl.Count = " + ldh.Count + " out of range");
                }
                if (ldc.Count <= 0 || ldc.Count > 250)
                {
                    throw new Exception("getStoch():ldc.Count = " + ldh.Count + " out of range");
                }
            }
            double[] outSlowK = new double[ldc.Count * 2];
            double[] outSlowD = new double[ldc.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.Stoch(
                0, 
                ldc.Count - 1,
                //ldc.Count - periods, ldc.Count - 1,
                ldh.ToArray(), 
                ldl.ToArray(), 
                ldc.ToArray(),
                periods, // typically 5, 9 or 14
                3,
                Core.MAType.Sma,
                3,
                Core.MAType.Sma,
                out begin, 
                out length,
                outSlowK,
                outSlowD);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return new double[] { outSlowK[length - 1], outSlowD[length - 1] };
            else
                return new double[] { 0.0, 0.0 };
        }

        public double getSMATrend(TimeFrame timeFrame, PriceComponent priceComponent, int periods)
        {
            return 0.0;
        }

        public double getEMATrend(TimeFrame timeFrame, PriceComponent priceComponent, int periods)
        {
            return 0.0;
        }

        public double getRSITrend(TimeFrame timeFrame, PriceComponent priceComponent, int periods)
        {
            return 0.0;
        }

        public virtual void start()
        {
            throw new NotImplementedException();
        }

        public virtual void stop()
        {
            throw new NotImplementedException();
        }

        public virtual void timeUpdate(TimeFrame timeFrame)
        {
            throw new NotImplementedException();
        }

        public virtual void priceUpdate(DateTime datetime, double bid, double ask)
        {
            throw new NotImplementedException();
        }
    }

    public interface PriceListener
    {
        void update(string pair, DateTime datetime, double bid, double ask);
        void periodicUpdate(TimeFrame[] timeFrames, int counter);
    }

    public class TimePriceComponents
    {
        static public Dictionary<TimeFrame, string> timeFrameMap = new Dictionary<TimeFrame, string>();
        public static Dictionary<TimeFrame, string> TimeFrameMap
        {
            get
            {
                return timeFrameMap;
            }
        }

        static public Dictionary<string, TimeFrame> timeFrameInverseMap = new Dictionary<string,TimeFrame>();
        static public List<PriceComponent> priceComponentList = new List<PriceComponent>();
        //static private Dictionary<string, TimeFrame> timeStringMap = new Dictionary<string, TimeFrame>();
        static TimePriceComponents()
        {
            timeFrameMap.Add(TimeFrame.m1, "m1");
            timeFrameMap.Add(TimeFrame.m5, "m5");
            timeFrameMap.Add(TimeFrame.m15, "m15");
            timeFrameMap.Add(TimeFrame.m30, "m30");
            timeFrameMap.Add(TimeFrame.H1, "H1");
            timeFrameMap.Add(TimeFrame.H4, "H4");
            timeFrameMap.Add(TimeFrame.D1, "D1");

            timeFrameInverseMap.Add("m1",TimeFrame.m1);
            timeFrameInverseMap.Add("m5", TimeFrame.m5);
            timeFrameInverseMap.Add("m15", TimeFrame.m15);
            timeFrameInverseMap.Add("m30", TimeFrame.m30);
            timeFrameInverseMap.Add("H1", TimeFrame.H1);
            timeFrameInverseMap.Add("H4", TimeFrame.H4);
            timeFrameInverseMap.Add("D1", TimeFrame.D1);

            priceComponentList.Add(PriceComponent.AskClose);
            priceComponentList.Add(PriceComponent.AskHigh);
            priceComponentList.Add(PriceComponent.AskLow);
            priceComponentList.Add(PriceComponent.AskOpen);
            priceComponentList.Add(PriceComponent.BidClose);
            priceComponentList.Add(PriceComponent.BidHigh);
            priceComponentList.Add(PriceComponent.BidLow);
            priceComponentList.Add(PriceComponent.BidOpen);

        }

    }
    public class ExpertFactory : TimePriceComponents, PriceListener
    {
        Logger log = Logger.LogManager("ExpertFactory");

        private bool debug = false;
        public bool Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                if (fxUpdates != null)
                {
                    fxUpdates.Debug = value;
                }
                foreach (ExpertBase expert in map.Keys)
                {
                    expert.Debug = value;
                }
            }
        }

        private Display display;

        public Display Display
        {
            get { return display; }
            set { display = value; }
        }
        private Control control;

        public Control Control
        {
            get { return control; }
            set { control = value; }
        }
        private IFXManager fxManager;

        public IFXManager FxManager
        {
            get { return fxManager; }
            set { fxManager = value; }
        }
        private FxUpdates fxUpdates;
        public FxUpdates FxUpdates
        {
            get { return fxUpdates; }
            set { fxUpdates = value; }
        }
        public string UniqueId
        {
            get
            {
                return FxUpdates.UniqueId;
            }
        }

        public ExpertFactory(PriceProcessor priceProcessor)
        {
            this.display = priceProcessor.Display;
            this.control = priceProcessor.Control;
            this.fxManager = priceProcessor.FxManager;
            this.fxUpdates = priceProcessor.FxUpdates;
            fxUpdates.AccumMgr.subscribePrice(this);
        }

        public void shutdown()
        {
            fxUpdates.AccumMgr.unsubscribePrice(this);
        }

        #region subscription

        Dictionary<ExpertBase, Dictionary<string, object>> map = new Dictionary<ExpertBase, Dictionary<string, object>>();
        // subscribe for callbacks to an expert
        public void subscribe(ExpertBase expert)
        {
            if (!map.ContainsKey(expert))
            {
                map.Add(expert, new Dictionary<string, object>());
            }
            Dictionary<string, object> smap = map[expert];
            smap["timeframe"] = expert.Timeframe;
            smap["pair"] = expert.Pair;
            expert.Factory = this;
        }

        public List<ExpertBase> getSubscribers(string pair, TimeFrame timeFrame)
        {
            List<ExpertBase> list = new List<ExpertBase>();
            foreach (ExpertBase expert in map.Keys)
            {
                TimeFrame tf = (TimeFrame)map[expert]["timeframe"];
                string pr = (string)map[expert]["pair"];
                if (pair == pr && timeFrame <= tf)
                    list.Add(expert);
            }
            return list;
        }

        // called by start()
        public List<TimeFrame> getSubscribersTimeFrames()
        {
            List<TimeFrame> list = new List<TimeFrame>();
            foreach (ExpertBase expert in map.Keys)
            {
                TimeFrame tf = (TimeFrame)map[expert]["timeframe"];
                if (!list.Contains(tf))
                    list.Add(tf);
            }
            return list;
        }

        // called by start()
        public List<string> getSubscribersPairs()
        {
            List<string> list = new List<string>();
            foreach (ExpertBase expert in map.Keys)
            {
                string pair = (string)map[expert]["pair"];
                if (!list.Contains(pair))
                    list.Add(pair);
            }
            return list;
        }

        // called by start()
        public Dictionary<TimeFrame, List<string>> getSubscribersTimeFramesPairs()
        {
            Dictionary<TimeFrame, List<string>> tpMap = new Dictionary<TimeFrame, List<string>>();
            List<TimeFrame> list = getSubscribersTimeFrames();
            foreach (TimeFrame t in list)
            {
                tpMap.Add(t, new List<string>());
            }

            foreach (ExpertBase expert in map.Keys)
            {
                TimeFrame tf = (TimeFrame)map[expert]["timeframe"];
                List<string> lt = tpMap[tf];
                string pr = (string)map[expert]["pair"];
                lt.Add(pr);
            }
            return tpMap;
        }
        Dictionary<string, List<ExpertBase>> mapExperts = new Dictionary<string, List<ExpertBase>>();

        public void removeExpert(ExpertBase expert)
        {
            if (mapExperts.ContainsKey(expert.Pair))
            {
                List<ExpertBase> leb = mapExperts[expert.Pair];
                if (leb.Contains(expert))
                {
                    leb.Remove(expert);
                }
                // rather than remove, complicating copy before iteration
                // leave leb empty, doesn't hurt
                // shutdown based on map, anyway
                //if (leb.Count == 0)
                //{
                //    mapExperts.Remove(expert.Pair);
                //}
            }
            if (map.ContainsKey(expert))
            {
                map.Remove(expert);
            }
            if (map.Count == 0)
                shutdown(); // this factory // How do you get here?!
        }

        public void subscribePrice(ExpertBase expert)
        {
            string pair = expert.Pair;
            if (!mapExperts.ContainsKey(pair))
                mapExperts.Add(pair, new List<ExpertBase>());

            mapExperts[pair].Add(expert);  // replacement
            expert.Factory = this;
        }

        public void startExperts()
        {
            foreach (ExpertBase expert in map.Keys)
            {
                expert.Factory = this;
                expert.start();
            }
        }
        #endregion
        #region PriceListener implementation

        private object lockObject = new System.Object();
        public void periodicUpdate(TimeFrame[] timeFrames, int counter)
        {
            foreach (string pair in getSubscribersPairs())
            {
                foreach (TimeFrame timeFrame in timeFrames.Reverse())
                {
                    if (PriceProcessor.isEvenIncrement(timeFrame, counter))
                    {
                        bool found = periodicUpdate(pair, timeFrame);
                        if (found)
                            break;
                    }
                }
            }
        }

        public bool periodicUpdate(string pair, TimeFrame timeFrame)
        {
            List<ExpertBase> listExperts = getSubscribers(pair, timeFrame);
            // make copy to support list modification
            List<ExpertBase> le = new List<ExpertBase>(listExperts);
            foreach (ExpertBase expert in le)
            {
                lock (lockObject) {     // synch journal writing
                    expert.timeUpdate(timeFrame);
                    if (Control.isJournalWrite())
                    {
                        journal(pair, timeFrame);
                    }
                }
            }
            return le.Count > 0;
        }

        public void update(string pair, DateTime datetime, double bid, double ask)
        {
            if (mapExperts.ContainsKey(pair))
            {
                List<ExpertBase> list = mapExperts[pair];
                List<ExpertBase> leb = new List<ExpertBase>(list);
                foreach (ExpertBase expert in leb)
                {
                    lock (lockObject)     // synch journal writing
                    {     // synch journal writing
                        expert.priceUpdate(datetime, bid, ask);
                        if (Control.isJournalWrite())
                        {
                            journal(datetime, pair, bid, ask);
                        }
                    }
                }
            }
        }
        #endregion
        #region journaling
        HashSet<string> pairs;
        string file;
        bool append=false;
        public void setJournal(List<string> pairs, string file, bool append)
        {
            this.pairs = new HashSet<string>(pairs);
            this.file = file;
            this.file += ".tdv";
            this.append = append;
        }
        private void journal(DateTime datetime, string pair, double bid, double ask)
        {
            if (pairs.Contains(pair))
                using (StreamWriter sw = new StreamWriter(file, append))
                {
                    sw.WriteLine(string.Format(
                        "{0}\t{1}\t{2}\t{3}", datetime, pair, bid, ask));
                }
        }
        private void journal(string pair, TimeFrame timeframe)
        {
            if (pairs.Contains(pair))
                using (StreamWriter sw = new StreamWriter(file, append))
                {
                    sw.WriteLine(string.Format(
                        "{0}\t{1}\t{2}", DateTime.Now, pair, timeFrameMap[timeframe], "timeUpdate"));
                }
        }
        private void journal(string command)
        {
            using (StreamWriter sw = new StreamWriter(file, append))
            {
                sw.WriteLine(string.Format(
                    "{0}\t{1}", DateTime.Now, command));
            }
        }
        public void readJournal()
        {
            new Thread(readJournalDelegate).Start();
        }
        public void readJournalDelegate()
        {
            Display.appendLine("File {0} started", file);
            using (StreamReader sr = new StreamReader(file))
            {
                while (offersFileReader(sr)) ;
            }
            Control.journalReadDone();
            Display.appendLine("File {0} finished", file);
        }

        public bool offersFileReader(StreamReader sr)
        {
            string line = sr.ReadLine();
            if (line == null)
            {
                sr.Close();
                return false;
            }
            if (line.StartsWith("#"))
            {
                return true;
            }
            string[] fields = line.Split('\t');
            switch (fields.Count())
            {
                case 1: // command
                    if ("breakCmd" == fields[0])
                        breakCommand();
                    break;

                case 3:
                    DateTime dt = DateTime.Parse(fields[0]);
                    string pr = fields[1];
                    string tf = fields[2];
                    TimeFrame timeframe = timeFrameInverseMap[tf];
                    if (pairs.Contains(pr))
                    {
                        roll(pr);
                        periodicUpdate(pr, timeframe);
                    }
                    break;

                case 4:
                    dt = DateTime.Parse(fields[0]);
                    pr = fields[1];
                    double bid = double.Parse(fields[2]);
                    double ask = double.Parse(fields[3]);
                    if (pairs.Contains(pr))
                    {
                        update(pr, dt, bid, ask);
                        fxUpdates.AccumMgr.accumulate(pr, dt, bid, ask);
                    }
                    break;

                default:
                    break;
            }

            return true;
        }

        // convenience place to stop 
        // when replaying journal
        public void breakCommand()
        {

        }

        Dictionary<string, int> mapPairHistoryCounter = new Dictionary<string, int>();
        private void roll(string pair)
        {
            if (!mapPairHistoryCounter.ContainsKey(pair))
            {
                mapPairHistoryCounter.Add(pair, 0);
            }
            fxUpdates.AccumMgr.roll(pair, mapPairHistoryCounter[pair]++);
        }
        #endregion
    }

    public delegate void FileTimeEventHandler();
    public class PriceProcessor :TimePriceComponents
    {
        #region declarations

        Logger log = Logger.LogManager("PriceProcessor");
        private bool debug = false;

        public bool Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                if (fxUpdates != null)
                {
                    fxUpdates.Debug = value;
                }
            }
        }

        private bool canProcess = false;

        public bool CanProcess
        {
            get { return canProcess; }
            set { canProcess = value; }
        }

        private Display display;

        public Display Display
        {
            get { return display; }
            set { display = value; }
        }
        private Control control;

        public Control Control
        {
            get { return control; }
            set { control = value; }
        }
        private IFXManager fxManager;

        public IFXManager FxManager
        {
            get { return fxManager; }
            set { fxManager = value; }
        }
        private FxUpdates fxUpdates;
        public FxUpdates FxUpdates
        {
            get { return fxUpdates; }
            set { fxUpdates = value; }
        }
        private AccumulatorMgr accumMgr;

        public AccumulatorMgr AccumMgr
        {
            get { return accumMgr; }
            set { accumMgr = value; }
        }


         #endregion

        #region lifecycle

        public PriceProcessor(Display display, Control control, IFXManager fxManager, FxUpdates fxUpdates)
        {
            this.display = display;
            this.control = control;
            this.fxManager = fxManager;
            this.fxUpdates = fxUpdates;
            this.accumMgr = fxUpdates.AccumMgr;
        }

       #endregion

        #region expert processing

        private PriceUpdateProcess priceUpdateProcess;

        public void start()
        {
            start(new List<string>(accumMgr.getPairs()));
        }

        public void start(List<string> pairs)
        {
            canProcess = true;
            if (Control.isJournalRead())
                return;
            fxManager.getSession();

            foreach (string pair in pairs)
            {
                foreach (TimeFrame t in timeFrameMap.Keys)
                {
                    foreach (PriceComponent priceComponent in priceComponentList)
                    {
                        accumMgr.create(pair, t, priceComponent);    // preloading
                    }
                }
            }

            // set up threads for update
            timedDisplayEvent(60, "m1", ExpertFactory.timeFrameMap.Keys.ToArray());
            log.debug("start()");

        }
        public void stop()
        {
            canProcess = false;
            if (!Control.isJournalRead())
                fxManager.closeSession();
            log.debug("stop()");
        }

        #endregion


        #region expert updates
        private void timedDisplayEvent(int seconds, string type, TimeFrame[] timeFrames)
        {
            int interval = (int)seconds;
            Timing timer = TimerMgr.getInstance().create(type, interval);
            TimerMgr.getInstance().putTimerMap(timer, "types", timeFrames);

            timer.Elapsed += new System.Timers.ElapsedEventHandler(timedDisplayEventHandler);
            timer.Start();
            // kick off immediately
            priceUpdateProcess = new PriceUpdateProcess(this, timeFrames);
            priceUpdateProcess.process();

        }
        void timedDisplayEventHandler(object sender, EventArgs e)
        {
            Timing timer = (Timing)sender;
            if (!canProcess)
            {
                TimerMgr.getInstance().stop(timer);
                return;
            }
            TimeFrame[] timeFrames = (TimeFrame[])TimerMgr.getInstance().getTimerMap(timer, "types");
            priceUpdateProcess.process();
        }

        int historyCounter = 0;

        public bool isEvenIncrement(TimeFrame timeFrame)
        {
            return isEvenIncrement(timeFrame, historyCounter);
        }
        public static bool isEvenIncrement(TimeFrame timeFrame, int cnt)
        {
            switch (timeFrame)
            {
                case TimeFrame.m1: return 0 == 0;
                case TimeFrame.m5: return cnt % 5 == 0;
                case TimeFrame.m15: return cnt % 15 == 0;
                case TimeFrame.m30: return cnt % 30 == 0;
                case TimeFrame.H1: return cnt % 60 == 0;
                case TimeFrame.H4: return cnt % (4 * 60) == 0;
                case TimeFrame.D1: return cnt % (24 * 60) == 0;
            }
            return false;
        }

        public void roll()
        {
            AccumMgr.roll(historyCounter);
        }

        // this stays in factory
        public void updatePriceListeners(TimeFrame[] timeFrames)
        {
            List<PriceListener> listListeners = accumMgr.getSubscribers();
            foreach (PriceListener listener in listListeners)
            {
                listener.periodicUpdate(timeFrames,historyCounter);
            }
            historyCounter++;
        }

        // TODO moves to OCOForm
        // pairs from selectable pairs
        // need timeframe and pricecomponent in OCOForm?  Or central place for initialization
        public void updatePriceHistory(TimeFrame[] timeFrames)
        {
            log.debug("updatePriceHistory() start");
            int bars = Control.getIntProperty("priceHistory.bars", 40);

            foreach (TimeFrame timeFrame in timeFrames)
            {
                if (!isEvenIncrement(timeFrame, historyCounter))
                    continue;

                HistoricPrices prices = fxManager.getHistoricPrices();
                foreach (string pair in accumMgr.getPairs())
                {
                    prices.getHistory(pair, timeFrame, bars);
                    Dictionary<DateTime, Dictionary<PriceComponent, object>> map = prices.getMap();

                    foreach (PriceComponent priceComponent in priceComponentList)
                    {
                        Accumulator accum = accumMgr.getAccum(pair, timeFrame, priceComponent);
                        List<double> list = new List<double>();
                        foreach (DateTime dt in map.Keys)
                        {
                            list.Add((double)map[dt][priceComponent]);
                        }
                        accum.add(list);
                    }

                }
                fxManager.closeHistoricPrices();
            }
            if (Control.isJournalWrite())
            {
                AccumMgr.write();
            }
            log.debug("PriceHistory done");
        }

        #endregion
    }
    public class PriceUpdateProcess
    {
        Logger log = Logger.LogManager("PriceUpdateProcess");
        private PriceProcessor priceProcessor;
        private TimeFrame[] types;
        private bool initialized = false;
        private int exceptionCount = 0;
        public PriceUpdateProcess(PriceProcessor priceProcessor, TimeFrame[] timeFrames)
        {
            this.priceProcessor = priceProcessor;
            this.types = timeFrames;
        }

        public void process()
        {
            Thread workerThread = new Thread(priceDecisionProcess);
            workerThread.Start();
        }

        public void priceDecisionProcess()
        {
            try
            {
                if (!initialized)
                {
                    priceProcessor.updatePriceHistory(types);
                }
                priceProcessor.roll();
                priceProcessor.updatePriceListeners(types);
                if (!initialized)
                {
                    priceProcessor.FxUpdates.readRows();
                    initialized = true;
                }
            }
            catch (Exception ex)
            {
                log.debug("Exception " + ex.Message);
                if (exceptionCount++ > 100)
                {
                    log.debug("Exceeded exception count, stopping process");
                    priceProcessor.stop();
                }
            }
        }

    }


}
