using Marketplace.Wpf.ViewModels;
using System.Windows;

namespace Marketplace.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
