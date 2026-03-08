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
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MarketplaceContext _context;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions =
            new DbContextOptionsBuilder<MarketplaceContext>()
            .UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;")
            .Options;

        public static class UserSession
        {
            public static User? CurrentUser { get; set; }
        }

        public MainWindow()
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

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _context.Users
                .Include(u => u.Seller) // Загружаем связанного продавца
                .FirstOrDefault(u => u.Email == email && u.PasswordHash == password && u.IsActive);

            if (user != null)
            {
                UserSession.CurrentUser = user;
                user.LastLogin = DateTime.Now;
                _context.SaveChanges();

                MessageBox.Show($"Добро пожаловать, {user.FullName}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Проверяем роль и открываем нужное окно
                if (user.Role == UserRole.seller && user.Seller != null)
                {
                    // Продавец с профилем -> дашборд продавца
                    var sellerDashboard = new Seller.MainWindow(user.Seller.Id);
                   // sellerDashboard.Show();
                }
                else
                {
                    
                   
                        UserSession.CurrentUser = user;
                        this.DialogResult = true;
                        MessageBox.Show("Вход выполнен успешно!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.Close();
                 

                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный email или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(RegName.Text))
            {
                MessageBox.Show("Введите ваше ФИО!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Проверка телефона
            string cleanPhone = new string(RegPhone.Text.Where(char.IsDigit).ToArray());
            if (cleanPhone.Length < 11)
            {
                MessageBox.Show("Номер телефона должен содержать минимум 11 цифр!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(RegEmail.Text) || !RegEmail.Text.Contains('@'))
            {
                MessageBox.Show("Введите корректный Email!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Проверка пароля
            if (string.IsNullOrWhiteSpace(RegPass.Password) || RegPass.Password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (!RegPass.Password.Any(char.IsLetter))
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну букву!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (!RegPass.Password.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Проверка уникальности email
            if (_context.Users.Any(u => u.Email == RegEmail.Text))
            {
                MessageBox.Show("Email занят!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Получаем выбранную роль из ComboBox
            var selectedRoleItem = RoleComboBox.SelectedItem as ComboBoxItem;
            var selectedRole = selectedRoleItem?.Tag?.ToString();
            var userRole = selectedRole == "seller" ? UserRole.seller : UserRole.buyer;

            var newUser = new User
            {
                FullName = RegName.Text,
                Phone = RegPhone.Text,
                Email = RegEmail.Text,
                PasswordHash = RegPass.Password,
                Role = userRole,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            UserSession.CurrentUser = newUser;

            MessageBox.Show($"Аккаунт {userRole} создан успешно! Теперь можете войти.",
                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            TogglePanels_Click(null, null);
        }

        private void RegPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
