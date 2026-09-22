using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Interfaces;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal interface IRegressionService
{
    IEnumerable<double> Get(
        IEnumerable<DataTwoFact> data,
        Func<IEnumerable<DataTwoFact>, IPolynomialExpression> func,
        params IBasisExponents[] bases);
}
