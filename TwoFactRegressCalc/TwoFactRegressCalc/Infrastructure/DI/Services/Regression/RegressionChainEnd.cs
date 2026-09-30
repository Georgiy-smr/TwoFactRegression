using Regression.OutlierDetection;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

// Innermost step of the chain: no coefficients yet - the steps around it fill them in.
internal class RegressionChainEnd : IRegressionCalculator
{
    public SensorCoefficientsResult Calculate(IReadOnlyList<CalibrationPoint> dataset) => new([], []);
}
