using System.IO;
using System.Text.Json;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Creator;

public class CreateFileWithCoefficients : ICreate<CoefficientsBySensor>
{
    public void Create(string filePath, CoefficientsBySensor data)
    {
        string json = JsonSerializer.Serialize(data);
        using FileStream fs = new(filePath, FileMode.Create, FileAccess.Write);
        using StreamWriter writer = new(fs);
        writer.WriteLine(json);
    }

    public async Task CreateAsync(string filePath, CoefficientsBySensor data, CancellationToken token = default)
    {
        var sPath = Path.ChangeExtension(filePath, null);
        await using FileStream fs = new(sPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
        await JsonSerializer.SerializeAsync(fs, data, cancellationToken: token);
    }
}