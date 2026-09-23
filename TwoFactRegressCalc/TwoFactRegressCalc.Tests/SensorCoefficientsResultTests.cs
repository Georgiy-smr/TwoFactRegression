using System.Globalization;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Tests;

public class SensorCoefficientsResultTests
{
    private static string F(double value) => value.ToString("F25", CultureInfo.InvariantCulture);

    private static string GetA(CoefficientsBySensor c, int index) => index switch
    {
        0 => c.a0, 1 => c.a1, 2 => c.a2, 3 => c.a3, 4 => c.a4, 5 => c.a5, 6 => c.a6, 7 => c.a7,
        8 => c.a8, 9 => c.a9, 10 => c.a10, 11 => c.a11, 12 => c.a12, 13 => c.a13, 14 => c.a14, 15 => c.a15,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    private static string GetB(CoefficientsBySensor c, int index) => index switch
    {
        0 => c.b0, 1 => c.b1, 2 => c.b2, 3 => c.b3, 4 => c.b4, 5 => c.b5, 6 => c.b6, 7 => c.b7, 8 => c.b8,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    [Fact]
    public void GetCoefficientsBySensor_ExactCounts_PassesThroughUnchanged()
    {
        var pressure = Enumerable.Range(0, 16).Select(i => (double)i).ToList();
        var temperature = Enumerable.Range(100, 9).Select(i => (double)i).ToList();

        var result = new SensorCoefficientsResult(pressure, temperature).GetCoefficientsBySensor();

        for (var i = 0; i < 16; i++)
            Assert.Equal(F(pressure[i]), GetA(result, i));
        for (var i = 0; i < 9; i++)
            Assert.Equal(F(temperature[i]), GetB(result, i));
    }

    [Fact]
    public void GetCoefficientsBySensor_PressureShortfall_ZeroPadsMissingPositions()
    {
        var pressure = Enumerable.Range(0, 9).Select(i => (double)i).ToList();
        var temperature = Enumerable.Range(100, 9).Select(i => (double)i).ToList();

        var result = new SensorCoefficientsResult(pressure, temperature).GetCoefficientsBySensor();

        for (var i = 0; i < 9; i++)
            Assert.Equal(F(pressure[i]), GetA(result, i));
        for (var i = 9; i < 16; i++)
            Assert.Equal(F(0), GetA(result, i));
    }

    [Fact]
    public void GetCoefficientsBySensor_TemperatureShortfall_ZeroPadsMissingPositions()
    {
        var pressure = Enumerable.Range(0, 16).Select(i => (double)i).ToList();
        var temperature = Enumerable.Range(100, 5).Select(i => (double)i).ToList();

        var result = new SensorCoefficientsResult(pressure, temperature).GetCoefficientsBySensor();

        for (var i = 0; i < 5; i++)
            Assert.Equal(F(temperature[i]), GetB(result, i));
        for (var i = 5; i < 9; i++)
            Assert.Equal(F(0), GetB(result, i));
    }

    [Fact]
    public void GetCoefficientsBySensor_PressureExcess_DropsExtraCoefficientsWithoutThrowing()
    {
        var pressure = Enumerable.Range(0, 25).Select(i => (double)i).ToList();
        var temperature = Enumerable.Range(100, 9).Select(i => (double)i).ToList();

        var result = new SensorCoefficientsResult(pressure, temperature).GetCoefficientsBySensor();

        for (var i = 0; i < 16; i++)
            Assert.Equal(F(pressure[i]), GetA(result, i));
    }

    [Fact]
    public void GetCoefficientsBySensor_TemperatureExcess_DropsExtraCoefficientsWithoutThrowing()
    {
        var pressure = Enumerable.Range(0, 16).Select(i => (double)i).ToList();
        var temperature = Enumerable.Range(100, 16).Select(i => (double)i).ToList();

        var result = new SensorCoefficientsResult(pressure, temperature).GetCoefficientsBySensor();

        for (var i = 0; i < 9; i++)
            Assert.Equal(F(temperature[i]), GetB(result, i));
    }

    [Theory]
    [InlineData(9)]
    [InlineData(16)]
    [InlineData(25)]
    public void GetAllCoefficients_ReturnsFullSetRegardlessOfLength(int count)
    {
        var pressure = Enumerable.Range(0, count).Select(i => (double)i).ToList();
        var temperature = Enumerable.Range(100, count).Select(i => (double)i).ToList();

        var result = new SensorCoefficientsResult(pressure, temperature).GetAllCoefficients();

        Assert.Equal(pressure, result.PressureCoefficients);
        Assert.Equal(temperature, result.TemperatureCoefficients);
    }
}
