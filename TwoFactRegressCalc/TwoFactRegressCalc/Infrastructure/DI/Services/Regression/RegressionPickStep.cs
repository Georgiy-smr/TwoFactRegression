using Regression.OutlierDetection;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

// Fits one physical value, lets the operator pick a model, then hands the dataset to the next step.
internal class RegressionPickStep : IRegressionCalculator
{
    private readonly PhysicalValue _physicalValue;
    private readonly RegressionCandidateCalculator _candidates;
    private readonly IRegressionResultPicker _picker;
    private readonly IRegressionCalculator _next;

    public RegressionPickStep(
        PhysicalValue physicalValue,
        RegressionCandidateCalculator candidates,
        IRegressionResultPicker picker,
        IRegressionCalculator next)
    {
        _physicalValue = physicalValue;
        _candidates = candidates;
        _picker = picker;
        _next = next;
    }

    public IReadOnlyList<TwoFactorRegressionResult> Calculate(IReadOnlyList<CalibrationPoint> dataset)
    {
        var candidates = _candidates.Calculate(dataset.Select(ToRegressionData), _physicalValue).ToArray();
        if (candidates.Length == 0)
            throw new NoRegressionCandidatesException(_physicalValue, dataset.Count);

        var picked = _picker.Pick(candidates, _physicalValue);
        return [picked, .. _next.Calculate(dataset)];
    }

    private DataTwoFact ToRegressionData(CalibrationPoint point) => new()
    {
        X1 = point.PressureCode,
        X2 = point.TemperatureCode,
        Y = _physicalValue == PhysicalValue.Pressure ? point.Pressure : point.Temperature,
    };
}
