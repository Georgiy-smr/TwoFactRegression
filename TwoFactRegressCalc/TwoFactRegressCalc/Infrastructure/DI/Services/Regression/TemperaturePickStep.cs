using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class TemperaturePickStep : RegressionPickStep
{
    public TemperaturePickStep(RegressionCandidateCalculator candidates, IRegressionResultPicker picker, IRegressionCalculator next)
        : base(PhysicalValue.Temperature, candidates, picker, next)
    {
    }
}
