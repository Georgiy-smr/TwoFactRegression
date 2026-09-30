using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class PressurePickStep : RegressionPickStep
{
    public PressurePickStep(RegressionCandidateCalculator candidates, IRegressionResultPicker picker, IRegressionCalculator next)
        : base(PhysicalValue.Pressure, candidates, picker, next)
    {
    }
}
