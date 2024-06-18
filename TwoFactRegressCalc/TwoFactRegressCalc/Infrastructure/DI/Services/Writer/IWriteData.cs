using Regression.Two_factor_regression;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Writer;

public interface IWriteData<in T>
{
    Task Write(T data, string filePath);
}