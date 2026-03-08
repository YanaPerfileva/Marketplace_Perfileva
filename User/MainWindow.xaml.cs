using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Repositories;
using Marketplace.Data.Context;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private readonly MarketplaceContext _context;
        private readonly UserRepository _userRepo;
        private readonly ProductRepository _productRepo;
      
        public ObservableCollection<Seller> AllSellers { get; set; } = [];

        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
        UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public MainWindow()
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
            _userRepo = new UserRepository(_context);
            _productRepo = new ProductRepository(_context);
            LoadAllSellers();
        }
        #region StatusBar
        public static readonly DependencyProperty StatusMessageProperty = DependencyProperty.Register("StatusMessage", typeof(string), typeof(MainWindow), new PropertyMetadata("Готов"));

        public string StatusMessage
        {
            get { return (string)GetValue(StatusMessageProperty); }
            set { SetValue(StatusMessageProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        #endregion

        private async void LoadAllSellers()
        {

            IsLoading = true;

            StatusMessage = "Загрузка магазинов...";

            var sellers = await _userRepo.GetAllSellersAsync();

            AllSellers.Clear();

            foreach (var s in sellers) AllSellers.Add(s);

            SellersList.ItemsSource = AllSellers;

            StatusMessage = $"Найдено магазинов: {AllSellers.Count}";
        }

        private async void LoadProducts_Click(object sender, RoutedEventArgs e)
        {
            IsLoading = true;

            StatusMessage = "Загрузка каталога...";

            try
            {
                var productPage = new ProductsPage(_productRepo);

                MainFrame.Navigate(productPage);

                StatusMessage = "Каталог загружен";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }


        private async void LoadOrders_Click(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            StatusMessage = "Загрузка данных...";

            try
            {
                if (UserSession.CurrentUser == null)
                {
                    var ordersPage = new OrdersPage
                    {
                    };
                    MainFrame.Navigate(ordersPage);
                    StatusMessage = "Данные не найдены(авторизируйтесь)";
                }
                else
                {

                    var orders = await _userRepo.GetUserWithOrdersAsync(UserSession.CurrentUser.Id);

                    if (orders != null)
                    {
                        var ordersPage = new OrdersPage
                        {
                            DataContext = orders
                        };
                        MainFrame.Navigate(ordersPage);

                        int count = orders.Orders?.Count ?? 0;

                        if (count > 0)
                        {
                            StatusMessage = $"Загружено {count} заказов пользователя {orders.FullName}";
                        }
                        else
                        {
                            StatusMessage = $"У пользователя пока нет заказов";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка БД: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadReviews_Click(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            StatusMessage = "Загрузка общей ленты отзывов...";

            try
            {
                var reviewsPage = new RevewsPage();

                using (var db = new MarketplaceContext(_dbOptions))
                {
                    int totalCount = await db.Reviews.CountAsync();

                    MainFrame.Navigate(reviewsPage);

                    StatusMessage = totalCount > 0
                        ? $"Отображено {totalCount} отзывов от всех покупателей"
                        : "В системе пока нет ни одного отзыва";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при загрузке ленты: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadFavorites_Click(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            StatusMessage = "Загрузка данных...";

            try
            {
                if (UserSession.CurrentUser == null)
                {
                    var favoritesPage = new FavoritesPage
                    {
                    };
                    MainFrame.Navigate(favoritesPage);
                    StatusMessage = "Данные не найдены(авторизируйтесь)";
                }
                else
                { 

                var userWithFavorites = await _userRepo.GetUserWithFavoritesAsync(UserSession.CurrentUser.Id);

                        var favoritesPage = new FavoritesPage
                        {
                            DataContext = userWithFavorites
                        };
                        MainFrame.Navigate(favoritesPage);

                        int count = userWithFavorites?.Favorites?.Count ?? 0;

                        if (count > 0)
                        {
                            StatusMessage = $"В избранном у {userWithFavorites?.FullName}: {count} шт.";
                        }
                        else
                        {
                            StatusMessage = $"Список избранного пуст";
                        }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка БД: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadCarts_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.CurrentUser == null)
            {
                var carPage = new CartsPage
                {
                };
                MainFrame.Navigate(carPage);
                StatusMessage = "Данные не найдены(авторизируйтесь)";
            }
            else
            {
                try
                {
                    var userWithCart = await _userRepo.GetUserWithCartAsync(UserSession.CurrentUser.Id);

                    
                        var activeCart = userWithCart?.Carts.FirstOrDefault();

                        if (activeCart != null && activeCart.CartItems?.Any() == true)
                        {
                            var cartPage = new CartsPage
                            {
                                DataContext = activeCart
                            };
                            MainFrame.Navigate(cartPage);

                            int itemCount = activeCart.CartItems.Count;

                            decimal totalSum = activeCart.CartItems.Sum(ci => ci.Total);

                            StatusMessage = $"Корзина {userWithCart?.FullName}: {itemCount} поз. на сумму {totalSum:N2} руб.";
                        }
                        else
                        {
                            StatusMessage = $"Корзина пользователя {userWithCart?.FullName} пуста";
                            MainFrame.Content = null;
                        }
                }

                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка загрузки корзины: {ex.Message}";
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.CurrentUser != null)
            {
                if (MessageBox.Show("Выйти из аккаунта?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    UserSession.CurrentUser = null;
                    AuthButton.Content = "Войти"; 
                    StatusMessage = "Вы вышли из системы";
                    if (MainFrame.Content is RevewsPage page) page.UpdateControlsState();
                }
                return;
            }

            AuthWindow authWin = new AuthWindow();
            if (authWin.ShowDialog() == true) 
            {
                AuthButton.Content = $"Выйти ({UserSession.CurrentUser?.FullName})";
                StatusMessage = $"Добро пожаловать, {UserSession.CurrentUser?.FullName}!";

                if (MainFrame.Content is RevewsPage page)
                {
                    page.UpdateControlsState();
                }
            }
        }
    }
}
