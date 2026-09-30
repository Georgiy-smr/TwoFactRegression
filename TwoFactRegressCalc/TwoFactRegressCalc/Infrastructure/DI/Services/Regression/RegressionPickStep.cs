using Regression.OutlierDetection;
using Regression.Two_factor_regression;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

// Fits one measured value, lets the operator pick a model, then hands the dataset to the next step.
// Subclasses say which value that is: its Excel column, its max degree, its name and where its coefficients go.
internal abstract class RegressionPickStep : IRegressionCalculator
{
    private readonly RegressionCandidateCalculator _candidates;
    private readonly IRegressionResultPicker _picker;
    private readonly IRegressionCalculator _next;

    protected RegressionPickStep(
        RegressionCandidateCalculator candidates,
        IRegressionResultPicker picker,
        IRegressionCalculator next)
    {
        _candidates = candidates;
        _picker = picker;
        _next = next;
    }

    // Shown to the operator, e.g. in the picker title: «давление».
    protected abstract string ValueName { get; }

    protected abstract int MaxDegree { get; }

    protected abstract double MeasuredValue(CalibrationPoint point);

    protected abstract SensorCoefficientsResult AddTo(SensorCoefficientsResult result, IReadOnlyList<double> coefficients);

    public SensorCoefficientsResult Calculate(IReadOnlyList<CalibrationPoint> dataset)
    {
        var candidates = _candidates.Calculate(dataset.Select(ToRegressionData), MaxDegree).ToArray();
        if (candidates.Length == 0)
            throw new NoRegressionCandidatesException(ValueName, dataset.Count);

        var picked = _picker.Pick(candidates, ValueName);
        return AddTo(_next.Calculate(dataset), picked.Coefficients);
    }

    private DataTwoFact ToRegressionData(CalibrationPoint point) => new()
    {
        X1 = point.PressureCode,
        X2 = point.TemperatureCode,
        Y = MeasuredValue(point),
    };
}
