using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.fx
{
    public enum TimeFrame
    {
        m1 = 1, m5 = 5, m15 = 15,
        m30 = 30, H1 = 60, H4 = 240,
        D1 = 60 * 24
    }

    public enum PriceComponent
    {
        BidHigh = 1, BidLow = 2,
        BidOpen = 3, BidClose = 4,
        AskHigh = 11, AskLow = 12,
        AskOpen = 13, AskClose = 14
    }
}
