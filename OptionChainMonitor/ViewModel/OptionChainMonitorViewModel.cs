using OptionChainMonitor.Model;
using OptionChainMonitor.Service;
using System;
using System.Collections.ObjectModel;

namespace OptionChainMonitor.ViewModel
{
    public class OptionChainMonitorViewModel
    {

        public ObservableCollection<OptionChain> OptionChainCollection { get; set; }
        public NSEOptionChainReader OptionChainReader { get; set; }

        public OptionChainMonitorViewModel()
        {
            OptionChainCollection = new ObservableCollection<OptionChain>();
            OptionChainReader = new NSEOptionChainReader();

            OptionChainCollection.Add(new OptionChain { PutAskPrice = 200, StrikePrice = 200, PutTotalTradedVolume = 10000, SnapshotTime=DateTime.Now });

            var options = OptionChainReader.ReadOptionChain();
        }
    }
}
