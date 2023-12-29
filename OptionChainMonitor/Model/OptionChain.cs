using System;

namespace OptionChainMonitor.Model
{
    public class OptionChain
    {      
        public double StrikePrice { get; internal set; }
        public double CallOpenInterest { get; internal set; }
        public double CallTotalTradedVolume { get; internal set; }
        public double CallLastPrice { get; internal set; }
        public double CallBidprice { get; internal set; }
        public double CallAskPrice { get; internal set; }
        public double PutOpenInterest { get; internal set; }
        public double PutTotalTradedVolume { get; internal set; }
        public double PutLastPrice { get; internal set; }
        public double PutBidprice { get; internal set; }
        public double PutAskPrice { get; internal set; }
        public DateTime SnapshotTime { get; internal set; }
    }
}
