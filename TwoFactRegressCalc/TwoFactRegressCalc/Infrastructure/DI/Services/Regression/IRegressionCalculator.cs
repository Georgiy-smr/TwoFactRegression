using Regression.Two_factor_regression;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal interface IRegressionCalculator
{
    IEnumerable<TwoFactorRegressionResult> Calculate(IEnumerable<DataTwoFact> data, PhysicalValue physicalValue);
}
