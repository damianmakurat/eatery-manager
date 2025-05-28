using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eatery_manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void ShowContent(UserControl control)
        {
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(control);
        }

        private void Management_Click(object sender, RoutedEventArgs e)
        {
            ShowContent(new Views.ManagementView());
        }

        private void Tables_Click(object sender, RoutedEventArgs e)
        {
            ShowContent(new Views.TablesView());
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            ShowContent(new Views.OrdersView());
        }
        private void Reservations_Click(object sender, RoutedEventArgs e)
        {
            ShowContent(new Views.ReservationsView());
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            ShowContent(new Views.MenuView());
        }

    }
}