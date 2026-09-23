using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public interface IRegressionResultPicker
{
    /// <exception cref="RegressionSelectionCancelledException">The user cancelled/closed the picker without confirming.</exception>
    TwoFactorRegressionResult Pick(IEnumerable<TwoFactorRegressionResult> candidates, PhysicalValue physicalValue);
}
