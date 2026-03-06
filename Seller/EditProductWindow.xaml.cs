using System;
using System.Collections.Generic;
using System.IO;
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
using Marketplace.DAL.Repositories;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Seller
{
    /// <summary>
    /// Логика взаимодействия для EditProductWindow.xaml
    /// </summary>

        public partial class EditProductWindow : Window
        {
            private readonly MarketplaceContext _context;
            private readonly int _sellerId;
            private List<Product> _sellerProducts = new();
            private List<Category> _categories = new();
            private Product? _selectedProduct;
        private string _selectedMainImagePath;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions =
                new DbContextOptionsBuilder<MarketplaceContext>()
                .UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;")
                .Options;

            public EditProductWindow(int sellerId)
            {
                InitializeComponent();
                _sellerId = sellerId;
                SellerIdInput.Text = _sellerId.ToString();
                _context = new MarketplaceContext(_dbOptions);
                _ = LoadInitialDataAsync();  
            }

            private async Task LoadInitialDataAsync()
            {
                try
                {
                    _sellerProducts = await _context.Products
                        .Where(p => p.SellerId == _sellerId)
                        .OrderBy(p => p.Name)
                        .ToListAsync();

                    ProductsComboBox.ItemsSource = _sellerProducts;

                    _categories = await _context.Categories.ToListAsync();
                    CategoryComboBox.ItemsSource = _categories;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }

            }
        private async void ProductsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsComboBox.SelectedItem is Product product)
            {
                _selectedProduct = product;
                ProductIdText.Text = $"ID: {product.Id}";

                NameTextBox.Text = product.Name;
                BrandTextBox.Text = product.Brand ?? "";
                PriceTextBox.Text = product.BasePrice.ToString("F2");
                DescriptionTextBox.Text = product.Description ?? "";
                CategoryComboBox.SelectedValue = product.CategoryId;

              if (!string.IsNullOrEmpty(product.MainImageUrl))
                {
                    try
                    {
                        MainImagePreview.Source = new BitmapImage(new Uri(product.MainImageUrl));
                        MainImageText.Text = $"{System.IO.Path.GetFileName(product.MainImageUrl)}";
                    }
                    catch
                    {
                        MainImageText.Text = "Ошибка изображения";
                    }
                }
            }
        }
        private string CopyImageToAppFolder(string sourcePath, string type)
        {
            string imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductImages");
            Directory.CreateDirectory(imagesFolder);
            string fileName = $"{type}_{DateTime.Now:yyyyMMddHHmmss}_{System.IO.Path.GetFileName(sourcePath)}";
            string destPath = System.IO.Path.Combine(imagesFolder, fileName);
            System.IO.File.Copy(sourcePath, destPath, true);
            return destPath;
        }

        private void SelectMainImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                _selectedMainImagePath = CopyImageToAppFolder(dialog.FileName, "main");
                MainImagePreview.Source = new BitmapImage(new Uri(_selectedMainImagePath));
                MainImageText.Text = $" {System.IO.Path.GetFileName(_selectedMainImagePath)}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // 5. SKU (временно)
        private void EditSkusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Сначала выберите товар!");
                ProductsComboBox.Focus();
                return;
            }

            // ✅ ОТКРЫВАЕМ ваше окно редактирования SKU!
            var skuWindow = new EditSkuWindow(_sellerId, _selectedProduct.Id);  // Параметры!
            skuWindow.ShowDialog();

            // Обновляем ComboBox после редактирования SKU
            _ = LoadInitialDataAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите товар из списка!");
                ProductsComboBox.Focus();
                return;
            }

            if (!ValidateInput()) return;

            try
            {
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Сохранение...";

                // Обновляем товар напрямую через context
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == _selectedProduct.Id && p.SellerId == _sellerId);

                if (product == null)
                {
                    MessageBox.Show("Товар не найден!");
                    return;
                }

                // Обновляем все поля
                product.Name = NameTextBox.Text.Trim();
                product.Brand = string.IsNullOrWhiteSpace(BrandTextBox.Text) ? null : BrandTextBox.Text.Trim();
                product.Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim();
                product.BasePrice = decimal.Parse(PriceTextBox.Text);
                product.CategoryId = (int)CategoryComboBox.SelectedValue!;
                product.MainImageUrl = _selectedMainImagePath ?? product.MainImageUrl;
                product.UpdatedAt = DateTime.UtcNow;
                product.IsActive = true;

                await _context.SaveChangesAsync();

                MessageBox.Show("Товар успешно обновлён!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
            finally
            {
                SaveButton.IsEnabled = true;
                SaveButton.Content = "Обновить товар";
            }
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Название обязательно!");
                NameTextBox.Focus();
                return false;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out _) || decimal.Parse(PriceTextBox.Text) <= 0)
            {
                MessageBox.Show("Цена > 0!");
                PriceTextBox.Focus();
                return false;
            }

            if (CategoryComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию!");
                return false;
            }

            return true;
        }

    }
}