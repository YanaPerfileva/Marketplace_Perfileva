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
using Microsoft.Win32;

namespace Seller
{
    /// <summary>
    /// Логика взаимодействия для AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private readonly MarketplaceContext _context;
        private readonly int _sellerId;
        private List<Category> _categories = new();
        private string? _selectedMainImagePath;

        private static readonly DbContextOptions<MarketplaceContext> _dbOptions =
            new DbContextOptionsBuilder<MarketplaceContext>()
            .UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;")
            .Options;

        public AddProductWindow(int sellerId)
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
                _categories = await _context.Categories.ToListAsync();
                CategoryComboBox.ItemsSource = _categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Создание...";

                int categoryId = (int)CategoryComboBox.SelectedValue!;
                decimal price = decimal.Parse(PriceTextBox.Text);
                int stock = int.Parse(SkuStockTextBox.Text);

                var product = new Product
                {
                    SellerId = _sellerId,
                    Name = NameTextBox.Text.Trim(),
                    Brand = BrandTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    BasePrice = price,
                    CategoryId = categoryId,
                    MainImageUrl = _selectedMainImagePath,
                    Skus = new List<ProductSku>
                    {
                        new ProductSku
                        {
                            SkuCode = $"SKU-{DateTime.Now:yyyyMMddHHmmss}",
                            Size = SkuSizeTextBox.Text.Trim(),
                            Color = SkuColorTextBox.Text.Trim(),
                            Stock = stock
                        }
                    }
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                MessageBox.Show("Товар + SKU успешно созданы!", "Успех",
                               MessageBoxButton.OK, MessageBoxImage.Information);
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
                SaveButton.Content = "Создать товар";
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) { MessageBox.Show("Название!"); NameTextBox.Focus(); return false; }
            if (!decimal.TryParse(PriceTextBox.Text, out _) || decimal.Parse(PriceTextBox.Text) <= 0) { MessageBox.Show("Цена > 0!"); PriceTextBox.Focus(); return false; }
            if (CategoryComboBox.SelectedValue == null) { MessageBox.Show("Категория!"); return false; }

            
            if (string.IsNullOrWhiteSpace(SkuSizeTextBox.Text)) { MessageBox.Show("Размер SKU!"); return false; }
            if (string.IsNullOrWhiteSpace(SkuColorTextBox.Text)) { MessageBox.Show("Цвет SKU!"); return false; }
            if (!int.TryParse(SkuStockTextBox.Text, out int stock) || stock <= 0) { MessageBox.Show("Остаток > 0!"); SkuStockTextBox.Focus(); return false; }
            if (!decimal.TryParse(SkuPriceTextBox.Text, out _) || decimal.Parse(SkuPriceTextBox.Text) <= 0) { MessageBox.Show("Цена SKU > 0!"); SkuPriceTextBox.Focus(); return false; }

            return true;
        }

        private void SelectMainImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите главное изображение товара"
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedMainImagePath = CopyImageToAppFolder(dialog.FileName, "main");
                MainImagePreview.Source = new BitmapImage(new Uri(_selectedMainImagePath));
                MainImageText.Text = $"{System.IO.Path.GetFileName(_selectedMainImagePath)}";
            }
        }

        private string CopyImageToAppFolder(string sourcePath, string type)
        {
            string imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductImages");
            Directory.CreateDirectory(imagesFolder);

            string fileName = $"{type}_{DateTime.Now:yyyyMMddHHmmss}_{System.IO.Path.GetFileName(sourcePath)}";
            string destPath = System.IO.Path.Combine(imagesFolder, fileName);

            File.Copy(sourcePath, destPath, true);
            return destPath;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

