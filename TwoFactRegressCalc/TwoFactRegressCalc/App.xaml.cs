using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
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


        public static string SettingsPath =>
            $"{Environment.CurrentDirectory}\\settings.json";
        public static string FileKeepDefaultPath => "Q:\\АПМ\\Характеризация\\";
        public static string? CurrentDirectory =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private IServiceCollection InitializeServices(ServiceCollection services)
        {
            services.ExcelReader();
            services.FileDialog();
            services.MainWindowAndViewModel();
            services.Regression();
            services.FilledExcelDoc();
            services.FileCreator();
            services.JsonFileService();
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
