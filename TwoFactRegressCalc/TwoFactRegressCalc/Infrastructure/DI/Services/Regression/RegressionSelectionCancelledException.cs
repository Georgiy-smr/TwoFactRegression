namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public sealed class RegressionSelectionCancelledException : OperationCanceledException
{
    public RegressionSelectionCancelledException(string valueName)
        : base($"Regression model selection was cancelled for {valueName}.")
    {
        ValueName = valueName;
    }

    public string ValueName { get; }
}
