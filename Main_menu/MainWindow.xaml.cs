using System.Diagnostics;
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
using Marketplace.Data.Entities;
using Marketplace.Wpf.Infrastructure;
using static Marketplace.Data.Entities.Seller;
using static Marketplace.Data.Entities.User;

namespace Main_menu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void SellerButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"D:\TOP_Computer_Academy\Marketplace_Perfileva\Seller\bin\Debug\net8.0-windows\Seller.exe");
            this.Close();
        }

        private void BuyerButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"D:\TOP_Computer_Academy\Marketplace_Perfileva\User\bin\Debug\net8.0-windows\User.exe");
            this.Close();
        }
    }
}