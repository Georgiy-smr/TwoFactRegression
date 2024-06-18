using Regression.Two_factor_regression;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Readers;

public interface IReadData<T>
{
    IAsyncEnumerable<DataTwoFact> ReadAsync(string pathReadingFile, PhysicalValue targetValue);
}