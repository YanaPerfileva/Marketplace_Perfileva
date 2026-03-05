using Marketplace.BLL.Extensions;
using Marketplace.DAL.Extensions;
using Marketplace.Data.Context;
using Marketplace.Wpf.ViewModels;
using Marketplace.Wpf.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;



namespace Marketplace.Wpf
{
    public partial class App : Application
    {
        public static ServiceProvider Services { get; private set; } = null!;
        private IServiceScope? _appScope;

        protected override void OnStartup(StartupEventArgs e)
        {
            var configuration = BuildConfiguration();
            var connectionString = configuration.GetConnectionString("MarketplaceDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'MarketplaceDb' is not configured. " +
                    "Set it in appsettings.json or via ConnectionStrings__MarketplaceDb environment variable.");
            }

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddDbContext<MarketplaceContext>(options =>
                options.UseMarketplaceSqlServer(connectionString));

            serviceCollection.AddDataAccessLayer();
            serviceCollection.AddBusinessLogicLayer();
            serviceCollection.AddScoped<MainViewModel>();
            serviceCollection.AddScoped<MainWindow>();

            Services = serviceCollection.BuildServiceProvider();

            _appScope = Services.CreateScope();
            var window = _appScope.ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _appScope?.Dispose();
            Services?.Dispose();
            base.OnExit(e);
        }

        private static IConfiguration BuildConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                              ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                              ?? "Production";

            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
