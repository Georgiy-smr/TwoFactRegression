using System.Windows.Input;
using Regression.OutlierDetection;
using TwoFactRegressCalc.Infrastructure.Commands.Base;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels.Base;

namespace TwoFactRegressCalc.ViewModels;

public class DatasetReviewViewModel : ViewModel
{
    // Dataset position 0 is Excel row 2: row 1 holds the column headers.
    public const int ExcelRowOffset = 2;

    public const string OutlierText = "Ошибочная точка";
    public const string AmbiguousPointText = "Подозрительная точка";
    public const string UnresolvableSeriesText = "Серия не разобрана — переснять";
    public const string SkippedSeriesPrefix = "Серия пропущена: ";

    public DatasetReviewViewModel(DatasetCheckResult checkResult)
    {
        Rows = BuildRows(checkResult);
        Message = $"Найдено проблем: {Rows.Count}. Продолжить расчёт коэффициентов?";
    }

    public string Message { get; }

    public IReadOnlyList<DatasetReviewRow> Rows { get; }

    public event EventHandler? RequestClose;

    private ICommand? _continueCommand;
    public ICommand ContinueCommand => _continueCommand ??= new LambdaCommand(OnContinueExecuted);

    private void OnContinueExecuted(object p) => RequestClose?.Invoke(this, EventArgs.Empty);

    public static IReadOnlyList<DatasetReviewRow> BuildRows(DatasetCheckResult checkResult)
        => checkResult.Series
            .SelectMany(BuildSeriesRows)
            .OrderBy(r => r.NominalTemperature)
            .ThenBy(r => r.FirstExcelRow)
            .ToList();

    private static IEnumerable<DatasetReviewRow> BuildSeriesRows(SeriesCheck series) => series switch
    {
        CheckedSeries checkedSeries => checkedSeries.SuspiciousPoints.Select(BuildPointRow),
        UnresolvableSeries => [BuildSeriesRow(series, UnresolvableSeriesText)],
        SkippedSeries skipped => [BuildSeriesRow(series, SkippedSeriesPrefix + skipped.Reason)],
        _ => [],
    };

    private static DatasetReviewRow BuildPointRow(DatasetSuspiciousPoint point)
    {
        var excelRow = point.Row + ExcelRowOffset;
        var (result, codeError) = point.Detail switch
        {
            Outlier outlier => (OutlierText, (double?)outlier.CodeError),
            _ => (AmbiguousPointText, null),
        };
        return new DatasetReviewRow(
            point.NominalTemperature, excelRow, excelRow.ToString(), point.Detail.Point.Y, result, codeError);
    }

    private static DatasetReviewRow BuildSeriesRow(SeriesCheck series, string result)
    {
        if (series.Rows.Count == 0)
            return new DatasetReviewRow(series.NominalTemperature, 0, string.Empty, null, result, null);

        var first = series.Rows.Min() + ExcelRowOffset;
        var last = series.Rows.Max() + ExcelRowOffset;
        var range = first == last ? first.ToString() : $"{first}–{last}";
        return new DatasetReviewRow(series.NominalTemperature, first, range, null, result, null);
    }
}
