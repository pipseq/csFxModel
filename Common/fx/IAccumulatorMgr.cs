using System;
using System.Collections.Generic;

namespace Common.fx
{
    public interface IAccumulatorMgr
    {
        bool Debug { get; set; }
        string Snapshot { get; set; }

        void accumulate(string pair, DateTime dt, double bid, double ask);
        void add(string pair, DateTime dt, double bid, double ask);
        void create(string pair, TimeFrame timeFrame, PriceComponent priceComponent);
        void create(List<string> pairs, TimeFrame timeFrame, PriceComponent priceComponent);
        IAccumulator getAccum(string pair, TimeFrame timeFrame, PriceComponent priceComponent);
        ICollection<string> getPairs();
        List<PriceListener> getSubscribers();
        ICollection<TimeFrame> getTypes(string pair);
        void read();
        void read(string file);
        void roll(DateTime now);
        void roll(string pair, DateTime now);
        void subscribePrice(PriceListener priceListener);
        void unsubscribePrice(PriceListener priceListener);
        void write();
        void write(string file);
        string writeString();
    }
}