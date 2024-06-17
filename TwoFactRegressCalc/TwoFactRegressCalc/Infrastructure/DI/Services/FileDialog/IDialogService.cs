namespace TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog;

public interface IDialogService
{
    /// <summary>
    /// Показ сообщения
    /// </summary>
    /// <param name="message"></param>
    void ShowMessage(string message);

    /// <summary>
    /// Наименование файла
    /// </summary>
    string FileName { get; set; }

    /// <summary>
    /// Путь к выбранному файлу
    /// </summary>
    string FilePath { get; set; }

    /// <summary>
    /// Путь к папке с файлом
    /// </summary>
    string FolderPath { get; }

    /// <summary>
    /// Начальная папка
    /// </summary>
    string? InitialDirectory { get; set; }

    /// <summary>
    /// Фильтр
    /// </summary>
    string Filter { get; set; }

    /// <summary>
    /// Открытие файла
    /// </summary>
    /// <returns></returns>
    bool OpenFileDialog();

    /// <summary>
    /// Сохранение файла
    /// </summary>
    /// <returns></returns>
    bool SaveFileDialog();
}