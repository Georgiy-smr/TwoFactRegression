using Microsoft.Extensions.Logging;
using Regression.OutlierDetection;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.DatasetReview;

public class DatasetErrorReviewService : IDatasetErrorReview
{
    private readonly ILogger<DatasetErrorReviewService> _logger;

    public DatasetErrorReviewService(ILogger<DatasetErrorReviewService> logger)
    {
        _logger = logger;
    }

    public void Review(IReadOnlyList<CalibrationPoint> dataset, double accuracyClassPercent)
    {
        var checkResult = new CalibrationDatasetChecker(accuracyClassPercent).Check(dataset);

        if (checkResult.IsClean)
        {
            _logger.LogInformation(
                "Calibration dataset check: {SeriesCount} series, {PointCount} points, no problems found",
                checkResult.Series.Count, dataset.Count);
            return;
        }

        _logger.LogWarning(
            "Calibration dataset check: {SeriesCount} series, {SuspiciousCount} suspicious points, " +
            "{UnresolvableCount} unresolved series, {SkippedCount} skipped series",
            checkResult.Series.Count,
            checkResult.SuspiciousPoints.Count,
            checkResult.Series.OfType<UnresolvableSeries>().Count(),
            checkResult.Series.OfType<SkippedSeries>().Count());

        var viewModel = new DatasetReviewViewModel(checkResult);
        var window = new DatasetReviewWindow { DataContext = viewModel, Owner = System.Windows.Application.Current?.MainWindow };

        if (window.ShowDialog() != true)
        {
            _logger.LogInformation("Calculation cancelled by the operator after the dataset check");
            throw new DatasetReviewCancelledException();
        }

        _logger.LogInformation("Operator chose to continue the calculation despite the dataset check findings");
    }
}
