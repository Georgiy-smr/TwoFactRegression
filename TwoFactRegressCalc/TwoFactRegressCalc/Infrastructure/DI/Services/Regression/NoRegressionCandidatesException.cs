using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public sealed class NoRegressionCandidatesException : Exception
{
    public NoRegressionCandidatesException(PhysicalValue physicalValue, int pointCount)
        : base($"Не удалось рассчитать коэффициенты ({(physicalValue == PhysicalValue.Pressure ? "давление" : "температура")}): " +
               $"недостаточно точек ({pointCount}).")
    {
        PhysicalValue = physicalValue;
    }

    public PhysicalValue PhysicalValue { get; }
}
