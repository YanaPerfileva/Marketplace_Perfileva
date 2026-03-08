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
using Microsoft.EntityFrameworkCore;

namespace Seller
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private readonly MarketplaceContext _context;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
        UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public static class UserSession
        {
            public static Marketplace.Data.Entities.User? CurrentUser { get; set; }
        }

        public AuthWindow()
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
        }
        private void TogglePanels_Click(object? sender, RoutedEventArgs? e)
        {
            bool isLoginVisible = LoginPanel.Visibility == Visibility.Visible;
            LoginPanel.Visibility = isLoginVisible ? Visibility.Collapsed : Visibility.Visible;
            RegisterPanel.Visibility = isLoginVisible ? Visibility.Visible : Visibility.Collapsed;
        }

      
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = LoginEmail.Text.Trim();
            string password = LoginPass.Password;

            var user = _context.Users
                .Include(u => u.Seller) 
                .FirstOrDefault(u => u.Email == email && u.PasswordHash == password && u.IsActive);

            if (user != null && user.Role == Marketplace.Data.Enums.UserRole.seller && user.Seller != null)
            {
                UserSession.CurrentUser = user;

                var sellerDashboard = new MainWindow(user.Seller.Id);
                sellerDashboard.Show();

                MessageBox.Show($"Добро пожаловать в дашборд, {user.FullName}!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Доступ только для продавцов с профилем!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegName.Text))
            {
                MessageBox.Show("Введите ваше ФИО!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            string cleanPhone = new string(RegPhone.Text.Where(char.IsDigit).ToArray());
            if (cleanPhone.Length < 11)
            {
                MessageBox.Show("Номер телефона должен содержать минимум 11 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (string.IsNullOrWhiteSpace(RegEmail.Text) || !RegEmail.Text.Contains('@'))
            {
                MessageBox.Show("Введите корректный Email!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (string.IsNullOrWhiteSpace(RegPass.Password) || RegPass.Password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (!RegPass.Password.Any(char.IsLetter))
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну букву!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (!RegPass.Password.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (_context.Users.Any(u => u.Email == RegEmail.Text))
            {
                MessageBox.Show("Email занят!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            var newUser = new Marketplace.Data.Entities.User
            {
                FullName = RegName.Text,
                Phone = RegPhone.Text,
                Email = RegEmail.Text,
                PasswordHash = RegPass.Password,
                Role = Marketplace.Data.Enums.UserRole.seller,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            UserSession.CurrentUser = newUser;
            MessageBox.Show("Успех! Теперь можете войти!", "Вход", MessageBoxButton.OK, MessageBoxImage.Information);
            TogglePanels_Click(null, null);
        }

        private void RegPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}

