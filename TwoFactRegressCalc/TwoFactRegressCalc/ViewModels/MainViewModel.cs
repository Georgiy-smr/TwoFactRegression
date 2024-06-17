using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Infrastructure.Commands.Base;
using TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.ViewModels.Base;

namespace TwoFactRegressCalc.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        public MainViewModel(IReadData<DataTwoFact> dataExcelReader, IDialogService dialog, IRegression<DataTwoFact> regression)
        {
            _dataExcelReader = dataExcelReader;
            _filedialog = dialog;
            _regression = regression;
        }
        private readonly IReadData<DataTwoFact> _dataExcelReader;
        private readonly IDialogService _filedialog;
        private readonly IRegression<DataTwoFact> _regression;


        /// <summary>
        /// summary
        /// </summary>
        private string _title = "Проверка";

        /// <summary>
        /// summary
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        #region CalcFromExel Расчет из файла эксель

        private ICommand? _сalcFromExelCommand;


        public ICommand СalcFromExelCommand =>
            _сalcFromExelCommand ?? new LambdaCommandAsync(OnCalcFromExelCommandExecuted, CanCalcFromExelCommandExecute);

        private async Task OnCalcFromExelCommandExecuted(object arg)
        {
            _filedialog.Filter = "Excel workbooks (*.xlsx)|*.xlsx";
            if (_filedialog.OpenFileDialog())
            {
                if(await _dataExcelReader.ReadAsync(_filedialog.FilePath).ToListAsync() is not {Count:>15} data)
                   return;
                var d = data.CreateThirdOrderPolynomialExpression();
                var s = _regression.CalcCoefs(d);
            }
        }

        private bool CanCalcFromExelCommandExecute(object p)
        {
            return true;
        }

 
        #endregion
    }
}
