using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace eatery_manager
{
    public class AppSettings
    {
        public bool httpState { get; set; }
        public bool httpsState { get; set; }
        public int httpPortListen { get; set; }
        public int httpsPortListen { get; set; }
        public int roomSizeX { get; set; }
        public int roomSizeY { get; set; }
        public string mainFilesPath { get; set; }

        public static AppSettings? Load()
        {
            try
            {
                string filePath = settingsFilePathGetter();
                if (!File.Exists(filePath))
                {
                    CreateDefaultSettingsFileIfNotExists();
                }

                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<AppSettings>(jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania pliku ustawień: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static string settingsFilePathGetter()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "eateryManagerSettings.json");
            return path;
        }

        public static void CreateDefaultSettingsFileIfNotExists()
        {
            try
            {
                string filePath = settingsFilePathGetter();

                if (!File.Exists(filePath))
                {
                    var defaultSettings = new AppSettings
                    {
                        httpState = false,
                        httpsState = true,
                        httpPortListen = 5000,
                        httpsPortListen = 5001,
                        roomSizeX = 5,
                        roomSizeY = 5,
                        mainFilesPath = AppContext.BaseDirectory
                    };

                    string jsonString = JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, jsonString);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można utworzyć pliku konfiguracyjnego AppSettings.json.\nBłąd: {ex.Message}", "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // NOWA METODA do zapisu aktualnych ustawień do pliku JSON
        public void Save()
        {
            try
            {
                string filePath = settingsFilePathGetter();
                string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);
                MessageBox.Show("Ustawienia zostały zapisane pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu ustawień: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
