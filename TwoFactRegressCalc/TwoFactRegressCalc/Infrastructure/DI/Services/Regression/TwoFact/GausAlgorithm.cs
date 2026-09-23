using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFact;

internal class GausAlgorithm : IRegression<DataTwoFact>
{
    public GausAlgorithm()
    {
        _serviceRegression = new ApproximationService(new Solver(), new RowParser(), new DerivativeCalculator());
    }
    private readonly global::Regression.Two_factor_regression.Interfaces.Services.IRegressionAnalysisService _serviceRegression;

    public string Name => "Гаусс";

    public IEnumerable<double> CalcCoefs(IPolynomialExpression regressionData)
    {
        return _serviceRegression.GetValues(regressionData);
    }
}