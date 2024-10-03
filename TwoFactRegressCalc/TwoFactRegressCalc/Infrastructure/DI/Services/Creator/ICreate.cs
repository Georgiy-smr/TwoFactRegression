using Microsoft.Win32;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Creator;

public interface ICreate<in T>
{
    public void Create(string filePath, T data);
    public Task CreateAsync(string filePath, T data, CancellationToken token = default);
}