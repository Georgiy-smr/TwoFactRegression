using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Creator;
using TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog;
using TwoFactRegressCalc.Infrastructure.DI.Services.JsonFileService;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFact;
using TwoFactRegressCalc.Infrastructure.DI.Services.Writer;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc.Infrastructure.DI
{
    internal static class ServiceRegistrar
    {
        internal static IServiceCollection MainWindowAndViewModel(this ServiceCollection services)
            => services
                .AddSingleton<MainViewModel>()
                .AddSingleton(provider =>
                {
                    var vm = provider.GetRequiredService<MainViewModel>();
                    return new MainWindow() { DataContext = vm };
                });

        internal static IServiceCollection ExcelReader(this ServiceCollection services)
            => services
                .AddSingleton<IReadData<DataTwoFact>, ExcelFileDataReader>();

        internal static IServiceCollection FileDialog(this ServiceCollection service) =>
            service.AddTransient<IDialogService, FileDialogService>();


        internal static IServiceCollection Regression(this ServiceCollection service) =>
            service
                .AddTransient<IRegressionCalculator, RegressionCalculator>()
                .AddTransient<IRegressionResultPicker, RegressionResultPickerService>();


        internal static IServiceCollection FilledExcelDoc(this ServiceCollection service) =>
            service.AddTransient<IWriteData<AllSensorCoefficients>, ExcelFillPressureAndTempData>();

        internal static IServiceCollection FileCreator(this ServiceCollection service) =>
            service.AddTransient<ICreate<CoefficientsBySensor>, CreateFileWithCoefficients>();
        internal static IServiceCollection JsonFileService(this ServiceCollection service) =>
            service.AddTransient<IJsonFileService<Config>, SettingsJsonFileService>();

    }
     
}

