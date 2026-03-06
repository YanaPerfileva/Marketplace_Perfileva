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
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions =
            new DbContextOptionsBuilder<MarketplaceContext>()
            .UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;")
            .Options;

        public MainWindow()
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
            _sellerRepo = new SellerRepository(_context);
            SellerIdInput.Text = "1";
        }

        private async void LoadSellerDashboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(SellerIdInput.Text, out _sellerId) || _sellerId <= 0)
                {
                    StatusLabel.Text = "Введите ID продавца (1)";
                    StatusLabel.Foreground = Brushes.Red;
                    return;
                }

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
                    StatusLabel.Text = "Продавец не найден";
                    StatusLabel.Foreground = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"{ex.Message}";
                StatusLabel.Foreground = Brushes.Red;
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

            TopProductsListView.ItemsSource = topProducts;
            LowStockListView.ItemsSource = new List<object>();
        }

        //private void ReplenishStock_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("Функция пополнения (в разработке)", "Пополнить",
        //        MessageBoxButton.OK, MessageBoxImage.Information);
        //}


        //private void NewProductButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var productWindow = new ProductWindow(_sellerId);  
        //    if (productWindow.ShowDialog() == true)
        //        LoadSellerDashboard_Click(null, null);
        //}


        //private async void EditProductButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var productWindow = new ProductWindow(_sellerId); 
        //    productWindow.ShowDialog();
        //}
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddProductWindow(_sellerId);
            if (addWindow.ShowDialog() == true)
                LoadSellerDashboard_Click(null, null);
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditProductWindow(_sellerId);
            if (editWindow.ShowDialog() == true)
                LoadSellerDashboard_Click(null, null);
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var deleteWindow = new DeleteProductWindow(_sellerId);
            if (deleteWindow.ShowDialog() == true)
                LoadSellerDashboard_Click(null, null);
        }
    }
}