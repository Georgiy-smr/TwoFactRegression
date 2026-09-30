using Regression.OutlierDetection;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Tests;

public class RegressionPickStepTests
{
    private sealed class FakePicker : IRegressionResultPicker
    {
        public List<PhysicalValue> Calls { get; } = new();
        public IReadOnlySet<PhysicalValue> CancelOn { get; init; } = new HashSet<PhysicalValue>();

        public TwoFactorRegressionResult Pick(IEnumerable<TwoFactorRegressionResult> candidates, PhysicalValue physicalValue)
        {
            Calls.Add(physicalValue);
            if (CancelOn.Contains(physicalValue))
                throw new RegressionSelectionCancelledException(physicalValue);
            return candidates.OrderBy(c => c.MaxError).First();
        }
    }

    // Pressure is an exact quadratic of the codes; temperature alternates wildly, so no model fits it closely.
    private static List<CalibrationPoint> GenerateDataset(int count)
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

    private static IRegressionCalculator Chain(FakePicker picker)
    {
        var candidates = new RegressionCandidateCalculator();
        return new RegressionPickStep(PhysicalValue.Pressure, candidates, picker,
            new RegressionPickStep(PhysicalValue.Temperature, candidates, picker, new RegressionChainEnd()));
    }

    [Fact]
    public void Calculate_PicksPressureThenTemperature_AndReturnsBothResults()
    {
        var picker = new FakePicker();

        var results = Chain(picker).Calculate(GenerateDataset(30));

        Assert.Equal([PhysicalValue.Pressure, PhysicalValue.Temperature], picker.Calls);
        Assert.Equal([PhysicalValue.Pressure, PhysicalValue.Temperature], results.Select(r => r.PhysicalValue));
    }

    [Fact]
    public void Calculate_EachStepFitsItsOwnColumn()
    {
        var results = Chain(new FakePicker()).Calculate(GenerateDataset(30));

        Assert.True(results[0].MaxError < 1e-6, $"Pressure fit MaxError {results[0].MaxError} - wrong column?");
        Assert.True(results[1].MaxError > 100, $"Temperature fit MaxError {results[1].MaxError} - wrong column?");
    }

    [Fact]
    public void Calculate_TooFewPoints_ThrowsWithoutShowingThePicker()
    {
        var picker = new FakePicker();

        var exception = Assert.Throws<NoRegressionCandidatesException>(() => Chain(picker).Calculate(GenerateDataset(5)));

        Assert.Equal(PhysicalValue.Pressure, exception.PhysicalValue);
        Assert.Empty(picker.Calls);
    }

    [Fact]
    public void Calculate_PressurePickCancelled_StopsBeforeTemperature()
    {
        var picker = new FakePicker { CancelOn = new HashSet<PhysicalValue> { PhysicalValue.Pressure } };

        Assert.ThrowsAny<OperationCanceledException>(() => Chain(picker).Calculate(GenerateDataset(30)));
        Assert.Equal([PhysicalValue.Pressure], picker.Calls);
    }
}
