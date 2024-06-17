using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using TwoFactRegressCalc.Infrastructure.DI;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() => _serviceProvider = InitializeServices(new ServiceCollection()).BuildServiceProvider();
        private readonly IServiceProvider _serviceProvider;

        private IServiceCollection InitializeServices(ServiceCollection services)
        {
            services.ExcelReader();
            services.FileDialog();
            services.MainWindowAndViewModel();
            services.Regression();
            return services;
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using var scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<MainWindow>().Show();
        }


    }

}
