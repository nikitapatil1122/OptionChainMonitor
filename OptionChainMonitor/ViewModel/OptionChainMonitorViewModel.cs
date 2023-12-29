using OptionChainMonitor.Model;
using OptionChainMonitor.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace OptionChainMonitor.ViewModel
{
    public class OptionChainMonitorViewModel : INotifyPropertyChanged
    {        
        private DateTime _selectedTimeStamp;
        private Timer snapshotTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public NSEOptionChainReader OptionChainReader { get; set; }

        public ObservableCollection<OptionChain> OptionChainCollection { get; set; }
        public ObservableCollection<OptionChain> SnapshotOptionsCollection { get; set; }
        public ObservableCollection<DateTime> SnapshotCollection { get; set; }

        public DateTime SelectedTimeStamp
        {
            get
            {
                return _selectedTimeStamp;
            }
            set
            {
                _selectedTimeStamp = value;
                OnPropertyChanged();

                SnapshotOptionsCollection.Clear();

                OptionChainCollection.Where(o => o.SnapshotTime == _selectedTimeStamp).ToList().ForEach(i => SnapshotOptionsCollection.Add(i));
            }
        }



        public OptionChainMonitorViewModel()
        {
            OptionChainCollection = new ObservableCollection<OptionChain>();
            SnapshotOptionsCollection = new ObservableCollection<OptionChain>();
            SnapshotCollection = new ObservableCollection<DateTime>();
            OptionChainReader = new NSEOptionChainReader();
            SetTimer();
        }

        private void SetTimer()
        {
            snapshotTimer = new Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);
            snapshotTimer.Elapsed += OnTimedEvent;
            snapshotTimer.AutoReset = true;
            snapshotTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _ = LoadOptionChainDataAsync();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async Task LoadOptionChainDataAsync()
        {
            DateTime snapshotTime = DateTime.Now;

            try
            {
                var options = await OptionChainReader.GetOptionChainAsync(snapshotTime);
                if (!options.Any()) return;

                options.ForEach(o => OptionChainCollection.Add(o));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Application.Current.Dispatcher.BeginInvoke(new Action(() => { SnapshotCollection.Add(snapshotTime); }));
        }
    }
}
