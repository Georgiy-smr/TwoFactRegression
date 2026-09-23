namespace TwoFactRegressCalc.Models;

public sealed class AllSensorCoefficients
{
    public AllSensorCoefficients(IReadOnlyList<double> pressureCoefficients, IReadOnlyList<double> temperatureCoefficients)
    {
        PressureCoefficients = pressureCoefficients;
        TemperatureCoefficients = temperatureCoefficients;
    }

    public IReadOnlyList<double> PressureCoefficients { get; }

    public IReadOnlyList<double> TemperatureCoefficients { get; }
}
