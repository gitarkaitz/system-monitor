using System.Windows;

namespace SystemMonitor.UI
{
    public partial class AlertNotificationWindow : Window
    {
        public string Message { get; }
        public bool WasAcknowledged { get; private set; }

        public AlertNotificationWindow(string message)
        {
            InitializeComponent();
            Message = message;
            DataContext = this;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            WasAcknowledged = true;
            Close();
        }

        private void OnAcceptClick(object sender, RoutedEventArgs e)
        {
            WasAcknowledged = true;
            Close();
        }
    }
}
