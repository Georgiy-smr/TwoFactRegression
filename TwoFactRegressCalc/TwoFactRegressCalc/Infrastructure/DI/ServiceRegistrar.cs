using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Regression.OutlierDetection;
using TwoFactRegressCalc.Infrastructure.DI.Services.Creator;
using TwoFactRegressCalc.Infrastructure.DI.Services.DatasetReview;
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
                .AddSingleton<IReadData<CalibrationPoint>, ExcelFileDataReader>();

        internal static IServiceCollection FileDialog(this ServiceCollection service) =>
            service.AddTransient<IDialogService, FileDialogService>();


        // Runs as: dataset review -> pressure fit and pick -> temperature fit and pick.
        // Scrutor wraps the registration, so the last Decorate call is the outermost step.
        internal static IServiceCollection Regression(this ServiceCollection service) =>
            service
                .AddSingleton<RegressionCandidateCalculator>()
                .AddTransient<IRegressionResultPicker, RegressionResultPickerService>()
                .AddTransient<IRegressionCalculator, RegressionChainEnd>()
                .Decorate<IRegressionCalculator>((next, provider) => PickStep(provider, PhysicalValue.Temperature, next))
                .Decorate<IRegressionCalculator>((next, provider) => PickStep(provider, PhysicalValue.Pressure, next))
                .Decorate<IRegressionCalculator, DatasetReviewStep>();

        private static RegressionPickStep PickStep(
            IServiceProvider provider, PhysicalValue physicalValue, IRegressionCalculator next) =>
            new(physicalValue,
                provider.GetRequiredService<RegressionCandidateCalculator>(),
                provider.GetRequiredService<IRegressionResultPicker>(),
                next);


        internal static IServiceCollection FilledExcelDoc(this ServiceCollection service) =>
            service.AddTransient<IWriteData<AllSensorCoefficients>, ExcelFillPressureAndTempData>();

        internal static IServiceCollection FileCreator(this ServiceCollection service) =>
            service.AddTransient<ICreate<CoefficientsBySensor>, CreateFileWithCoefficients>();
        internal static IServiceCollection JsonFileService(this ServiceCollection service) =>
            service
                .AddTransient<IJsonFileService<Config>, SettingsJsonFileService>()
                // One shared instance: MainViewModel loads and edits it, the dataset review reads it.
                .AddSingleton<Config>();

    }
     
}

