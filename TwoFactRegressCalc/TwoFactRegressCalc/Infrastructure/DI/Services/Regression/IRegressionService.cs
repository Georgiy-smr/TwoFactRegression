using Regression.Two_factor_regression;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal interface IRegressionService
{
    IEnumerable<double> Get(IEnumerable<DataTwoFact> data);
}
