using System.IO;
using Newtonsoft.Json;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.JsonFileService;

public class SettingsJsonFileService
    : IJsonFileService<Config>
{
    /// <summary>
    /// Путь до файла настроек JSON
    /// </summary>
    private readonly string _settingsJsonPath = App.SettingsPath;

    /// <summary>
    /// Чтение из файла
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public async Task<Config> ReadAsync()
    {
        var fileExists = File.Exists(_settingsJsonPath);
        if (!fileExists)
        {
            await File.CreateText(_settingsJsonPath).DisposeAsync();
            return new Config();
        }

        using var reader = File.OpenText(_settingsJsonPath);

        var fileText = await reader.ReadToEndAsync();
        var settings = JsonConvert.DeserializeObject<Config>(fileText);
        return settings ?? new Config();
    }

    /// <summary>
    /// Запись в файл
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="data"></param>
    public async Task WriteAsync(Config data)
    {
        await using var writer = File.CreateText(_settingsJsonPath);
        var output = JsonConvert.SerializeObject(data);
        await writer.WriteAsync(output).ConfigureAwait(false);
    }

}