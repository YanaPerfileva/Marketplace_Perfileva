using Marketplace.DAL.Interfaces;
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
using User.ViewModels;
using static User.AuthWindow;

namespace User
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private readonly MarketplaceContext _context;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
        UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public ProductsPage(IProductRepository repo)
        {
            InitializeComponent();
            this.DataContext = new ProductsViewModel(repo);
            _context = new MarketplaceContext(_dbOptions);
        }


        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is int productId)
            {
                this.NavigationService?.Navigate(new ProductDetailsPage(productId));
            }
        }

        private async void BtnFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.CurrentUser == null)
            {
                MessageBox.Show("Войдите в систему, чтобы добавлять товары в избранное.", "Внимание");
                return;
            }

            var btn = sender as Button;
            int productId = (int)btn.Tag;
            string message = "";

            try
            {
                var existing = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == UserSession.CurrentUser.Id && f.ProductId == productId);

                if (existing == null)
                {
                    _context.Favorites.Add(new Favorite
                    {
                        UserId = UserSession.CurrentUser.Id,
                        ProductId = productId,
                        CreatedAt = DateTime.Now
                    });
                    message = "Товар успешно добавлен в избранное!";
                }
                else
                {
                    _context.Favorites.Remove(existing);
                    message = "Товар удален из списка избранного.";
                }

                await _context.SaveChangesAsync();

                MessageBox.Show(message, "Избранное", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при работе с избранным: {ex.Message}", "Ошибка");
            }
        }
    }
}
