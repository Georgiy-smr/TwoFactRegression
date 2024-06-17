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
using MathNet.Symbolics;

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
                var s = _regression.CalcCoefs(d).ToArray();

                double inputX1 = data[1].X1;
                double inputX2 = data[1].X2;
                double outPut = data[1].Y;
                double res = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            res = +s[i];
                            break;
                        case 1:
                            res = +s[i] * inputX2;
                            break;
                        case 2:
                            res = +s[i] * inputX2 * inputX2;
                            break;
                        case 3:
                            res = +s[i] * inputX1;
                            break;
                        case 4:
                            res = +s[i] * inputX1 * inputX1;
                            break;
                        case 5:
                            res = +s[i] * inputX1 * inputX2;
                            break;
                        case 6:
                            res = +s[i] * inputX2 * inputX1 * inputX1;
                            break;
                        case 7:
                            res = +s[i] * inputX2 * inputX2 * inputX1;
                            break;
                        case 8:
                            res = +s[i] * inputX1 * inputX1 * inputX2 * inputX2;
                            break;
                        case 9:
                            res = +s[i] * inputX1 * inputX1 * inputX1;
                            break;
                        case 10:
                            res = +s[i] * inputX2 * inputX1 * inputX1 * inputX1;
                            break;
                        case 11:
                            res = +s[i] * inputX2 * inputX2 * inputX1 * inputX1 * inputX1;
                            break;
                        case 12:
                            res = +s[i] * inputX2 * inputX2 * inputX2;
                            break;
                        case 13:
                            res = +s[i] * inputX2 * inputX2 * inputX2 * inputX1;
                            break;
                        case 14:
                            res = +s[i] * inputX2 * inputX2 * inputX2 * inputX1 * inputX1;
                            break;
                        case 15:
                            res = +s[i] * inputX2 * inputX2 * inputX2 * inputX1 * inputX1 * inputX1;
                            break;

                    }
                }
            }
        }

        private bool CanCalcFromExelCommandExecute(object p)
        {
            return true;
        }

 
        #endregion
    }
}
