using Regression.OutlierDetection;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc.Tests;

public class DatasetReviewViewModelTests
{
    private DatasetSuspiciousPoint OutlierAt(int row, double temperature, double pressure, double codeError)
        => new(row, temperature, new Outlier(0, new DataTwoFact { Y = pressure }, codeError));

    private DatasetSuspiciousPoint AmbiguousAt(int row, double temperature, double pressure)
        => new(row, temperature, new AmbiguousPoint(0, new DataTwoFact { Y = pressure }));

    private CheckedSeries Checked(double temperature, int[] rows, params DatasetSuspiciousPoint[] points)
        => new(temperature, rows, points);

    [Fact]
    public void Rows_OrdersByNominalTemperatureThenExcelRow()
    {
        var result = new DatasetCheckResult(new SeriesCheck[]
        {
            Checked(60, [12, 13, 14], OutlierAt(14, 60, 40, 1), OutlierAt(12, 60, 0, 1)),
            Checked(-20, [0, 1, 2], AmbiguousAt(2, -20, 40)),
            new SkippedSeries(20, [6, 7], "мало точек"),
            Checked(20, [8, 9, 10], OutlierAt(9, 20, 20, 1)),
        });

        var rows = new DatasetReviewViewModel(result).Rows;

        Assert.Equal(
            [(-20.0, 4), (20.0, 8), (20.0, 11), (60.0, 14), (60.0, 16)],
            rows.Select(r => (r.NominalTemperature, r.FirstExcelRow)).ToArray());
    }

    [Fact]
    public void Rows_ExcelRowIsDatasetPositionPlusTwo()
    {
        var result = new DatasetCheckResult(new SeriesCheck[] { Checked(20, [0, 5, 7], OutlierAt(5, 20, 40, 3)) });

        var row = Assert.Single(new DatasetReviewViewModel(result).Rows);

        Assert.Equal(7, row.FirstExcelRow);
        Assert.Equal("7", row.ExcelRow);
    }

    [Fact]
    public void Rows_WholeSeriesStatusShowsItsExcelRowRange()
    {
        var result = new DatasetCheckResult(new SeriesCheck[] { new UnresolvableSeries(20, [6, 7, 8, 9], 2, 0.1) });

        var row = Assert.Single(new DatasetReviewViewModel(result).Rows);

        Assert.Equal((8, 11), (row.FirstExcelRow, row.LastExcelRow));
        Assert.Equal("8–11", row.ExcelRow);
        Assert.Equal(string.Empty, row.Pressure);
    }

    [Fact]
    public void Rows_UsesRussianTextForEachResultType()
    {
        var result = new DatasetCheckResult(new SeriesCheck[]
        {
            Checked(-20, [0, 1], OutlierAt(0, -20, 0, 5), AmbiguousAt(1, -20, 20)),
            new UnresolvableSeries(20, [2, 3], 1, 0.1),
            new SkippedSeries(60, [4, 5], "мало точек"),
        });

        var rows = new DatasetReviewViewModel(result).Rows;

        Assert.Equal(
            ["Ошибочная точка", "Подозрительная точка", "Серия не разобрана — переснять", "Серия пропущена: мало точек"],
            rows.Select(r => r.Result).ToArray());
    }

    [Fact]
    public void Rows_ShowsCodeErrorOnlyForOutliers()
    {
        var result = new DatasetCheckResult(new SeriesCheck[]
        {
            Checked(-20, [0, 1], OutlierAt(0, -20, 0, -250.5), AmbiguousAt(1, -20, 20)),
            new UnresolvableSeries(20, [2, 3], 1, 0.1),
            new SkippedSeries(60, [4, 5], "мало точек"),
        });

        var rows = new DatasetReviewViewModel(result).Rows;

        Assert.Equal(
            [(-250.5 * 100000).ToString("0"), "", "", ""],
            rows.Select(r => r.CodeError).ToArray());
    }

    [Fact]
    public void Rows_ShowsPressureOfThePoint()
    {
        var result = new DatasetCheckResult(new SeriesCheck[] { Checked(20, [3], OutlierAt(3, 20, 45, 1)) });

        var row = Assert.Single(new DatasetReviewViewModel(result).Rows);

        Assert.Equal("45", row.Pressure);
    }
}
