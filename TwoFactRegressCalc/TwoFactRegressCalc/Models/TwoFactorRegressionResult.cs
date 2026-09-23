using Regression.ErrorAnalysis;
using Regression.Two_factor_regression;

namespace TwoFactRegressCalc.Models;

public sealed class TwoFactorRegressionResult
{
    private readonly IReadOnlyList<double> _coefficients;
    private readonly ApproximationCalculationError _error;

    public TwoFactorRegressionResult(string name, IReadOnlyList<double> coefficients, IReadOnlyList<DataTwoFact> data)
    {
        Name = name;
        _coefficients = coefficients;
        _error = new ApproximationCalculationError(coefficients, data);
    }

    public string Name { get; }

    public IReadOnlyList<double> Coefficients => _coefficients;

    public double MaxError => _error.GetMax();
}
