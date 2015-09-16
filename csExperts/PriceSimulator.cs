using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.fx;
using fxCoreLink;
using simulatedTrading;

namespace csExperts
{
    public class PriceSimulator : ExpertBase, IExpert
    {
        public PriceSimulator(string pair, TimeFrame timeframe) : base(pair, timeframe)
        {
        }

        public override void priceUpdate(DateTime datetime, double bid, double ask)
        {
            TransactionManager.getInstance().priceUpdate(Pair, datetime, bid, ask);
        }

    }
}
