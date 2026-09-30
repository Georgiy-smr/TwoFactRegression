using Regression.OutlierDetection;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class PressurePickStep : RegressionPickStep
{
    public PressurePickStep(RegressionCandidateCalculator candidates, IRegressionResultPicker picker, IRegressionCalculator next)
        : base(candidates, picker, next)
    {
    }

    protected override string ValueName => "давление";

    protected override int MaxDegree => 4;

    protected override double MeasuredValue(CalibrationPoint point) => point.Pressure;

    protected override SensorCoefficientsResult AddTo(SensorCoefficientsResult result, IReadOnlyList<double> coefficients)
        => result.WithPressure(coefficients);
}
