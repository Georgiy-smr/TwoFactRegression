using System.Globalization;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.View.Converters;

namespace TwoFactRegressCalc.Tests;

public class RegressionCalculatorTests
{
    private static List<DataTwoFact> GenerateData(int count)
    {
        var side = (int)Math.Ceiling(Math.Sqrt(count));
        var data = new List<DataTwoFact>(count);
        for (var i = 0; i < count; i++)
        {
            double x1 = i % side + 1;
            double x2 = i / side + 1;
            double y = 2 + 3 * x1 - 1.5 * x2 + 0.4 * x1 * x2 - 0.05 * x1 * x1 * x1 + 0.02 * x2 * x2 * x2;
            data.Add(new DataTwoFact { X1 = x1, X2 = x2, Y = y });
        }
        return data;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(8)]
    public void Calculate_Pressure_BelowMinimumPointCount_ReturnsNoCandidates(int count)
    {
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(count), PhysicalValue.Pressure);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(12)]
    [InlineData(15)]
    public void Calculate_Pressure_BelowThirdOrderThreshold_OnlyReturnsSecondOrderCandidates(int count)
    {
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(count), PhysicalValue.Pressure).ToList();

        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Equal(9, r.Coefficients.Count));
        Assert.All(results, r => Assert.DoesNotContain(r.Coefficients, double.IsNaN));
    }

    [Theory]
    [InlineData(16)]
    [InlineData(20)]
    [InlineData(24)]
    public void Calculate_Pressure_BelowFourthOrderThreshold_ReturnsSecondAndThirdOrderCandidatesOnly(int count)
    {
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(count), PhysicalValue.Pressure).ToList();

        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Contains(r.Coefficients.Count, new[] { 9, 16 }));
        Assert.Contains(results, r => r.Coefficients.Count == 16);
        Assert.All(results, r => Assert.DoesNotContain(r.Coefficients, double.IsNaN));
    }

    [Theory]
    [InlineData(25)]
    [InlineData(30)]
    public void Calculate_Pressure_AtOrAboveFourthOrderThreshold_CanReturnAllThreeOrders(int count)
    {
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(count), PhysicalValue.Pressure).ToList();

        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Contains(r.Coefficients.Count, new[] { 9, 16, 25 }));
        Assert.Contains(results, r => r.Coefficients.Count == 25);
        Assert.All(results, r => Assert.DoesNotContain(r.Coefficients, double.IsNaN));
    }

    [Theory]
    [InlineData(25)]
    [InlineData(30)]
    public void Calculate_Pressure_AtOrAboveFourthOrderThreshold_ReturnsFourFourthOrderCandidates(int count)
    {
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(count), PhysicalValue.Pressure).ToList();

        Assert.Equal(4, results.Count(r => r.Coefficients.Count == 25));
        Assert.All(results, r => Assert.DoesNotContain(r.Coefficients, double.IsNaN));
    }

    [Fact]
    public void Calculate_Pressure_DoesNotThrow_AcrossBoundaryAndNonBoundaryPointCounts()
    {
        var calculator = new RegressionCalculator();

        for (var count = 0; count <= 30; count++)
        {
            var exception = Record.Exception(() => calculator.Calculate(GenerateData(count), PhysicalValue.Pressure).ToList());
            Assert.Null(exception);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(8)]
    public void Calculate_Temperature_BelowMinimumPointCount_ReturnsNoCandidates(int count)
    {
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(count), PhysicalValue.Temperature);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(16)]
    [InlineData(25)]
    [InlineData(30)]
    public void Calculate_Temperature_AtOrAboveMinimumPointCount_OnlyReturnsSecondOrderCandidates(int count)
    {
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(count), PhysicalValue.Temperature).ToList();

        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Equal(9, r.Coefficients.Count));
        Assert.True(results.Count <= 4, "Temperature should never surface more than the 4 second-order variants.");
        Assert.All(results, r => Assert.DoesNotContain(r.Coefficients, double.IsNaN));
    }

    [Fact]
    public void Calculate_Temperature_DoesNotThrow_AcrossBoundaryAndNonBoundaryPointCounts()
    {
        var calculator = new RegressionCalculator();

        for (var count = 0; count <= 30; count++)
        {
            var exception = Record.Exception(() => calculator.Calculate(GenerateData(count), PhysicalValue.Temperature).ToList());
            Assert.Null(exception);
        }
    }

    [Theory]
    [InlineData(2, 9)]
    [InlineData(3, 16)]
    [InlineData(4, 25)]
    public void Calculate_Pressure_ReturnsMinimaxCandidateForEachOrder(int order, int coefficientCount)
    {
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(30), PhysicalValue.Pressure).ToList();

        var minimax = Assert.Single(results, r => r.Name == RegressionMethodNames.Minimax && r.Degree == order);
        Assert.Equal(coefficientCount, minimax.Coefficients.Count);
        Assert.DoesNotContain(minimax.Coefficients, double.IsNaN);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Calculate_Pressure_MinimaxMaxErrorDoesNotExceedDesignMatrixQr(int order)
    {
        // GenerateData is an exact cubic, so at orders 3 and 4 both errors are ~0 and only rounding separates them.
        const double epsilon = 1e-9;
        var calculator = new RegressionCalculator();

        var results = calculator.Calculate(GenerateData(30), PhysicalValue.Pressure).ToList();

        var minimax = Assert.Single(results, r => r.Name == RegressionMethodNames.Minimax && r.Degree == order);
        var qr = Assert.Single(results, r => r.Name == RegressionMethodNames.QrDesignMatrix && r.Degree == order);
        Assert.True(minimax.MaxError <= qr.MaxError + epsilon,
            $"Minimax MaxError {minimax.MaxError} exceeds design-matrix QR MaxError {qr.MaxError} at order {order}.");
    }

    [Fact]
    public void DescriptionConverter_HasSpecificDescriptionForEveryProducedMethodName()
    {
        var calculator = new RegressionCalculator();
        var converter = new RegressionMethodDescriptionConverter();

        var candidates = calculator.Calculate(GenerateData(30), PhysicalValue.Pressure)
            .Concat(calculator.Calculate(GenerateData(30), PhysicalValue.Temperature))
            .ToList();

        Assert.NotEmpty(candidates);
        Assert.All(candidates, candidate =>
        {
            var description = (string)converter.Convert(candidate, typeof(string), null!, CultureInfo.InvariantCulture);
            Assert.False(description.StartsWith(RegressionMethodDescriptionConverter.DefaultDescription),
                $"Method \"{candidate.Name}\" falls back to the default description.");
        });
    }
}
