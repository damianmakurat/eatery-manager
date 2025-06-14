using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using eatery_manager_server.Data.Models;
using eatery_manager_server.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace eatery_manager.Windows
{
    public partial class TablesWindow : Window
    {
        public ObservableCollection<Tables> Tables { get; set; } = new();

        private readonly DispatcherTimer _refreshTimer;

        public TablesWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadTables();

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _refreshTimer.Tick += (s, e) => LoadTables();
            _refreshTimer.Start();
        }

        private void LoadTables()
        {
            try
            {
                using var db = CreateDbContext();
                var list = db.Tables.OrderBy(t => t.TableId).ToList();

                Tables.Clear();
                foreach (var table in list)
                    Tables.Add(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania stolików: {ex.Message}");
            }
        }

        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            string locationX = TbLocationX.Text.Trim();
            string locationY = TbLocationY.Text.Trim();
            string capacityText = TbCapacity.Text.Trim();

            if (string.IsNullOrEmpty(locationX) || string.IsNullOrEmpty(locationY) || string.IsNullOrEmpty(capacityText))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(capacityText, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Ilość miejsc musi być dodatnią liczbą całkowitą.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var db = CreateDbContext();
                var newTable = new Tables
                {
                    LocationX = locationX,
                    LocationY = locationY,
                    Capacity = capacity
                };

                db.Tables.Add(newTable);
                db.SaveChanges();

                // Odśwież listę po dodaniu
                LoadTables();

                // Wyczyść pola
                TbLocationX.Clear();
                TbLocationY.Clear();
                TbCapacity.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania stolika: {ex.Message}");
            }
        }

        private AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "database.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath};Mode=ReadWrite;Cache=Shared;Foreign Keys=True");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
