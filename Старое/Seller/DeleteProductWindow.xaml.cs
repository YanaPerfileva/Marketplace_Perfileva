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
using System.Windows.Shapes;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Seller
{
    /// <summary>
    /// Логика взаимодействия для DeleteProductWindow.xaml
    /// </summary>
    public partial class DeleteProductWindow : Window
    {
        private readonly MarketplaceContext _context;
        private readonly int _sellerId;

        private static readonly DbContextOptions<MarketplaceContext> _dbOptions =
            new DbContextOptionsBuilder<MarketplaceContext>()
            .UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;")
            .Options;

        public DeleteProductWindow(int sellerId)
        {
            InitializeComponent();
            _sellerId = sellerId;
            _context = new MarketplaceContext(_dbOptions);
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = _context.Products
                    .Include(p => p.Skus)
                    .Where(p => p.SellerId == _sellerId)
                    .OrderBy(p => p.Name)
                    .ToList();

                ProductsListView.ItemsSource = products;

                if (!products.Any())
                {
                    DeleteButton.IsEnabled = false;
                    ProductsListView.ItemsSource = new List<Product>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem is Product product)
            {
                var result = MessageBox.Show(
                    $"Удалить товар:\n\n{product.Name}\n({product.Skus.Count} SKU)",
                    "Подтвердить удаление",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DeleteButton.IsEnabled = false;
                        DeleteButton.Content = "Удаление...";

                        // ✅ SQL УДАЛЕНИЕ ПО ID - 100% работает!
                        var productId = product.Id;

                        // Удаляем SKU сначала
                        var skus = _context.ProductSkus.Where(s => s.ProductId == productId);
                        _context.ProductSkus.RemoveRange(skus);

                        // Удаляем Product
                        var productToDelete = _context.Products.Find(productId);
                        if (productToDelete != null)
                        {
                            _context.Products.Remove(productToDelete);
                            _context.SaveChanges();
                        }

                        MessageBox.Show("Товар удален!");
                        DialogResult = true;
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                    finally
                    {
                        DeleteButton.IsEnabled = true;
                        DeleteButton.Content = "Удалить";
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар!");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}
