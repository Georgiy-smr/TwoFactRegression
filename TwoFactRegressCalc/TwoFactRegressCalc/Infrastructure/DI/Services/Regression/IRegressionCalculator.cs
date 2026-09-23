using Regression.Two_factor_regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal interface IRegressionCalculator
{
    /// <summary>
    /// Computes every regression variant applicable to <paramref name="data"/>'s point count.
    /// Temperature is always fit at 2nd order only, per the client's requirement that its
    /// degree never needs to be chosen; Pressure searches every order (2nd/3rd/4th).
    /// </summary>
    IEnumerable<TwoFactorRegressionResult> Calculate(IEnumerable<DataTwoFact> data, PhysicalValue physicalValue);
}
