using Regression.OutlierDetection;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

// Implemented by a decorator chain (see ServiceRegistrar.Regression): each step adds its part and calls the next.
internal interface IRegressionCalculator
{
    IReadOnlyList<TwoFactorRegressionResult> Calculate(IReadOnlyList<CalibrationPoint> dataset);
}
