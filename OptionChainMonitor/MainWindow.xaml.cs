using OptionChainMonitor.ViewModel;
using System.Windows;

namespace OptionChainMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new OptionChainMonitorViewModel();           
        }
    }
}
