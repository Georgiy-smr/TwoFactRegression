using Regression.OutlierDetection;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Tests;

public class RegressionPickStepTests
{
    private sealed record PickCall(string ValueName, TwoFactorRegressionResult[] Candidates, TwoFactorRegressionResult Picked);

    private sealed class FakePicker : IRegressionResultPicker
    {
        public List<PickCall> Calls { get; } = new();
        public IReadOnlySet<string> CancelOn { get; init; } = new HashSet<string>();

        public TwoFactorRegressionResult Pick(IEnumerable<TwoFactorRegressionResult> candidates, string valueName)
        {
            if (CancelOn.Contains(valueName))
            {
                Calls.Add(new PickCall(valueName, candidates.ToArray(), candidates.First()));
                throw new RegressionSelectionCancelledException(valueName);
            }
            var all = candidates.ToArray();
            var picked = all.OrderBy(c => c.MaxError).First();
            Calls.Add(new PickCall(valueName, all, picked));
            return picked;
        }
    }

    // Pressure is an exact quadratic of the codes; temperature alternates wildly, so no model fits it closely.
    private List<CalibrationPoint> GenerateDataset(int count)
    {
        var dataset = new List<CalibrationPoint>(count);
        for (var i = 0; i < count; i++)
        {
            double pressureCode = i % 6 + 1;
            double temperatureCode = i / 6 + 1;
            var pressure = 1 + 2 * pressureCode - temperatureCode + 0.5 * pressureCode * temperatureCode;
            var temperature = i % 2 == 0 ? 1000 : -1000;
            dataset.Add(new CalibrationPoint(pressureCode, temperatureCode, pressure, temperature));
        }
        return dataset;
    }

    private IRegressionCalculator Chain(FakePicker picker)
    {
        var candidates = new RegressionCandidateCalculator();
        return new PressurePickStep(candidates, picker,
            new TemperaturePickStep(candidates, picker, new RegressionChainEnd()));
    }

    [Fact]
    public void Calculate_PicksPressureThenTemperature_AndFillsBothCoefficientSets()
    {
        var picker = new FakePicker();

        var result = Chain(picker).Calculate(GenerateDataset(30)).GetAllCoefficients();

        Assert.Equal(["давление", "температура"], picker.Calls.Select(c => c.ValueName));
        Assert.Equal(picker.Calls[0].Picked.Coefficients, result.PressureCoefficients);
        Assert.Equal(picker.Calls[1].Picked.Coefficients, result.TemperatureCoefficients);
    }

    [Fact]
    public void Calculate_EachStepFitsItsOwnColumn()
    {
        var picker = new FakePicker();

        Chain(picker).Calculate(GenerateDataset(30));

        var (pressure, temperature) = (picker.Calls[0].Picked, picker.Calls[1].Picked);
        Assert.True(pressure.MaxError < 1e-6, $"Pressure fit MaxError {pressure.MaxError} - wrong column?");
        Assert.True(temperature.MaxError > 100, $"Temperature fit MaxError {temperature.MaxError} - wrong column?");
    }

    [Fact]
    public void Calculate_OffersUpToFourthOrderForPressureAndOnlySecondOrderForTemperature()
    {
        var picker = new FakePicker();

        Chain(picker).Calculate(GenerateDataset(30));

        Assert.Equal([2, 3, 4], picker.Calls[0].Candidates.Select(c => c.Degree).Distinct().Order());
        Assert.Equal([2], picker.Calls[1].Candidates.Select(c => c.Degree).Distinct());
    }

    [Fact]
    public void Calculate_TooFewPoints_ThrowsWithoutShowingThePicker()
    {
        var picker = new FakePicker();

        var exception = Assert.Throws<NoRegressionCandidatesException>(() => Chain(picker).Calculate(GenerateDataset(5)));

        Assert.Equal("давление", exception.ValueName);
        Assert.Empty(picker.Calls);
    }

    [Fact]
    public void Calculate_PressurePickCancelled_StopsBeforeTemperature()
    {
        var picker = new FakePicker { CancelOn = new HashSet<string> { "давление" } };

        Assert.ThrowsAny<OperationCanceledException>(() => Chain(picker).Calculate(GenerateDataset(30)));
        Assert.Equal(["давление"], picker.Calls.Select(c => c.ValueName));
    }
}
