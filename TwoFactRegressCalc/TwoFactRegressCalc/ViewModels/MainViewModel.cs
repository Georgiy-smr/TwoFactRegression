using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Regression.OutlierDetection;
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
using Serilog;

namespace TwoFactRegressCalc.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        private readonly IReadData<CalibrationPoint> _dataExcelReader;
        private readonly IDialogService _filedialog;
        private readonly IRegressionCalculator _regressionCalculator;
        private readonly IWriteData<AllSensorCoefficients> _writer;
        private readonly ICreate<CoefficientsBySensor> _fileCreator;
        private readonly IJsonFileService<Config> _configService;
        private readonly Config _config;
        private readonly ILogger<MainViewModel> _logger;

        public MainViewModel(
            IReadData<CalibrationPoint> dataExcelReader,
            IDialogService dialog,
            IRegressionCalculator regressionCalculator,
            IWriteData<AllSensorCoefficients> writer,
            ICreate<CoefficientsBySensor> fileCreator,
            IJsonFileService<Config> configService,
            Config config,
            ILogger<MainViewModel> logger)
        {
            _dataExcelReader = dataExcelReader;
            _filedialog = dialog;
            _regressionCalculator = regressionCalculator;
            _writer = writer;
            _fileCreator = fileCreator;
            _configService = configService;
            _config = config;
            _logger = logger;

            СalcFromExсelCommand = new LambdaCommandAsync(OnCalcFromExelCommandExecuted, CanCalcFromExelCommandExecute);
            EditPathFileSaveCommand = new LambdaCommandAsync(OnEditPathFileSaveCommandExecuted, CanEditPathFileSaveCommandExecute);
            LoadCommand = new LambdaCommandAsync(OnLoadCommandExecuted, CanLoadCommandExecute);
        }

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

        public ICommand СalcFromExсelCommand { get; }

        private async Task OnCalcFromExelCommandExecuted(object arg)
        {
            _filedialog.Filter = _filedialog.Filter = "Excel workbooks (*.xlsx;*.xls;*.ods)|*.xlsx;*.xls;*.ods|Excel 2003 (*.xls)|*.xls|OpenDocument Spreadsheet (*.ods)|*.ods";
            if (!_filedialog.OpenFileDialog()) 
                return;
            
            string combine = Path.Combine(
                FilePath, SerialText ?? Path.GetFileNameWithoutExtension(_filedialog.FilePath));

            if (ShowUserDialogIfFilePathIsExists(combine))
                return;

            try
            {
                var dataset = await _dataExcelReader.ReadAsync(_filedialog.FilePath).ToListAsync();
                var sensorCoefficients = _regressionCalculator.Calculate(dataset);

                await _writer.Write(sensorCoefficients.GetAllCoefficients(), _filedialog.FilePath);
                await _fileCreator.CreateAsync(combine, sensorCoefficients.GetCoefficientsBySensor());
                await _configService.WriteAsync(_config);
            }
            catch (OperationCanceledException e)
            {
                // The operator cancelled the dataset review or a model picker.
                _logger.LogInformation(e.Message);
            }
            catch (NoRegressionCandidatesException e)
            {
                MessageBox.Show(e.Message, "Ошибка расчёта", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Calculation from {File} failed", _filedialog.FilePath);
                MessageBox.Show(e.Message, "Ошибка расчёта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                $"Выбранный файл {combine} уже существует.\n Перезаписать?",
                "Совпадение названий файла",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question
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

        /// <summary>
        /// Класс точности датчика, % - порог проверки точек калибровки.
        /// Сохраняется в настройки после успешного расчёта.
        /// </summary>
        public double AccuracyClassPercent
        {
            get => _config.AccuracyClassPercent;
            set
            {
                if (double.IsFinite(value) && value > 0)
                    _config.AccuracyClassPercent = value;
                // Also reverts the TextBox to the last valid value when the input was rejected.
                OnPropertyChanged();
            }
        }

        private string? _serialText;

        public string? SerialText
        {
            get => _serialText;
            set => Set(ref _serialText, value);
        }


        #region CmdChangePath

        public ICommand EditPathFileSaveCommand { get; }
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
        public ICommand LoadCommand { get; }

        private bool CanLoadCommandExecute(object p) => true;

        // Reads settings.json into the shared Config; unreadable settings keep the defaults.
        private async Task OnLoadCommandExecuted(object p)
        {
            try
            {
                var saved = await _configService.ReadAsync();
                _config.FilePath = saved.FilePath;
                _config.AccuracyClassPercent = saved.AccuracyClassPercent;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to read settings, using defaults");
            }
            FilePath = _config.FilePath;
            OnPropertyChanged(nameof(AccuracyClassPercent));
        }

        #endregion

    }
}
