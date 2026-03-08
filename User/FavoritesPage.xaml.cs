using Marketplace.DAL.Repositories;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для FavoritesPage.xaml
    /// </summary>
    public partial class FavoritesPage : Page
    {
        private readonly MarketplaceContext _context;
        private readonly FavoriteRepository _favRepo;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
        UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public FavoritesPage()
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
            _favRepo = new FavoriteRepository(_context);
            this.DataContext = _favRepo;
            LoadFavorites();
        }

        private async void LoadFavorites()
        {
            if (UserSession.CurrentUser == null) return;
            var userFavorites = await _context.Favorites
                .Include(f => f.Product)
                .Where(f => f.UserId == UserSession.CurrentUser.Id)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            this.DataContext = new { Favorites = userFavorites };
        }

        private async void DeleteFavorite_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.CommandParameter is int favoriteId)
            {
                var result = MessageBox.Show("Удалить этот товар из избранного?", "Подтверждение",MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var fav = await _context.Favorites.FindAsync(favoriteId);

                        if (fav != null)
                        {
                            _context.Favorites.Remove(fav);
                            await _context.SaveChangesAsync();

                            LoadFavorites();

                            MessageBox.Show("Товар удален из избранного.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
        }

        private  async void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.CurrentUser == null) return;

            var button = sender as Button;
            if (button?.Tag is int productId)
            {
                try
                {
                    var cart = await _context.Carts
                        .FirstOrDefaultAsync(c => c.UserId == UserSession.CurrentUser.Id && c.IsActive);

                    if (cart == null)
                    {
                        cart = new Cart { UserId = UserSession.CurrentUser.Id, IsActive = true, CreatedAt = DateTime.Now };
                        _context.Carts.Add(cart);
                        await _context.SaveChangesAsync();
                    }
                    var sku = await _context.ProductSkus
                        .FirstOrDefaultAsync(s => s.ProductId == productId && s.Stock > 0);

                    if (sku == null)
                    {
                        MessageBox.Show("Извините, этого товара сейчас нет в наличии (нет доступных размеров).", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var existingItem = await _context.CartItems
                        .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.SkuId == sku.Id);

                    if (existingItem != null)
                    {
                        existingItem.Quantity++;
                    }
                    else
                    {
                        _context.CartItems.Add(new CartItem
                        {
                            CartId = cart.Id,
                            SkuId = sku.Id,
                            Quantity = 1,
                            AddedAt = DateTime.Now
                        });
                    }

                    await _context.SaveChangesAsync();
                    MessageBox.Show("Товар добавлен в корзину!", "Успех",MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
                }
            }
        }
    }
}
    

