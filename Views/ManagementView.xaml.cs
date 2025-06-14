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
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using eatery_manager;

namespace eatery_manager.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ManagementView.xaml
    /// </summary>
    public partial class ManagementView : UserControl, INotifyPropertyChanged
    {
        private string _selectedFolderPath;
           private void LoadConfiguration()
        {
            try
            {
                string configPath = AppSettings.settingsFilePathGetter();
                if (!File.Exists(configPath))
                {
                    AppendLog($"Plik konfiguracyjny nie istnieje: {configPath}");
                    return;
                }

                string json = File.ReadAllText(configPath);
                AppendLog("Wczytany JSON: " + json);

                var config = JsonSerializer.Deserialize<AppSettings>(json);

                if (config == null)
                {
                    AppendLog("Deserializacja zwróciła null.");
                    return;
                }

                AppendLog($"Deserializacja OK: HTTP={config.httpState}, HTTPS={config.httpsState}, HTTP port={config.httpPortListen}, HTTPS port={config.httpsPortListen}, Path={config.mainFilesPath}");

                HttpCheckBox.IsChecked = config.httpState;
                HttpsCheckBox.IsChecked = config.httpsState;
                HttpPortTextBox.Text = config.httpPortListen.ToString();
                HttpsPortTextBox.Text = config.httpsPortListen.ToString();

                _selectedFolderPath = config.mainFilesPath;
                UpdatePathDisplay();
            }
            catch (Exception ex)
            {
                AppendLog($"Błąd podczas wczytywania konfiguracji: {ex.Message}");
            }
        }


        private string _logs;
        public string Logs
        {
            get => _logs;
            set { _logs = value; OnPropertyChanged(nameof(Logs)); }
        }

        public ManagementView()
        {
            InitializeComponent();
            DataContext = this; // <- najpierw ustaw DataContext!
            _webServer = new WebServerService(AppendLog);
            LoadConfiguration(); // <- dopiero teraz wczytaj konfigurację

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
        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Używamy "hacka" z OpenFileDialog do wyboru folderu, 
            // ponieważ nie ma wbudowanego dialogu folderu w WPF bez dodatkowych bibliotek.
            var dialog = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Wybierz folder", // Tekst pomocniczy, nie będzie widoczny w tytule
                Title = "Wybierz katalog z plikami strony WWW"
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedFolderPath = Path.GetDirectoryName(dialog.FileName);
                UpdatePathDisplay();
            }
        }

        private void UpdatePathDisplay()
        {
            if (string.IsNullOrEmpty(_selectedFolderPath))
            {
                SelectedPathTextBlock.Text = "Nie wybrano katalogu";
                SelectedPathTextBlock.ToolTip = null;
                SelectedPathTextBlock.FontStyle = FontStyles.Italic;
                SelectedPathTextBlock.Foreground = System.Windows.Media.Brushes.Gray;
            }
            else
            {
                // Ustaw pełną ścieżkę jako ToolTip
                SelectedPathTextBlock.ToolTip = _selectedFolderPath;

                // Ustaw skróconą ścieżkę jako tekst widoczny
                SelectedPathTextBlock.Text = ShortenPath(_selectedFolderPath);
                SelectedPathTextBlock.FontStyle = FontStyles.Normal;
                SelectedPathTextBlock.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private string ShortenPath(string path, int maxLength = 50)
        {
            if (path.Length <= maxLength)
                return path;

            // Skracanie: Pokaż początek i koniec ścieżki
            // np. "C:\Users\...\Documents\Project"
            string ellipsis = "\\...\\";
            string[] parts = path.Split(Path.DirectorySeparatorChar);

            if (parts.Length < 4) return path; // Zbyt krótka, by skracać

            string start = parts[0]; // np. "C:"
            string end = Path.Combine(parts[parts.Length - 2], parts[parts.Length - 1]);

            return start + ellipsis + end;
        }
        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var configToSave = new AppSettings
                {
                    httpState = HttpCheckBox.IsChecked ?? false,
                    httpsState = HttpsCheckBox.IsChecked ?? false,
                    httpPortListen = int.TryParse(HttpPortTextBox.Text, out int httpPort) ? httpPort : 8080,
                    httpsPortListen = int.TryParse(HttpsPortTextBox.Text, out int httpsPort) ? httpsPort : 8443,
                    mainFilesPath = _selectedFolderPath ?? ""
                };

                configToSave.Save();
                AppendLog("Ustawienia zapisane.");
            }
            catch (Exception ex)
            {
                AppendLog($"Błąd zapisu ustawień: {ex.Message}");
            }
        }

    }
}

