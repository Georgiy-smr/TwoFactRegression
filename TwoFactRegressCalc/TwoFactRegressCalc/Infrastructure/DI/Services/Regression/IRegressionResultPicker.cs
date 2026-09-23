using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public interface IRegressionResultPicker
{
    TwoFactorRegressionResult Pick(IEnumerable<TwoFactorRegressionResult> candidates, PhysicalValue physicalValue);
}
