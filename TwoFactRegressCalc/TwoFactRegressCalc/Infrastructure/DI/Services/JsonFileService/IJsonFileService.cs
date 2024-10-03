namespace TwoFactRegressCalc.Infrastructure.DI.Services.JsonFileService;

public interface IJsonFileService<T>
{
    /// <summary>
    /// Чтение из файла
    /// </summary>
    /// <param name="filename">имя файла</param>
    /// <returns></returns>
    Task<T> ReadAsync();

    /// <summary>
    /// Запись в файл
    /// </summary>
    /// <param name="filename">имя файла</param>
    /// <param name="data">данные</param>
    Task WriteAsync(T data);
}