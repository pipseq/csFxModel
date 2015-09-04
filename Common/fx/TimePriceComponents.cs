using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.fx
{
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

        public static Dictionary<string, TimeFrame> TimeFrameInverseMap
        {
            get
            {
                return timeFrameInverseMap;
            }

            set
            {
                timeFrameInverseMap = value;
            }
        }

        private static Dictionary<string, TimeFrame> timeFrameInverseMap = new Dictionary<string, TimeFrame>();
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

            TimeFrameInverseMap.Add("m1", TimeFrame.m1);
            TimeFrameInverseMap.Add("m5", TimeFrame.m5);
            TimeFrameInverseMap.Add("m15", TimeFrame.m15);
            TimeFrameInverseMap.Add("m30", TimeFrame.m30);
            TimeFrameInverseMap.Add("H1", TimeFrame.H1);
            TimeFrameInverseMap.Add("H4", TimeFrame.H4);
            TimeFrameInverseMap.Add("D1", TimeFrame.D1);

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
}
