namespace TwoFactRegressCalc.Infrastructure.DI.Services.Readers;

public interface IReadData<out T>
{
    IAsyncEnumerable<T> ReadAsync(string pathReadingFile);
}
