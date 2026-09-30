using Microsoft.Extensions.Logging;
using Regression.OutlierDetection;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.DatasetReview;

// Checks the dataset for mis-loaded points before any fitting; the operator may cancel the whole calculation.
internal class DatasetReviewStep : IRegressionCalculator
{
    private readonly IRegressionCalculator _next;
    private readonly Config _config;
    private readonly ILogger<DatasetReviewStep> _logger;

    public DatasetReviewStep(IRegressionCalculator next, Config config, ILogger<DatasetReviewStep> logger)
    {
        _next = next;
        _config = config;
        _logger = logger;
    }

    public IReadOnlyList<TwoFactorRegressionResult> Calculate(IReadOnlyList<CalibrationPoint> dataset)
    {
        Review(dataset);
        return _next.Calculate(dataset);
    }

    private void Review(IReadOnlyList<CalibrationPoint> dataset)
    {
        var checkResult = new CalibrationDatasetChecker(_config.AccuracyClassPercent).Check(dataset);

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
