using Regression.OutlierDetection;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

// Innermost step of the chain: nothing left to calculate.
internal class RegressionChainEnd : IRegressionCalculator
{
    public IReadOnlyList<TwoFactorRegressionResult> Calculate(IReadOnlyList<CalibrationPoint> dataset) => [];
}
