using Marketplace.Data.Context;
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

namespace User
{
    /// <summary>
    /// Логика взаимодействия для OrderDetailsPage.xaml
    /// </summary>
    public partial class OrderDetailsPage : Page
    {
        private readonly MarketplaceContext _context;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
       UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public OrderDetailsPage(Marketplace.Data.Entities.Order order)
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
            LoadOrderData(order.Id);
        }

        private async void LoadOrderData(int orderId)
        {
            var fullOrder = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductSku)
                        .ThenInclude(ps => ps.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (fullOrder != null)
            {
                this.DataContext = fullOrder;
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack) this.NavigationService.GoBack();
        }
    }
}
