using Common;
using Common.fx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TicTacTec.TA.Library;

namespace fxCoreLink
{


    public enum Indicator
    {
        SMA = 1, EMA = 2, NONE = 0
    }
    public enum TradeType
    {
        BUY = 10, SELL = 1, NONE = 0
    }
    public enum TimeGranularity
    {
        SECOND = 0, MINUTE = 1, HOUR = 2, DAY = 3
    }
    public class Accumulator : IAccumulator
    {
        private List<double> list = new List<double>();
        private static int cntr = 0;
        private int id = 0;

        public List<double> List
        {
            get
            {
                return list;
            }

            set
            {
                list = value;
            }
        }

        public Accumulator()
        {
            id = ++cntr;
        }
        public void add(List<double> list)
        {
            this.List = list;
        }
        public List<double> getList()
        {
            return List;
        }
        public List<double> getList(int periods)
        {
            return List;
        }
        public double getLast()
        {
            return List.Last();
        }
        public bool isEnoughData(int periods)
        {
            return List.Count > periods;
        }
        public void addLast(double value)
        {
            int n = List.Count;
            if (n>0)
                List.RemoveAt(0);   // FIFO
            List.Add(value);
            int n2 = List.Count;
        }

    }
    public class AccumulatorMgr : IAccumulatorMgr
    {
        Logger log = Logger.LogManager("AccumulatorMgr");
        Dictionary<string, Dictionary<TimeFrame, Dictionary<PriceComponent, Accumulator>>> pairTimePriceMap
            = new Dictionary<string, Dictionary<TimeFrame, Dictionary<PriceComponent, Accumulator>>>();
        Dictionary<string, Dictionary<string, List<double>>> dataMap = new Dictionary<string, Dictionary<string, List<double>>>();
        List<PriceListener> priceListeners = new List<PriceListener>();

        public AccumulatorMgr()
        {
            log.debug("created");
        }

        public List<PriceListener> getSubscribers()
        {
            return priceListeners;
        }
        public void subscribePrice(PriceListener priceListener)
        {
            if (!priceListeners.Contains(priceListener))
                priceListeners.Add(priceListener);
        }

        public void unsubscribePrice(PriceListener priceListener)
        {
            if (priceListeners.Contains(priceListener))
                priceListeners.Remove(priceListener);
        }

        public IAccumulator getAccum(string pair, TimeFrame timeFrame, PriceComponent priceComponent)
        {
            return pairTimePriceMap[pair][timeFrame][priceComponent];
        }

        public ICollection<string> getPairs()
        {
            return pairTimePriceMap.Keys;
        }

        public ICollection<TimeFrame> getTypes(string pair)
        {
            return pairTimePriceMap[pair].Keys;
        }

        public void create(List<string> pairs, TimeFrame timeFrame, PriceComponent priceComponent)
        {
            foreach (string pair in pairs)
            {
                create(pair, timeFrame, priceComponent);
            }
        }

        public void create(string pair, TimeFrame timeFrame, PriceComponent priceComponent)
        {
            if (!pairTimePriceMap.ContainsKey(pair))
            {
                Dictionary<TimeFrame, Dictionary<PriceComponent, Accumulator>> m
                    = new Dictionary<TimeFrame, Dictionary<PriceComponent, Accumulator>>();
                pairTimePriceMap.Add(pair, m);
            }
            if (!pairTimePriceMap[pair].ContainsKey(timeFrame))
            {
                pairTimePriceMap[pair].Add(timeFrame, new Dictionary<PriceComponent, Accumulator>());
            }
            Dictionary<PriceComponent, Accumulator> mp = pairTimePriceMap[pair][timeFrame];
            if (!mp.ContainsKey(priceComponent))
            {
                mp.Add(priceComponent, new Accumulator());
            }

        }

        private object lockObject = new System.Object();
        public void add(string pair, DateTime dt, double bid, double ask)
        {
            if (priceListeners.Count > 0)
            {
                // make copy to support list modification
                List<PriceListener> lpl = new List<PriceListener>(priceListeners);
                foreach (PriceListener pl in lpl)
                    pl.update(pair, dt, bid, ask);
            }

            accumulate(pair, dt, bid, ask);
        }

