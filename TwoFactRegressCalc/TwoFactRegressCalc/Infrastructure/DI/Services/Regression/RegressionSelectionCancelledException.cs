using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public sealed class RegressionSelectionCancelledException : OperationCanceledException
{
    public RegressionSelectionCancelledException(PhysicalValue physicalValue)
        : base($"Regression model selection was cancelled for {physicalValue}.")
    {
        PhysicalValue = physicalValue;
    }

    public PhysicalValue PhysicalValue { get; }
}
