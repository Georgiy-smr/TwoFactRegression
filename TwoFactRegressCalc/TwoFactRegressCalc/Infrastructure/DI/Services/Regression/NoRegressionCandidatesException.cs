namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public sealed class NoRegressionCandidatesException : Exception
{
    public NoRegressionCandidatesException(string valueName, int pointCount)
        : base($"Не удалось рассчитать коэффициенты ({valueName}): недостаточно точек ({pointCount}).")
    {
        ValueName = valueName;
    }

    public string ValueName { get; }
}
