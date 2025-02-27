using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Extansions.TwoFactExpression;
using TwoFactRegressCalc.Infrastructure.Commands.Base;
using TwoFactRegressCalc.Infrastructure.DI.Services.Creator;
using TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Writer;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels.Base;
using Microsoft.WindowsAPICodePack.Dialogs;
using TwoFactRegressCalc.Infrastructure.DI.Services.JsonFileService;

namespace TwoFactRegressCalc.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        public MainViewModel(
            IReadData<DataTwoFact> dataExcelReader, 
            IDialogService dialog, 
            IRegression<DataTwoFact> regression,
            IWriteData<IEnumerable<double[]>> writer,
            ICreate<Coefficients> fileCreator,
            IJsonFileService<Config> configService)
        {
            _dataExcelReader = dataExcelReader;
            _filedialog = dialog;
            _regression = regression;
            _writer = writer;
            _fileCreator = fileCreator;
            _configService = configService;
        }
        private readonly IReadData<DataTwoFact> _dataExcelReader;
        private readonly IDialogService _filedialog;
        private readonly IRegression<DataTwoFact> _regression;
        private readonly IWriteData<IEnumerable<double[]>> _writer;
        private readonly ICreate<Coefficients> _fileCreator;
        private readonly IJsonFileService<Config> _configService;
        private Config _config;
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


        public ICommand СalcFromExсelCommand =>
            _сalcFromExelCommand ?? new LambdaCommandAsync(OnCalcFromExelCommandExecuted, CanCalcFromExelCommandExecute);

        private async Task OnCalcFromExelCommandExecuted(object arg)
        {
            _filedialog.Filter = "Excel workbooks (*.xlsx)|*.xlsx";
            if (!_filedialog.OpenFileDialog()) 
                return;
            
            string combine = Path.Combine(
                FilePath, SerialText ?? Path.GetFileNameWithoutExtension(_filedialog.FilePath));

            if (ShowUserDialogIfFilePathIsExists(combine))
                return;

            if (await _dataExcelReader.ReadAsync(_filedialog.FilePath, PhysicalValue.Pressure).ToListAsync() is not
                { Count: > 15 } dataPressure)
                return;
            var pressurePolynimial = dataPressure.CreateThirdOrderPolynomialExpression();
            var resultCoefPressure = _regression.CalcCoefs(pressurePolynimial);
            var сheckResult = dataPressure.Select(x1x2y => CalcResDelta(x1x2y, resultCoefPressure.ToArray())).ToList();
            if (await _dataExcelReader.ReadAsync(_filedialog.FilePath, PhysicalValue.Temperature).ToListAsync() is
                not { Count: > 8 } dataTemp)
                return;
            var polyTemp = dataTemp.CreateTwoOrderPolynomialExpression();
            var resCoefTemp = _regression.CalcCoefs(polyTemp);
            var resultsTemps = dataTemp.Select(x1x2y => CalcResTemp(x1x2y, resCoefTemp.ToArray())).ToList();
            if (resultCoefPressure is null || resCoefTemp is null)
                MessageBox.Show("Error. Нету коэффицентов");
            if (resultCoefPressure!.Any() && resCoefTemp!.Any())
            {
                var p = resultCoefPressure.ToArray();
                var t = resCoefTemp.ToArray();
                if(p.Length != 16 || t.Length != 9)
                    return;
                Coefficients coefficients = new(p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7], p[8], p[9], p[10], p[11], p[12], p[13], p[14], p[15],
                    t[0], t[1], t[2], t[3], t[4], t[5], t[6], t[7], t[8]);

                await _writer.Write(new List<double[]>() { p, t }, _filedialog.FilePath);


                await _fileCreator.CreateAsync(combine, coefficients);
                MessageBox.Show($" ΔP_max = {сheckResult.Max()};\n ΔT_max = {resultsTemps.Max()};", "Успех!");
            }
            else MessageBox.Show("Error. Нету коэффицентов");

        }

        /// <summary>
        /// Продолжить выполнение если файл существет
        /// </summary>
        /// <param name="combine"></param>
        /// <returns></returns>
        private static bool ShowUserDialogIfFilePathIsExists(string combine)
        {
            if (!File.Exists(combine)) return false;
            var msgBox = MessageBox.Show(
                $"Перезаписать уже существующтй путь? ({combine})",
                "Перезаписать файл",
                MessageBoxButton.YesNoCancel
            );
            switch (msgBox)
            {
                case MessageBoxResult.None:
                case MessageBoxResult.Cancel:
                case MessageBoxResult.No:
                    return true;
            }
            return false;
        }

        private double CalcResTemp(DataTwoFact data, double[] resultCheckedCoef)
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
                }
            }
            return Math.Abs(data.Y - res);
        }
        private double CalcResDelta(DataTwoFact data, double[] resultCheckedCoef)
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
            return Math.Abs(data.Y - res);
        }
	//Test
        private bool CanCalcFromExelCommandExecute(object p)
        {
		
            return true;
        }

 
        #endregion

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }


        private string _inputSerial;

        public string InputSerial
        {
            get => _inputSerial;
            set
            {
                if(value.Length > 10)
                    return;
                if (!Set(ref _inputSerial, value)) return;
                var count = _inputSerial.Length;
                if (count == 0) SerialText = "0000000000";
                else
                {
                    SerialText = InputSerial;
                    while (SerialText.Length < 10)
                    {
                        if (SerialText.Length < 9)
                            SerialText = "0" + SerialText;

                        if (SerialText.Length != 9) continue;
                        SerialText = "3" + SerialText;
                        break;
                    }
                }
            }
        }

        private string? _serialText;

        public string? SerialText
        {
            get => _serialText;
            set => Set(ref _serialText, value);
        }


        #region CmdChangePath

        private ICommand? _editPathFileSaveCommand;
        public ICommand EditPathFileSaveCommand =>
            _editPathFileSaveCommand ?? new LambdaCommandAsync(OnEditPathFileSaveCommandExecuted, CanEditPathFileSaveCommandExecute);
        private bool CanEditPathFileSaveCommandExecute(object p) => true;
        private async Task OnEditPathFileSaveCommandExecuted(object p)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Выбор расположения журнала подключений";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = App.CurrentDirectory;
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = App.CurrentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (dlg.FileName is not { Length: > 0 } newPath) return;

                if (!newPath.Equals(FilePath))
                {
                    _config.FilePath = (FilePath = dlg.FileName);
                    await _configService.WriteAsync(_config);
                }
            }
        }

        #endregion

        #region Cmd Load
        private ICommand? _loadCommand;

        public ICommand LoadCommand => _loadCommand ?? new LambdaCommandAsync(On_NAME_CommandExecuted, Can_NAME_CommandExecute);

        //сами методы
        private bool Can_NAME_CommandExecute(object p) => true;

        private async Task On_NAME_CommandExecuted(object p)
        {
            _config = await _configService.ReadAsync();
            FilePath = _config.FilePath;
        }

        #endregion

    }
}
