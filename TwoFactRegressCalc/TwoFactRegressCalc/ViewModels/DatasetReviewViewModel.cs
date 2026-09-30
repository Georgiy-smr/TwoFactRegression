using System.Windows.Input;
using Regression.OutlierDetection;
using TwoFactRegressCalc.Infrastructure.Commands.Base;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels.Base;

namespace TwoFactRegressCalc.ViewModels;

public class DatasetReviewViewModel : ViewModel
{
    // Dataset position 0 is Excel row 2: row 1 holds the column headers.
    private const int ExcelRowOffset = 2;

    private const string OutlierText = "Ошибочная точка";
    private const string AmbiguousPointText = "Подозрительная точка";
    private const string UnresolvableSeriesText = "Серия не разобрана — переснять";
    private const string SkippedSeriesPrefix = "Серия пропущена: ";

    public DatasetReviewViewModel(DatasetCheckResult checkResult)
    {
        Rows = BuildRows(checkResult);
        Message = $"Найдено проблем: {Rows.Count}. Продолжить расчёт коэффициентов?";
        ContinueCommand = new LambdaCommand(OnContinueExecuted);
    }

    public string Message { get; }

    public IReadOnlyList<DatasetReviewRow> Rows { get; }

    public event EventHandler RequestClose = delegate { };

    public ICommand ContinueCommand { get; }

    private void OnContinueExecuted(object p) => RequestClose(this, EventArgs.Empty);

    private IReadOnlyList<DatasetReviewRow> BuildRows(DatasetCheckResult checkResult)
        => checkResult.Series
            .SelectMany(BuildSeriesRows)
            .OrderBy(r => r.NominalTemperature)
            .ThenBy(r => r.FirstExcelRow)
            .ToList();

    private IEnumerable<DatasetReviewRow> BuildSeriesRows(SeriesCheck series) => series switch
    {
        CheckedSeries checkedSeries => checkedSeries.SuspiciousPoints.Select(BuildPointRow),
        UnresolvableSeries => [BuildSeriesRow(series, UnresolvableSeriesText)],
        SkippedSeries skipped => [BuildSeriesRow(series, SkippedSeriesPrefix + skipped.Reason)],
        _ => [],
    };

    private DatasetReviewRow BuildPointRow(DatasetSuspiciousPoint point)
    {
        var excelRow = point.Row + ExcelRowOffset;
        var (result, codeError) = point.Detail switch
        {
            Outlier outlier => (OutlierText, FormatCodeError(outlier.CodeError)),
            _ => (AmbiguousPointText, string.Empty),
        };
        return new DatasetReviewRow(
            point.NominalTemperature, excelRow, excelRow, FormatPressure(point.Detail.Point.Y), result, codeError);
    }

    // The checker builds each series from dataset points, so Rows is never empty.
    private DatasetReviewRow BuildSeriesRow(SeriesCheck series, string result)
        => new(series.NominalTemperature,
            series.Rows.Min() + ExcelRowOffset,
            series.Rows.Max() + ExcelRowOffset,
            string.Empty, result, string.Empty);

    private string FormatPressure(double pressure) => pressure.ToString("0.####");

    private string FormatCodeError(double codeError) => codeError.ToString("0.##");
}
