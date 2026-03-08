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
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private readonly MarketplaceContext _context;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
        UseSqlServer(@"Server=.\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public OrdersPage()
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
        }

        private  async void DGridUser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DGridUser.SelectedItem is Marketplace.Data.Entities.Order selectedOrder)
            {
                var orderWithDetails = await _context.Orders
            .Include(o => o.OrderItems)                
                .ThenInclude(oi => oi.ProductSku)      
                    .ThenInclude(ps => ps.Product)      
            .FirstOrDefaultAsync(o => o.Id == selectedOrder.Id);

                if (orderWithDetails != null)
                {
                    var detailsPage = new OrderDetailsPage(orderWithDetails);
                    this.NavigationService.Navigate(detailsPage);
                }
            }
        }

    }
}
