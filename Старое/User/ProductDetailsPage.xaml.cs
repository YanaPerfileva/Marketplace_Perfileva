using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Repositories;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq; 
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
using static User.AuthWindow;

namespace User
{
    /// <summary>
    /// Логика взаимодействия для ProductDetailsPage.xaml
    /// </summary>
    public partial class ProductDetailsPage : Page
    {
        private List<ProductSku> _currentProduct = null!;
        private string? _selectedColorHex;

        private readonly MarketplaceContext _context;
        private readonly IProductRepository _repo;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
        UseSqlServer(@"Server =.\SQLEXPRESS; Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public ProductDetailsPage(int productId)
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
            _repo = new ProductRepository(_context);
            LoadProduct(productId);
        }
        private async void LoadProduct(int id)
        {
            var product = await _repo.GetProductWithSellerAsync(id);

            if (product != null)
            {
                this.DataContext = product;

                var availableSkus = product.Skus.Where(s => s.IsActive && s.Stock > 0).ToList();

                _currentProduct = availableSkus;

                if (availableSkus.Count != 0)
                {
                    SizeComboBox.ItemsSource = availableSkus.Select(s => s.Size).Distinct().ToList();

                    ColorSelector.ItemsSource = availableSkus.GroupBy(s => s.ColorHex).Select(g => g.First()).ToList();

                    SizeComboBox.SelectedIndex = 0;
                }
                else
                {
                    SizeComboBox.ItemsSource = null;
                    ColorSelector.ItemsSource = null;
                    SizeComboBox.IsEnabled = false;
                    Color.Visibility = Visibility.Collapsed;
                    StockStatus.Text = "ТОВАРА НЕТ В НАЛИЧИИ";
                    StockStatus.Foreground = Brushes.Red;
                    StockStatus.FontSize = 16;
                    ProductPrice.Text = "--- ₽";
                    AddToCartButton.IsEnabled = false;

                }

                ProductName.Text = product.Name;
                Seller.Text = product.Seller.StoreName;
                ProductDescription.Text = product.Description;
                ProductPrice.Text = $"{product.BasePrice:N0} ₽";

                if (!string.IsNullOrEmpty(product.MainImageUrl))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(product.MainImageUrl, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ProductImage.Source = bitmap;
                }
            }
        }

        private void SkuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePriceAndStock();
        }

        private void UpdatePriceAndStock()
        {
            if (_currentProduct.Count == 0) return;

            string? selectedSize = SizeComboBox.SelectedItem as string;

            string? selectedColorHex = _selectedColorHex;

            var selectedSku = _currentProduct.FirstOrDefault(s =>
                s.Size == selectedSize &&
                s.ColorHex == selectedColorHex);

            if (selectedSku != null)
            {
                ProductPrice.Text = $"{selectedSku.Price:N0} ₽";
                int available = selectedSku.Stock - (selectedSku.ReservedStock);
                if (available > 0)
                {
                    StockStatus.Text = $"В наличии: {available} шт.";
                    StockStatus.Foreground = Brushes.Green;
                    AddToCartButton.IsEnabled = true;
                }
                else if (selectedSku.Stock > 0 && available <= 0)
                {
                    StockStatus.Text = "Забронировано";
                    StockStatus.Foreground = Brushes.Orange;
                    AddToCartButton.IsEnabled = false;
                }
                else
                {
                    SizeComboBox.IsEnabled = false;
                    Color.Visibility = Visibility.Collapsed;
                    StockStatus.Text = "ТОВАРА НЕТ В НАЛИЧИИ";
                    StockStatus.Foreground = Brushes.Red;
                    StockStatus.FontSize = 16;
                    AddToCartButton.IsEnabled = false;
                }
            }
            else
            {
                ProductPrice.Text = "---";
                AddToCartButton.IsEnabled = false;
            }
        }

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                _selectedColorHex = button.Tag?.ToString() ?? string.Empty;
                UpdatePriceAndStock();
            }
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack) this.NavigationService.GoBack();
        }

        private async void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {

            var selectedSize = SizeComboBox.SelectedItem as string;
            var selectedSku = _currentProduct?.FirstOrDefault(s =>
                s.Size == selectedSize &&
                s.ColorHex == _selectedColorHex &&
                s.IsActive);

            if (selectedSku == null)
            {
                MessageBox.Show("Выберите размер и цвет!");
                return;
            }
            else
            {
                if (UserSession.CurrentUser == null)
                {
                    MessageBox.Show("Пожалуйста, войдите в аккаунт, чтобы добавить товар в корзину", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                int currentUserId = UserSession.CurrentUser.Id;

                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == currentUserId && c.IsActive);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = currentUserId,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var existingItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.SkuId == selectedSku.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                    _context.Entry(existingItem).State = EntityState.Modified;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = cart.Id,
                        SkuId = selectedSku.Id,
                        Quantity = 1,
                        AddedAt = DateTime.Now
                    };
                    _context.CartItems.Add(newItem);
                }

                selectedSku.Stock -= 1;

                await _context.SaveChangesAsync();

                MessageBox.Show($"Добавлено: {selectedSku?.Product?.Name} ({selectedSku?.Size})", "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
    
}


