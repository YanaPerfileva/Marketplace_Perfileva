using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
    /// Логика взаимодействия для CartsPage.xaml
    /// </summary>
    public partial class CartsPage : Page
    {
        private readonly MarketplaceContext _context;
        private static readonly DbContextOptions<MarketplaceContext> _dbOptions = new DbContextOptionsBuilder<MarketplaceContext>().
        UseSqlServer(@"Server =.\SQLEXPRESS; Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;").Options;

        public CartsPage()
        {
            InitializeComponent();
            _context = new MarketplaceContext(_dbOptions);
            LoadCart();
        }

        private async void LoadCart()
        {
            if (UserSession.CurrentUser == null) return;

            var items = await _context.CartItems
                .Include(ci => ci.ProductSku)
                .ThenInclude(s => s.Product)
                .Where(ci => ci.Cart.UserId == UserSession.CurrentUser.Id && ci.Cart.IsActive)
                .ToListAsync();

            var displayData = items.Select(ci => new
            {
                ci.Id,
                ci.ProductSku,
                ci.Quantity,
                TotalPrice = ci.Quantity * ci.ProductSku.Price
            }).ToList();

            CartGrid.ItemsSource = displayData;

            decimal totalSum = items.Sum(ci => ci.Quantity * ci.ProductSku.Price);
            TotalOrderSum.Text = $"{totalSum:N0} ₽";
        }


        private async void DeleteCartItem_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = ((Button)sender).DataContext;

            int itemId = selected.Id;

            var itemInDb = await _context.CartItems
                .Include(ci => ci.ProductSku)
                .FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (itemInDb != null)
            {
                itemInDb.ProductSku.Stock += itemInDb.Quantity;

                _context.CartItems.Remove(itemInDb);
                await _context.SaveChangesAsync();

                LoadCart();
            }
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.CurrentUser == null) return;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductSku)
                        .ThenInclude(ps => ps.Product)
                            .ThenInclude(p => p.PromotionProducts)
                                .ThenInclude(pp => pp.Promotion)
                .FirstOrDefaultAsync(c => c.UserId == UserSession.CurrentUser.Id && c.IsActive);

            if (cart == null || cart.CartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            try
            {
                decimal totalAmount = 0;
                decimal totalDiscount = 0;

                foreach (var item in cart.CartItems)
                {
                    if (item.ProductSku?.Stock < item.Quantity)
                    {
                        MessageBox.Show($"Недостаточно товара: {item.ProductSku?.Product?.Name}. В наличии: {item.ProductSku?.Stock}");
                        return;
                    }

                    decimal itemBasePrice = item.ProductSku.Price * item.Quantity;
                    totalAmount += itemBasePrice;

                    var activePromo = item.ProductSku?.Product?.PromotionProducts
                        .Select(pp => pp.Promotion)
                        .FirstOrDefault(p => p.IsActive
                            && DateTime.Now >= p.StartDate
                            && DateTime.Now <= p.EndDate);

                    if (activePromo != null)
                    {
                        decimal itemDiscount = 0;
                        if (activePromo.DiscountType == Marketplace.Data.Enums.DiscountType.percent)
                            itemDiscount = itemBasePrice * (activePromo.DiscountValue / 100);
                        else if (activePromo.DiscountType == Marketplace.Data.Enums.DiscountType.@fixed)
                            itemDiscount = Math.Min(itemBasePrice, activePromo.DiscountValue * item.Quantity);

                        totalDiscount += itemDiscount;
                    }

                    item.ProductSku?.Stock -= item.Quantity;
                }

                decimal finalAmount = totalAmount - totalDiscount;

                var newOrder = new Order
                {
                    UserId = UserSession.CurrentUser.Id,
                    SellerId = cart.CartItems.First().ProductSku!.Product!.SellerId,
                    OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                    Status = Marketplace.Data.Enums.OrderStatus.processing,
                    TotalPrice = totalAmount,
                    DiscountAmount = totalDiscount,
                    FinalPrice = finalAmount,
                    Comment = "Заказ со скидками",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    OrderItems = [.. cart.CartItems.Select(item => new OrderItem
                    {
                        SkuId = item.ProductSku.Id,          
                        Quantity = item.Quantity,
                        PriceAtTime = item.ProductSku.Price, 
                        ProductSku = item.ProductSku
                    })]
                };

                _context.Orders.Add(newOrder);
                cart.IsActive = false;
                cart.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                string discountMsg = totalDiscount > 0 ? $"\nВаша скидка: {totalDiscount:N0} ₽" : "";
                MessageBox.Show($"Заказ №{newOrder.OrderNumber} успешно оформлен!{discountMsg}", "Успех");

                CartGrid.ItemsSource = null;
                TotalOrderSum.Text = "0 ₽";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении: {ex.Message}");
            }
        }
    }

}
