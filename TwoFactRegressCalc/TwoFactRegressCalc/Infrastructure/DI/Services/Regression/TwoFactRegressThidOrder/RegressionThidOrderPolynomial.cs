using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFactRegressThidOrder;

internal class RegressionThidOrderPolynomial : IRegression<DataTwoFact>
{
    public RegressionThidOrderPolynomial()
    {
        _serviceRegression = new ApproximationService(new Solver(), new RowParser(), new DerivativeCalculator());
    }
    private readonly global::Regression.Two_factor_regression.Interfaces.Services.IRegressionAnalysisService _serviceRegression;

    public IEnumerable<double> CalcCoefs(IPolynomialExpression regressionData)
    {
        return _serviceRegression.GetValues(regressionData);
    }
}