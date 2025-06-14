using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using eatery_manager_server.Data.Models;
using eatery_manager_server.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace eatery_manager.Windows
{
    public partial class MenuWindow : Window
    {
        public ObservableCollection<eatery_manager_server.Data.Models.Menu> MenuItems { get; set; } = new();

        public MenuWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadMenuItems();
        }

        private void LoadMenuItems()
        {
            try
            {
                using var db = CreateDbContext();
                var list = db.Menu.OrderBy(m => m.Order).ThenBy(m => m.Name).ToList();

                MenuItems.Clear();
                foreach (var item in list)
                    MenuItems.Add(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania menu: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbName.Text) ||
                string.IsNullOrWhiteSpace(TbCategory.Text) ||
                string.IsNullOrWhiteSpace(TbPrice.Text) ||
                string.IsNullOrWhiteSpace(TbIngredients.Text))
            {
                MessageBox.Show("Proszę wypełnić wszystkie pola.");
                return;
            }

            if (!decimal.TryParse(TbPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Cena musi być liczbą większą od 0.");
                return;
            }

            try
            {
                using var db = CreateDbContext();

                if (selectedMenuItem == null)
                {
                    // Dodajemy nową pozycję
                    var newItem = new Menu
                    {
                        Name = TbName.Text.Trim(),
                        Category = TbCategory.Text.Trim(),
                        Price = price,
                        Ingredients = TbIngredients.Text.Trim(),
                        Order = db.Menu.Any() ? db.Menu.Max(m => m.Order) + 1 : 1
                    };

                    db.Menu.Add(newItem);
                    db.SaveChanges();

                    MenuItems.Add(newItem);
                }
                else
                {
                    // Edytujemy istniejącą pozycję
                    var itemToUpdate = db.Menu.Find(selectedMenuItem.Id);
                    if (itemToUpdate != null)
                    {
                        itemToUpdate.Name = TbName.Text.Trim();
                        itemToUpdate.Category = TbCategory.Text.Trim();
                        itemToUpdate.Price = price;
                        itemToUpdate.Ingredients = TbIngredients.Text.Trim();

                        db.SaveChanges();

                        // Aktualizujemy lokalnie ObservableCollection, by widok się odświeżył
                        selectedMenuItem.Name = itemToUpdate.Name;
                        selectedMenuItem.Category = itemToUpdate.Category;
                        selectedMenuItem.Price = itemToUpdate.Price;
                        selectedMenuItem.Ingredients = itemToUpdate.Ingredients;

                        // Odśwież DataGrid
                        var view = System.Windows.Data.CollectionViewSource.GetDefaultView(MenuDataGrid.ItemsSource);
                        view?.Refresh();
                    }
                }

                ClearInputFields();
                selectedMenuItem = null;
                BtnAdd.Content = "Dodaj";
                MenuDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania pozycji menu: {ex.Message}");
            }
        }


        private AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "database.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath};Mode=ReadWrite;Cache=Shared;Foreign Keys=True");
            return new AppDbContext(optionsBuilder.Options);
        }

        private Menu? selectedMenuItem = null;

        private void MenuDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MenuDataGrid.SelectedItem is Menu menu)
            {
                selectedMenuItem = menu;
                TbName.Text = menu.Name;
                TbCategory.Text = menu.Category;
                TbPrice.Text = menu.Price.ToString();
                TbIngredients.Text = menu.Ingredients;

                BtnAdd.Content = "Zapisz zmiany";
            }
            else
            {
                selectedMenuItem = null;
                ClearInputFields();
                BtnAdd.Content = "Dodaj";
            }
        }

        private void ClearInputFields()
        {
            TbName.Clear();
            TbCategory.Clear();
            TbPrice.Clear();
            TbIngredients.Clear();
        }
    }
}
