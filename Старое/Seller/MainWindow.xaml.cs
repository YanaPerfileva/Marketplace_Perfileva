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
using Marketplace.DAL.Repositories;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Seller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MarketplaceContext _context;
        private readonly SellerRepository _sellerRepo;
        private int _sellerId = 1;
        private int _productId;
        private int id;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions =
            new DbContextOptionsBuilder<MarketplaceContext>()
            .UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;")
            .Options;

        public MainWindow(int sellerId)
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
            _sellerRepo = new SellerRepository(_context);
            _sellerId = sellerId;

            SellerIdInput.Text = sellerId.ToString();
            SellerIdInput.Visibility = Visibility.Collapsed; 

            
            _ = LoadSellerDashboard(); 
        }

        private async Task LoadSellerDashboard() 
        {
            try
            {
                StatusLabel.Text = "Загрузка...";
                StatusLabel.Foreground = Brushes.Orange;

                var seller = await _sellerRepo.GetSellerWithDetailedInfoAsync(_sellerId);

                if (seller != null)
                {
                    UpdateDashboardFromSeller(seller);
                    SellerIdLabel.Text = $"Seller #{_sellerId} - {seller.StoreName}";
                    StatusLabel.Text = "Дашборд загружен!";
                    StatusLabel.Foreground = Brushes.Green;
                    
                }
                else
                {
                    StatusLabel.Text = $"Продавец #{_sellerId} не найден";
                    StatusLabel.Foreground = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Ошибка: {ex.Message}";
                StatusLabel.Foreground = Brushes.Red;
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void LoadSellerDashboard_Click(object sender, RoutedEventArgs e)
        {
            await LoadSellerDashboardAsync();
        }

        private async Task LoadSellerDashboardAsync()
        {
            try
            {
                StatusLabel.Text = "Загрузка дашборда...";
                var sellerId = int.Parse(SellerIdInput.Text);

                var sellerRepo = new SellerRepository(_context);
                var seller = await sellerRepo.GetSellerWithDetailedInfoAsync(sellerId);

                if (seller != null)
                {
                  
                    TotalProductsText.Text = seller.Products.Count.ToString();
                    

                    // Загружаем таблицу продуктов
                    var products = seller.Products.Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Brand = p.Brand ?? "",
                        CategoryName = "Категория", // Загружает из БД
                        Price = p.Skus.FirstOrDefault()?.Price ?? p.BasePrice,
                        Stock = p.Skus.Sum(s => s.Stock),
                        Status = p.IsActive ? "Активен" : "Неактивен"
                    }).ToList();

                    ProductsDataGrid.ItemsSource = products;
                    ProductsCountLabel.Text = $"({products.Count})";
                    SellerIdLabel.Text = $"Seller #{sellerId}";
                }

                StatusLabel.Text = "Готов";
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void UpdateDashboardFromSeller(Marketplace.Data.Entities.Seller seller)
        {
            TotalProductsText.Text = seller.Products.Count.ToString();
            TotalSkusText.Text = seller.Products.Sum(p => p.Skus?.Count ?? 0).ToString("N0");
            TotalStockText.Text = seller.Products.Sum(p => p.Skus.Sum(s => s.Stock)).ToString("N0");
            TotalOrdersText.Text = seller.Orders.Count.ToString();
            ItemsSoldText.Text = seller.TotalSales.ToString("N0");
            TotalRevenueText.Text = $"₽{(seller.TotalSales * 5000m):N0}";
            AvgRatingText.Text = seller.Rating?.ToString("F1") ?? "0.0";

            var topProducts = seller.Products.Take(3).Select(p => new
            {
                Name = p.Name,
                Brand = p.Brand ?? "—",
                OrdersCount = 10,
                SoldCount = p.Skus.Sum(s => s.Stock),
                Revenue = p.BasePrice * 1.5m
            }).ToList();

           
        }

        
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddProductWindow(_sellerId);
            if (addWindow.ShowDialog() == true)
                LoadSellerDashboard();
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditProductWindow(_sellerId);
            if (editWindow.ShowDialog() == true)
                LoadSellerDashboard();
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var deleteWindow = new DeleteProductWindow(_sellerId);
            if (deleteWindow.ShowDialog() == true)
                LoadSellerDashboard();
        }
       
   
    }
}