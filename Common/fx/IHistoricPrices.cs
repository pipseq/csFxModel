using System;
using System.Collections.Generic;

namespace Common.fx
{
    public interface IHistoricPrices
    {
        void getHistory(string pair, TimeFrame timeframe, int bars);
        Dictionary<DateTime, Dictionary<PriceComponent, object>> getMap();
    }
}