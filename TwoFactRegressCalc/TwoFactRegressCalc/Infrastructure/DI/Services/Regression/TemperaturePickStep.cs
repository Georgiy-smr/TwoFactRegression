using Regression.OutlierDetection;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class TemperaturePickStep : RegressionPickStep
{
    public TemperaturePickStep(RegressionCandidateCalculator candidates, IRegressionResultPicker picker, IRegressionCalculator next)
        : base(candidates, picker, next)
    {
    }

    protected override string ValueName => "температура";

    // Temperature is always fit at 2nd order - the client doesn't need a degree choice for it.
    protected override int MaxDegree => 2;

    protected override double MeasuredValue(CalibrationPoint point) => point.Temperature;

    protected override SensorCoefficientsResult AddTo(SensorCoefficientsResult result, IReadOnlyList<double> coefficients)
        => result.WithTemperature(coefficients);
}
