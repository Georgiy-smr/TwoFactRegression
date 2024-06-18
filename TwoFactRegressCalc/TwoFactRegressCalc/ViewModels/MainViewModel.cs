using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Infrastructure.Commands.Base;
using TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Writer;
using TwoFactRegressCalc.ViewModels.Base;

namespace TwoFactRegressCalc.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        public MainViewModel(
            IReadData<DataTwoFact> dataExcelReader, 
            IDialogService dialog, 
            IRegression<DataTwoFact> regression,
            IEnumerable<double[]> writer)
        {
            _dataExcelReader = dataExcelReader;
            _filedialog = dialog;
            _regression = regression;
            _writer = writer;
        }
        private readonly IReadData<DataTwoFact> _dataExcelReader;
        private readonly IDialogService _filedialog;
        private readonly IRegression<DataTwoFact> _regression;
        private readonly IEnumerable<double[]> _writer;
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
                if(await _dataExcelReader.ReadAsync(_filedialog.FilePath, PhysicalValue.Pressure).ToListAsync() is not {Count:>15} dataPressure)
                   return;
                var d = dataPressure.CreateThirdOrderPolynomialExpression();
                var s = _regression.CalcCoefs(d);
                var results = dataPressure.Select(x1x2y => CalcRes(x1x2y, s.ToArray())).ToList();

                if (await _dataExcelReader.ReadAsync(_filedialog.FilePath, PhysicalValue.Temperature).ToListAsync() is not { Count: > 15 } dataTemp)
                    return;


                //if (results.Any())
                //   await _writer.Write(new List<double[]>(){ s.ToArray() }, _filedialog.FilePath);
                //var strbuilder = new StringBuilder();
                //foreach (var coef in s)
                //{
                //    strbuilder.AppendLine(coef.ToString());
                //}
                //var coefsString = strbuilder.ToString();
                //strbuilder.Clear();
            }
        }

        private double CalcRes(DataTwoFact data, double[] resultCheckedCoef)
        {
            double res = 0;
            for (int i = 0; i < resultCheckedCoef.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        res += resultCheckedCoef[i];
                        break;
                    case 1:
                        //a1* item.X2
                        res += resultCheckedCoef[i] * data.X2;
                        break;
                    case 2:
                        //a2* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2;
                        break;
                    case 3:
                        //a3* item.X1 +
                        res += resultCheckedCoef[i] * data.X1;
                        break;
                    case 4:
                        //   a4 * item.X1 * item.X1 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1;
                        break;
                    case 5:
                        //a5 * item.X1 * item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X2;
                        break;
                    case 6:
                        // a6* item.X2* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X1 * data.X1;
                        break;
                    case 7:
                        //a7* item.X2* item.X2* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X1;
                        break;
                    case 8:
                        //a8* item.X1* item.X1* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2 * data.X2;
                        break;
                    case 9:
                        //a9* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1;
                        break;
                    case 10:
                        //a10* item.X2* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                    case 11:
                        //a11* item.X2* item.X2* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                    case 12:
                        //a12* item.X2* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2;
                        break;
                    case 13:
                        //a13* item.X2* item.X2* item.X2* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1;
                        break;
                    case 14:
                        //a14* item.X2* item.X2* item.X2* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1 * data.X1;
                        break;
                    case 15:
                        //a15* item.X2* item.X2* item.X2* item.X1* item.X1* item.X1);
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                }
            }
            return res;
        }
        private bool CanCalcFromExelCommandExecute(object p)
        {
            return true;
        }

 
        #endregion
    }
}
