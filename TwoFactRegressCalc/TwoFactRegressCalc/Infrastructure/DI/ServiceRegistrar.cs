using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFactRegressThidOrder;
using TwoFactRegressCalc.Infrastructure.DI.Services.Writer;
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

        internal static IServiceCollection ThridTwoFactRegress(this ServiceCollection service) =>
            service.AddTransient<IRegression<DataTwoFact>, RegressionThidOrderPolynomial>();

        internal static IServiceCollection Regression(this ServiceCollection service) =>
            service.AddTransient<IRegression<DataTwoFact>, RegressionThidOrderPolynomial>();

        internal static IServiceCollection FilledExcelDoc(this ServiceCollection service) =>
            service.AddTransient<IWriteData<double[]>, ExcelFillData>();
    }
     
}

