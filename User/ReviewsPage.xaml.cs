using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для RevewsPage.xaml
    /// </summary>
    public partial class RevewsPage : Page
    {
        private readonly MarketplaceContext _context;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
        UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public List<Product> AllProducts { get; set; }
        public ObservableCollection<Review> Reviews { get; set; } = new ObservableCollection<Review>();

        public string FullName => UserSession.CurrentUser?.FullName ?? "Все пользователи";

        public RevewsPage()
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);

            this.Loaded += (s, e) =>
            {
                LoadProducts();
                LoadReviews();
                UpdateControlsState();
            };
            
        }
        private void LoadProducts()
        {
            AllProducts = _context.Products.Where(p => p.IsActive).ToList();
            ProductComboBox.ItemsSource = AllProducts;

        }

        private void LoadReviews()
        {

        var allReviews = _context.Reviews
        .AsNoTracking()
        .Include(r => r.Product)
        .Include(r => r.User)
        .OrderByDescending(r => r.CreatedAt)
        .ToList();

            Reviews.Clear();

            foreach (var review in allReviews)
            {
                Reviews.Add(review);
            }

            this.DataContext = this;
        }

        private void SubmitReview_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.CurrentUser == null)
            {
                MessageBox.Show("Вы должны войти в аккаунт, чтобы оставить отзыв!");
                return;
            }

            if (ProductComboBox.SelectedValue == null)
            {
                MessageBox.Show("Сначала выберите товар!");
                return;
            }

            int selectedProductId = (int)ProductComboBox.SelectedValue;

            var userOrder = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductSku) 
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault(o => o.UserId == UserSession.CurrentUser.Id &&
                                     o.OrderItems.Any(oi => oi.ProductSku != null &&
                                                            oi.ProductSku.ProductId == selectedProductId));
            if (userOrder == null)
            {
                MessageBox.Show("Вы не можете оставить отзыв, так как не покупали этот товар.");
                return;
            }

            try
            {
                var newReview = new Review
                {
                    UserId = UserSession.CurrentUser.Id,
                    ProductId = selectedProductId,
                    OrderId = userOrder.Id,
                    Rating = 5 - RatingCombo.SelectedIndex,
                    Title = ReviewTitleBox.Text,
                    Comment = ReviewCommentBox.Text,
                    IsVerifiedPurchase = true,
                    IsApproved = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Reviews.Add(newReview);
                _context.SaveChanges();

                MessageBox.Show("Отзыв опубликован!");

                ReviewTitleBox.Clear();
                ReviewCommentBox.Clear();
                ProductComboBox.SelectedIndex = -1;

                LoadReviews();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

       public void UpdateControlsState()
        {
            bool isAuth = UserSession.CurrentUser != null;

            ProductComboBox.IsEnabled = isAuth;
            RatingCombo.IsEnabled = isAuth;
            ReviewTitleBox.IsEnabled = isAuth;
            ReviewCommentBox.IsEnabled = isAuth;
           
            Publish.IsEnabled = isAuth;

            if (!isAuth)
            {
                ReviewCommentBox.Text = "Пожалуйста, войдите в систему, чтобы оставить отзыв.";
            }
        }

    }
}
