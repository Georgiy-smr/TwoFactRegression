using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public interface IRegressionResultPicker
{
    /// <exception cref="RegressionSelectionCancelledException">The user cancelled/closed the picker without confirming.</exception>
    TwoFactorRegressionResult Pick(IEnumerable<TwoFactorRegressionResult> candidates, string physicalValueLabel);
}
