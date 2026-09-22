using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFact;

internal class QrFactorizedAlgorithm : IRegression<DataTwoFact>
{
    public QrFactorizedAlgorithm()
    {
        _serviceRegression = new ApproximationService(new SolverMathNet(), new RowParser(), new DerivativeCalculator());
    }
    private readonly global::Regression.Two_factor_regression.Interfaces.Services.IRegressionAnalysisService _serviceRegression;

    public string Name => "QR decomposition";

    public IEnumerable<double> CalcCoefs(IPolynomialExpression regressionData)
    {
        return _serviceRegression.GetValues(regressionData);
    }
}