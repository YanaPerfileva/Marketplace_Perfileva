using System.Configuration;
using System.Data;
using System.Windows;
using Marketplace.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Main_menu
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // СОЗДАЕМ/МИГРИРУЕМ БД ПРИ ЗАПУСКЕ
            var success = await CreateDatabaseAsync();

            if (success)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Ошибка создания БД! Приложение остановлено.", "Ошибка БД");
                Shutdown();
            }
        }

        private static async Task<bool> CreateDatabaseAsync()
        {
            try
            {
                string connectionString = @"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;";

                var options = new DbContextOptionsBuilder<MarketplaceContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                await using var db = new MarketplaceContext(options);
                await db.Database.MigrateAsync();  // Создает БД + применяет миграции

                MessageBox.Show("База данных готова!", "Marketplace");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}");
                return false;
            }
        }
    }
}


