using System.Windows;
using SystemMonitor.UI.ViewModels;

namespace SystemMonitor.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Closed += (s, e) => viewModel.Dispose();
        }
    }
}