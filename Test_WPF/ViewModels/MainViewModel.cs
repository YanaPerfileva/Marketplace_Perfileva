using Marketplace.BLL.Interfaces;
using Marketplace.Wpf.Infrastructure;
using Marketplace.Data.Context;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Windows.Input;

namespace Marketplace.Wpf.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly IMarketplaceProbeService _probeService;
        private readonly IConfiguration _configuration;
        private string _logText = "Ready.";
        private readonly SemaphoreSlim _contextGate = new(1, 1);
        private bool _isDbCreatorVisible;

        public MainViewModel(IMarketplaceProbeService probeService, IConfiguration configuration)
        {
            _probeService = probeService;
            _configuration = configuration;

            LoadProductsCommand = new RelayCommand(LoadProductsAsync);
            LoadCategoriesCommand = new RelayCommand(LoadCategoriesAsync);
            LoadPromotionsCommand = new RelayCommand(LoadPromotionsAsync);
            LoadFavoritesCommand = new RelayCommand(LoadFavoritesAsync);
            LoadLogsCommand = new RelayCommand(LoadLogsAsync);
            LoadActiveProductsViewCommand = new RelayCommand(LoadActiveProductsViewAsync);
            LoadPopularProductsViewCommand = new RelayCommand(LoadPopularProductsViewAsync);
            LoadSellerDashboardCommand = new RelayCommand(LoadSellerDashboardAsync);
            TestPriceTriggerCommand = new RelayCommand(TestPriceTriggerAsync);
            TestStockTriggerCommand = new RelayCommand(TestStockTriggerAsync);
            ToggleDbCreatorCommand = new RelayCommand(ToggleDbCreatorAsync);
            CreateDatabaseCommand = new RelayCommand(CreateDatabaseAsync);

            NewConnectionString = configuration.GetConnectionString("MarketplaceDb") ?? string.Empty;
        }

        public ObservableCollection<object> Rows { get; } = new();

        public string SellerIdText { get; set; } = "1";
        public string UserIdText { get; set; } = "1";
        public string CategoryIdText { get; set; } = "1";
        public string SearchText { get; set; } = string.Empty;
        public string NewConnectionString { get; set; } = string.Empty;

        public bool IsDbCreatorVisible
        {
            get => _isDbCreatorVisible;
            set
            {
                _isDbCreatorVisible = value;
                OnPropertyChanged();
            }
        }

        public string LogText
        {
            get => _logText;
            set
            {
                _logText = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadProductsCommand { get; }
        public ICommand LoadCategoriesCommand { get; }
        public ICommand LoadPromotionsCommand { get; }
        public ICommand LoadFavoritesCommand { get; }
        public ICommand LoadLogsCommand { get; }
        public ICommand LoadActiveProductsViewCommand { get; }
        public ICommand LoadPopularProductsViewCommand { get; }
        public ICommand LoadSellerDashboardCommand { get; }
        public ICommand TestPriceTriggerCommand { get; }
        public ICommand TestStockTriggerCommand { get; }
        public ICommand ToggleDbCreatorCommand { get; }
        public ICommand CreateDatabaseCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async Task LoadProductsAsync()
        {
            await LoadPagedAsync(
                () => _probeService.GetProductsAsync(SearchText, 1, 100),
                "Loaded products");
        }

        private async Task LoadCategoriesAsync()
        {
            await LoadPagedAsync(
                () => _probeService.GetCategoriesAsync(1, 100),
                "Loaded categories");
        }

        private async Task LoadPromotionsAsync()
        {
            await LoadPagedAsync(
                () => _probeService.GetPromotionsAsync(1, 100),
                "Loaded promotions");
        }

        private async Task LoadFavoritesAsync()
        {
            var userId = ParseOrDefault(UserIdText, 1);
            await LoadPagedAsync(
                () => _probeService.GetFavoritesAsync(userId, 1, 100),
                $"Loaded favorites for user {userId}");
        }

        private async Task LoadLogsAsync()
        {
            var userId = ParseOrDefault(UserIdText, 1);
            await LoadPagedAsync(
                () => _probeService.GetUserLogsAsync(userId, 1, 100),
                $"Loaded logs for user {userId}");
        }

        private async Task LoadActiveProductsViewAsync()
        {
            await LoadRowsAsync(
                () => _probeService.GetActiveProductsViewAsync(),
                "Loaded v_active_products rows");
        }

        private async Task LoadPopularProductsViewAsync()
        {
            await LoadRowsAsync(
                () => _probeService.GetPopularProductsViewAsync(),
                "Loaded v_popular_products rows");
        }

        private async Task LoadPagedAsync<T>(Func<Task<Marketplace.DAL.Models.PaginatedResult<T>>> query, string message)
        {
            await ExecuteSerializedAsync(async () =>
            {
                var result = await query();
                FillRows(result.Items.Cast<object>());
                AppendLog($"{message}: {result.Items.Count()} / {result.TotalCount}");
            });
        }

        private async Task LoadRowsAsync<T>(Func<Task<IReadOnlyCollection<T>>> query, string message)
        {
            await ExecuteSerializedAsync(async () =>
            {
                var rows = await query();
                FillRows(rows.Cast<object>());
                AppendLog($"{message}: {rows.Count}");
            });
        }

        private async Task LoadSellerDashboardAsync()
        {
            await ExecuteSerializedAsync(async () =>
            {
                var sellerId = ParseOrDefault(SellerIdText, 1);
                var dashboard = await _probeService.GetSellerDashboardProbeAsync(sellerId);

                var rows = dashboard.Products.Cast<object>().ToList();
                FillRows(rows);

                var json = JsonSerializer.Serialize(dashboard.Dashboard, new JsonSerializerOptions { WriteIndented = true });
                AppendLog($"Seller dashboard via stored procedure for seller={sellerId}:{Environment.NewLine}{json}");
                AppendLog($"Seller categories count: {dashboard.Categories.Count}");
            });
        }

        private async Task TestPriceTriggerAsync()
        {
            await ExecuteSerializedAsync(async () =>
            {
                var sellerId = ParseOrDefault(SellerIdText, 1);
                var categoryId = ParseOrDefault(CategoryIdText, 1);
                var result = await _probeService.VerifyPriceHistoryTriggerAsync(sellerId, categoryId);
                AppendLog($"Price trigger test: {(result.Success ? "OK" : "FAIL")} - {result.Message}");
            });
        }

        private async Task TestStockTriggerAsync()
        {
            await ExecuteSerializedAsync(async () =>
            {
                var sellerId = ParseOrDefault(SellerIdText, 1);
                var categoryId = ParseOrDefault(CategoryIdText, 1);
                var userId = ParseOrDefault(UserIdText, 1);
                var result = await _probeService.VerifyOrderStockTriggerAsync(userId, sellerId, categoryId);
                AppendLog($"Stock trigger test: {(result.Success ? "OK" : "FAIL")} - {result.Message}");
            });
        }

        private async Task ExecuteSerializedAsync(Func<Task> action)
        {
            await _contextGate.WaitAsync();
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                AppendLog($"ERROR: {ex.Message}");
            }
            finally
            {
                _contextGate.Release();
            }
        }

        private Task ToggleDbCreatorAsync()
        {
            IsDbCreatorVisible = !IsDbCreatorVisible;
            AppendLog(IsDbCreatorVisible
                ? "Database creation panel opened."
                : "Database creation panel hidden.");
            return Task.CompletedTask;
        }

        private async Task CreateDatabaseAsync()
        {
            await ExecuteSerializedAsync(async () =>
            {
                var connectionString = string.IsNullOrWhiteSpace(NewConnectionString)
                    ? _configuration.GetConnectionString("MarketplaceDb")
                    : NewConnectionString;

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    AppendLog("ERROR: Connection string is empty.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewConnectionString))
                {
                    AppendLog("Connection string field is empty. Using configured MarketplaceDb connection.");
                    NewConnectionString = connectionString;
                }

                var options = new DbContextOptionsBuilder<MarketplaceContext>()
                    .UseMarketplaceSqlServer(connectionString)
                    .Options;

                var connectionInfo = new SqlConnectionStringBuilder(connectionString);
                AppendLog($"Target SQL Server: {connectionInfo.DataSource}; Database: {connectionInfo.InitialCatalog}");

                await using var db = new MarketplaceContext(options);
                AppendLog("Creating/updating database via EF migrations...");
                // MigrateAsync creates the database if it does not exist and then applies migrations.
                await db.Database.MigrateAsync();
                AppendLog("Database is ready. Migrations applied successfully.");
            });
        }

        private void FillRows(IEnumerable<object> values)
        {
            Rows.Clear();
            foreach (var value in values)
            {
                Rows.Add(value);
            }
        }

        private void AppendLog(string line)
        {
            LogText = $"{DateTime.Now:HH:mm:ss} | {line}{Environment.NewLine}{Environment.NewLine}{LogText}";
        }

        private static int ParseOrDefault(string text, int fallback) => int.TryParse(text, out var value) ? value : fallback;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
