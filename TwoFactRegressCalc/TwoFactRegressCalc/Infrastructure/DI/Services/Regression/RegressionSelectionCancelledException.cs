namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public sealed class RegressionSelectionCancelledException : Exception
{
    public RegressionSelectionCancelledException(string physicalValueLabel)
        : base($"Regression model selection was cancelled for {physicalValueLabel}.")
    {
        PhysicalValueLabel = physicalValueLabel;
    }

    public string PhysicalValueLabel { get; }
}