        public void accumulate(string pair, DateTime dt, double bid, double ask)
        {
            if (pairTimePriceMap.ContainsKey(pair))
            {
                lock (lockObject)
                {
                    if (!dataMap.ContainsKey(pair))
                    {
                        dataMap.Add(pair, new Dictionary<string, List<double>>());
                    }
                    if (!dataMap[pair].ContainsKey("bid"))
                        dataMap[pair].Add("bid", new List<double>());
                    if (!dataMap[pair].ContainsKey("ask"))
                        dataMap[pair].Add("ask", new List<double>());
                    List<double> ldb = dataMap[pair]["bid"];
                    List<double> lda = dataMap[pair]["ask"];
                    ldb.Add(bid);
                    lda.Add(ask);
                }
            }
        }

        private bool debug = false;

        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        public string Snapshot
        {
            get
            {
                return snapshot;
            }

            set
            {
                snapshot = value;
                snapshot += ".json";

            }
        }

        public void roll(DateTime now)
        {
            foreach (string pair in pairTimePriceMap.Keys)
            {
                roll(pair, now);
            }
        }

        public void roll(string pair, DateTime now)
        {
            double lastBid = Double.NaN;
            double highBid = Double.NaN;
            double lowBid = Double.NaN;
            double openBid = Double.NaN;
            double lastAsk = Double.NaN;
            double highAsk = Double.NaN;
            double lowAsk = Double.NaN;
            double openAsk = Double.NaN;
            lock (lockObject)
            {
                if (dataMap.ContainsKey(pair)
                    && debug)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Double d in dataMap[pair]["bid"])
                    {
                        sb.Append("" + d + ",");
                    }
                    log.debug("bid dataMap(" + dataMap[pair].Count + ")=" + pair + ":" + sb.ToString());
                    foreach (Double d in dataMap[pair]["ask"])
                    {
                        sb.Append("" + d + ",");
                    }
                    log.debug("ask dataMap(" + dataMap[pair].Count + ")=" + pair + ":" + sb.ToString());
                }
                if (dataMap.ContainsKey(pair)
                    && dataMap[pair]["bid"].Count > 0)
                {
                    lastBid = dataMap[pair]["bid"].Last();
                    highBid = dataMap[pair]["bid"].Max();
                    lowBid = dataMap[pair]["bid"].Min();
                    openBid = dataMap[pair]["bid"].First();

                    lastAsk = dataMap[pair]["ask"].Last();
                    highAsk = dataMap[pair]["ask"].Max();
                    lowAsk = dataMap[pair]["ask"].Min();
                    openAsk = dataMap[pair]["ask"].First();

                    dataMap[pair]["bid"] = new List<double>();
                    dataMap[pair]["ask"] = new List<double>();
                }
            }
            foreach (TimeFrame timeFrame in ExpertFactory.timeFrameMap.Keys)
                if (PriceProcessor.isEvenIncrement(timeFrame, now))
                {
                    IAccumulator accumBidLa = getAccum(pair, timeFrame, PriceComponent.BidClose);
                    IAccumulator accumBidHi = getAccum(pair, timeFrame, PriceComponent.BidHigh);
                    IAccumulator accumBidLo = getAccum(pair, timeFrame, PriceComponent.BidLow);
                    IAccumulator accumBidOp = getAccum(pair, timeFrame, PriceComponent.BidOpen);
                    IAccumulator accumAskLa = getAccum(pair, timeFrame, PriceComponent.AskClose);
                    IAccumulator accumAskHi = getAccum(pair, timeFrame, PriceComponent.AskHigh);
                    IAccumulator accumAskLo = getAccum(pair, timeFrame, PriceComponent.AskLow);
                    IAccumulator accumAskOp = getAccum(pair, timeFrame, PriceComponent.AskOpen);

                    if (!Double.IsNaN(lastBid))
                        accumBidLa.addLast(lastBid);
                    if (!Double.IsNaN(openBid))
                        accumBidOp.addLast(openBid); // TODO - fix
                    if (!Double.IsNaN(lastAsk))
                        accumAskLa.addLast(lastAsk);
                    if (!Double.IsNaN(openAsk))
                        accumAskOp.addLast(openAsk); // TODO - fix

                    if (timeFrame == TimeFrame.m1)
                    {
                        if (!Double.IsNaN(highBid))
                            accumBidHi.addLast(highBid);
                        if (!Double.IsNaN(lowBid))
                            accumBidLo.addLast(lowBid);
                        if (!Double.IsNaN(highAsk))
                            accumAskHi.addLast(highAsk);
                        if (!Double.IsNaN(lowAsk))
                            accumAskLo.addLast(lowAsk);
                    }
                    else if (timeFrame == TimeFrame.m5)
                    {
                        int offset = 5;
                        rollDetail(pair, offset, TimeFrame.m1, accumBidHi, accumBidLo, PriceComponent.BidHigh, PriceComponent.BidLow);
                        rollDetail(pair, offset, TimeFrame.m1, accumAskHi, accumAskLo, PriceComponent.AskHigh, PriceComponent.AskLow);
                    }
                    else if (timeFrame == TimeFrame.m15)
                    {
                        int offset = 15;
                        rollDetail(pair, offset, TimeFrame.m1, accumBidHi, accumBidLo, PriceComponent.BidHigh, PriceComponent.BidLow);
                        rollDetail(pair, offset, TimeFrame.m1, accumAskHi, accumAskLo, PriceComponent.AskHigh, PriceComponent.AskLow);
                    }
                    else if (timeFrame == TimeFrame.m30)
                    {
                        int offset = 30;
                        rollDetail(pair, offset, TimeFrame.m1, accumBidHi, accumBidLo, PriceComponent.BidHigh, PriceComponent.BidLow);
                        rollDetail(pair, offset, TimeFrame.m1, accumAskHi, accumAskLo, PriceComponent.AskHigh, PriceComponent.AskLow);
                    }
                    else if (timeFrame == TimeFrame.H1)
                    {
                        int offset = 12;
                        rollDetail(pair, offset, TimeFrame.m5, accumBidHi, accumBidLo, PriceComponent.BidHigh, PriceComponent.BidLow);
                        rollDetail(pair, offset, TimeFrame.m5, accumAskHi, accumAskLo, PriceComponent.AskHigh, PriceComponent.AskLow);
                    }
                    else if (timeFrame == TimeFrame.H4)
                    {
                        int offset = 8;
                        rollDetail(pair, offset, TimeFrame.m30, accumBidHi, accumBidLo, PriceComponent.BidHigh, PriceComponent.BidLow);
                        rollDetail(pair, offset, TimeFrame.m30, accumAskHi, accumAskLo, PriceComponent.AskHigh, PriceComponent.AskLow);
                    }
                    else if (timeFrame == TimeFrame.D1)
                    {
                        int offset = 24;
                        rollDetail(pair, offset, TimeFrame.H1, accumBidHi, accumBidLo, PriceComponent.BidHigh, PriceComponent.BidLow);
                        rollDetail(pair, offset, TimeFrame.H1, accumAskHi, accumAskLo, PriceComponent.AskHigh, PriceComponent.AskLow);
                    }
                    debugList("bid close", pair, timeFrame, accumBidLa);
                    debugList("bid high", pair, timeFrame, accumBidHi);
                    debugList("bid low", pair, timeFrame, accumBidLo);
                    debugList("bid open", pair, timeFrame, accumBidOp);
                    debugList("ask close", pair, timeFrame, accumAskLa);
                    debugList("ask high", pair, timeFrame, accumAskHi);
                    debugList("ask low", pair, timeFrame, accumAskLo);
                    debugList("ask open", pair, timeFrame, accumAskOp);

            }
        }

        private void rollDetail(string pair, int offset, TimeFrame baseTimeFrame, 
            IAccumulator accumHi, IAccumulator accumLo, PriceComponent highComp, PriceComponent lowComp)
        {
            double high = Double.NaN;
            double low = Double.NaN;

            IAccumulator accumHi2 = getAccum(pair, baseTimeFrame, highComp);
            IAccumulator accumLo2 = getAccum(pair, baseTimeFrame, lowComp);

            List<double> ldh = accumHi2.getList();
            if (ldh.Count >= offset)
            {
                List<double> ldh2 = ldh.GetRange(ldh.Count - offset, offset);
                high = ldh2.Max();
            }
            List<double> ldl = accumLo2.getList();
            if (ldl.Count >= offset)
            {
                List<double> ldl2 = ldh.GetRange(ldh.Count - offset, offset);
                low = ldl2.Min();
            }
            if (!Double.IsNaN(high))
                accumHi.addLast(high);
            if (!Double.IsNaN(low))
                accumLo.addLast(low);
        }

        private void debugList(string title, string pair, TimeFrame timeFrame, IAccumulator accum)
        {
            if (!debug) return;
            log.debug("roll "+title+":" + pair + ":" + ExpertFactory.timeFrameMap[timeFrame]);
            StringBuilder sb = new StringBuilder();
            writeShortenedList(accum.getList(), sb);
            log.debug(title + "(" + accum.getList().Count + ")=" + sb.ToString());
        }

        private void writeShortenedList(List<double> list, StringBuilder sb)
        {
            if (
            // write the whole list by uncommenting the next line
            // true ||
            list.Count <= 7)
                foreach (Double d in list)
                {
                    sb.Append("" + d + ",");
                }
            else
            {
                int sz = list.Count;
                for (int i = 0; i < 3; i++)
                {
                    sb.Append("" + list[i] + ",");
                }
                sb.Append("... ");
                for (int i = sz-3; i < sz; i++)
                {
                    sb.Append("" + list[i] + ",");
                }

            }
        }

        private string snapshot=null;

        public void write()
        {
            if (snapshot != null)
                write(snapshot);
        }

        public void write(string file)
        {
            string json = writeString();
            File.WriteAllText(file, json);
        }

        public string writeString()
        {
            string json = JsonConvert.SerializeObject(pairTimePriceMap, Formatting.Indented);
            return json;
        }

        public void read()
        {
            if (snapshot != null)
                read(snapshot);
        }

        public void read(string file)
        {
            pairTimePriceMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<TimeFrame, Dictionary<PriceComponent, Accumulator>>>>(File.ReadAllText(file));

        }
    }

    public class EmaCummDataList : DataList
    {
        private DataList listdata;
        public EmaCummDataList(int size)
            : base(size)
        {
            listdata = new DataList(size);
        }
        public override void add(double data)
        {
            listdata.add(data);
            checksize();

            // compute stat
            // add result
            double ema = getEma(size);
            if (ema != 0.0)
                base.add(ema);
        }
        private double getEma(int periods)
        {
            List<double> ld = listdata.get();
            int begin;
            int length;
            double[] output = new double[ld.Count * 2];
            TicTacTec.TA.Library.Core.RetCode retCode
                = Core.Ema(ld.Count - periods, ld.Count - 1, ld.ToArray(), periods, out begin, out length, output);

            if (retCode == TicTacTec.TA.Library.Core.RetCode.Success)
                return output[length - 1];
            else return 0.0;
        }
    }

    public class DataList
    {
        protected List<double> list = new List<double>();
        protected int size;

        public DataList(int size)
        {
            this.size = size;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (double data in list)
            {
                sb.AppendFormat("{0},", data);
            }
            sb.Append("]");
            return sb.ToString();
        }

        public virtual void add(double data)
        {
            list.Add(data);
            checksize();
        }

        public bool isFull()
        {
            return list.Count == size;
        }

        public void checksize()
        {
            if (list.Count > size)
            {
                list.RemoveAt(0); // remove earliest
            }
        }

        public List<double> get()
        {
            return list;
        }

        public double last()
        {
            return list.Last();
        }

        public bool isMonotonicUp()
        {
            double data = list[0];
            if (list.Count == 1)
                return false;
            for (int i = 1; i < list.Count; i++)
            {
                if (data >= list[i])
                    return false;
                data = list[i];
            }
            return true;
        }
        public bool isMonotonicDown()
        {
            double data = list[0];
            if (list.Count == 1)
                return false;
            for (int i = 1; i < list.Count; i++)
            {
                if (data <= list[i])
                    return false;
                data = list[i];
            }
            return true;
        }
        public bool isHigherThan(DataList list2)
        {
            List<double> l2 = list2.get();
            if (list.Count != l2.Count)
                return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] <= l2[i])
                    return false;
            }
            return true;
        }
        public bool isLowerThan(DataList list2)
        {
            List<double> l2 = list2.get();
            if (list.Count != l2.Count)
                return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] >= l2[i])
                    return false;
            }
            return true;
        }
        // if the end of list1 is below list2
        // superset of isLowerThan()==true
        public bool isCrossBelow(DataList list2)
        {
            List<double> l2 = list2.get();
            if (list.Count != l2.Count)
                return false;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                // only care about last value above or below?! 
                if (list[i] < l2[i])
                    return true;
                else
                    return false;
            }
            return false;
        }
        // if the end of list1 is above list2
        // superset of isHigherThan()==true
        public bool isCrossAbove(DataList list2)
        {
            List<double> l2 = list2.get();
            if (list.Count != l2.Count)
                return false;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                // only care about last value above or below?! 
                if (list[i] > l2[i])
                    return true;
                else
                    return false;
            }
            return false;
        }
    }

}
