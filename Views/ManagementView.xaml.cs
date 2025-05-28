using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace eatery_manager.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ManagementView.xaml
    /// </summary>
    public partial class ManagementView : UserControl, INotifyPropertyChanged
    {
        private string _logs;
        public string Logs
        {
            get => _logs;
            set { _logs = value; OnPropertyChanged(nameof(Logs)); }
        }

        public ManagementView()
        {
            InitializeComponent();
            DataContext = this;
            _webServer = new WebServerService(AppendLog); // przekazujemy delegat do logowania
        }

        private WebServerService _webServer;

        public void AppendLog(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => AppendLog(message));
                return;
            }
            Logs += message + Environment.NewLine;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            _webServer.Start();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _webServer.Stop();
        }
    }
}

