using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.fx
{
    public interface PriceListener
    {
        void update(string pair, DateTime datetime, double bid, double ask);
        void periodicUpdate(TimeFrame[] timeFrames, DateTime now);
    }

}
