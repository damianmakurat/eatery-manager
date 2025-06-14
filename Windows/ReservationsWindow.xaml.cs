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
    public partial class ReservationsWindow : Window
    {
        public ObservableCollection<Reservations> Reservations { get; set; } = new();

        private readonly DispatcherTimer _refreshTimer;
        private List<int> _previousReservationIds = new();

        public ReservationsWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadReservations();

            // Timer ustawiony na odświeżanie co 5 sekund
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _refreshTimer.Tick += (s, e) => LoadReservations();
            _refreshTimer.Start();
        }

        private bool _isFirstLoad = true;

        private void LoadReservations()
        {
            try
            {
                using var db = CreateDbContext();
                var list = db.Reservations
                    .OrderBy(r => r.Date)
                    .ThenBy(r => r.StartTime)
                    .ToList();

                if (_isFirstLoad)
                {
                    // Pierwsze załadowanie — nie pokazuj powiadomień, tylko zapisz ID
                    _previousReservationIds = list.Select(r => r.ReservationId).ToList();
                    _isFirstLoad = false;
                }
                else
                {
                    // Znajdź nowe rezerwacje, których nie było wcześniej
                    var newReservations = list
                        .Where(r => !_previousReservationIds.Contains(r.ReservationId))
                        .ToList();

                    // Jeśli są nowe, pokaż powiadomienia
                    if (newReservations.Any())
                    {
                        foreach (var newRes in newReservations)
                        {
                            MessageBox.Show(
                                $"Nowa rezerwacja: {newRes.Name} {newRes.Surname} na dzień {newRes.Date:yyyy-MM-dd} od {newRes.StartTime} do {newRes.EndTime}",
                                "Nowa rezerwacja",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                    }

                    // Zaktualizuj listę ID rezerwacji
                    _previousReservationIds = list.Select(r => r.ReservationId).ToList();
                }

                Reservations.Clear();
                foreach (var r in list)
                    Reservations.Add(r);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania rezerwacji: {ex.Message}");
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
