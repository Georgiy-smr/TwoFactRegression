namespace TwoFactRegressCalc.Models;

public sealed class SensorCoefficientsResult
{
    private readonly IReadOnlyList<double> _pressureCoefficients;
    private readonly IReadOnlyList<double> _temperatureCoefficients;

    public SensorCoefficientsResult(
        IReadOnlyList<double> pressureCoefficients,
        IReadOnlyList<double> temperatureCoefficients)
    {
        _pressureCoefficients = pressureCoefficients;
        _temperatureCoefficients = temperatureCoefficients;
    }

    public CoefficientsBySensor GetCoefficientsBySensor()
        => new(
            At(_pressureCoefficients, 0), At(_pressureCoefficients, 1), At(_pressureCoefficients, 2), At(_pressureCoefficients, 3),
            At(_pressureCoefficients, 4), At(_pressureCoefficients, 5), At(_pressureCoefficients, 6), At(_pressureCoefficients, 7),
            At(_pressureCoefficients, 8), At(_pressureCoefficients, 9), At(_pressureCoefficients, 10), At(_pressureCoefficients, 11),
            At(_pressureCoefficients, 12), At(_pressureCoefficients, 13), At(_pressureCoefficients, 14), At(_pressureCoefficients, 15),
            At(_temperatureCoefficients, 0), At(_temperatureCoefficients, 1), At(_temperatureCoefficients, 2), At(_temperatureCoefficients, 3),
            At(_temperatureCoefficients, 4), At(_temperatureCoefficients, 5), At(_temperatureCoefficients, 6), At(_temperatureCoefficients, 7),
            At(_temperatureCoefficients, 8));

    public AllSensorCoefficients GetAllCoefficients()
        => new(_pressureCoefficients, _temperatureCoefficients);

    private static double At(IReadOnlyList<double> src, int i) => i < src.Count ? src[i] : 0d;
}
