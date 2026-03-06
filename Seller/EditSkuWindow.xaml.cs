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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Seller
{
    /// <summary>
    /// Логика взаимодействия для EditSkuWindow.xaml
    /// </summary>
    public partial class EditSkuWindow : Window
    {
        private readonly MarketplaceContext _context;
        private readonly int _sellerId;
        private readonly int _productId;
        private List<ProductSku> _skus = new();
        private ProductSku? _editingSku;

        private static readonly DbContextOptions<MarketplaceContext> _dbOptions =
            new DbContextOptionsBuilder<MarketplaceContext>()
            .UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;")
            .Options;

        public EditSkuWindow(int sellerId, int productId)
        {
            InitializeComponent();
            _sellerId = sellerId;
            _productId = productId;
            _context = new MarketplaceContext(_dbOptions);
            _ = LoadSkusAsync();
        }

        private async Task LoadSkusAsync()
        {
            try
            {
                var skus = await _context.ProductSkus
                    .Where(s => s.ProductId == _productId)
                    .OrderBy(s => s.Size)
                    .ThenBy(s => s.Color)
                    .ToListAsync();

                _skus = skus;
                SkusDataGrid.ItemsSource = _skus;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки SKU: {ex.Message}");
            }
        }

        private async void AddSkuButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            SaveButton.Content = "Добавить SKU";
            _editingSku = null;
        }

        private async void DeleteSkuButton_Click(object sender, RoutedEventArgs e)
        {
            if (SkusDataGrid.SelectedItem is ProductSku sku)
            {
                if (MessageBox.Show("Удалить SKU?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.ProductSkus.Remove(sku);
                    await _context.SaveChangesAsync();
                    await LoadSkusAsync();
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Сохранение...";

                decimal skuPrice = decimal.Parse(SkuPriceTextBox.Text);
                int skuStock = int.Parse(SkuStockTextBox.Text);

                if (_editingSku == null) 
                {
                    
                    var sql = @"
                INSERT INTO ProductSkus (ProductId, Size, Color, ColorHex, Price, Stock) 
                VALUES (@productId, @size, @color, @colorHex, @price, @stock)";

                    await _context.Database.ExecuteSqlRawAsync(sql,
                        new SqlParameter("@productId", _productId),
                        new SqlParameter("@size", SkuSizeTextBox.Text.Trim()),
                        new SqlParameter("@color", SkuColorTextBox.Text.Trim()),
                        new SqlParameter("@colorHex", SkuColorHexTextBox.Text.Trim()),
                        new SqlParameter("@price", skuPrice),
                        new SqlParameter("@stock", skuStock));
                }
                else 
                {
                    //прямой апдейт потому триггеры в БД на таблице ProductSkus, а EF Core 7+ не может обновлять таблицы с триггерами.
                    var sql = @"
                UPDATE ProductSkus 
                SET Size = @size, Color = @color, ColorHex = @colorHex, 
                    Price = @price, Stock = @stock
                WHERE Id = @id";

                    await _context.Database.ExecuteSqlRawAsync(sql,
                        new SqlParameter("@id", _editingSku.Id),
                        new SqlParameter("@size", SkuSizeTextBox.Text.Trim()),
                        new SqlParameter("@color", SkuColorTextBox.Text.Trim()),
                        new SqlParameter("@colorHex", SkuColorHexTextBox.Text.Trim()),
                        new SqlParameter("@price", skuPrice),
                        new SqlParameter("@stock", skuStock));
                }

                await LoadSkusAsync();
                ClearForm();
                SaveButton.Content = "Сохранить SKU";
                MessageBox.Show("SKU сохранён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
            finally
            {
                SaveButton.IsEnabled = true;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(SkuSizeTextBox.Text))
            {
                MessageBox.Show("Размер SKU!");
                SkuSizeTextBox.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(SkuColorTextBox.Text))
            {
                MessageBox.Show("Цвет SKU!");
                SkuColorTextBox.Focus();
                return false;
            }
            if (!decimal.TryParse(SkuPriceTextBox.Text, out _) || decimal.Parse(SkuPriceTextBox.Text) <= 0)
            {
                MessageBox.Show("Цена SKU > 0!");
                SkuPriceTextBox.Focus();
                return false;
            }
            if (!int.TryParse(SkuStockTextBox.Text, out int stock) || stock <= 0)
            {
                MessageBox.Show("Остаток > 0!");
                SkuStockTextBox.Focus();
                return false;
            }
            return true;
        }

        private void SkusDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SkusDataGrid.SelectedItem is ProductSku sku)
            {
                SkuSizeTextBox.Text = sku.Size;
                SkuColorTextBox.Text = sku.Color;
                SkuColorHexTextBox.Text = sku.ColorHex ?? "#";
                SkuPriceTextBox.Text = sku.Price.ToString("F2");
                SkuStockTextBox.Text = sku.Stock.ToString();
                SaveButton.Content = "Обновить SKU";
                _editingSku = sku;
            }
        }
        private void ClearForm()
        {
            SkuSizeTextBox.Text = "";
            SkuColorTextBox.Text = "";
            SkuColorHexTextBox.Text = "#";
            SkuPriceTextBox.Text = "";
            SkuStockTextBox.Text = "";
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
