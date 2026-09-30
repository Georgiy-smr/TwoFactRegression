namespace TwoFactRegressCalc.Models;

// One line of the «Проверка точек» window. Pressure and CodeError are display text, empty where they don't apply.
public sealed record DatasetReviewRow(
    double NominalTemperature,
    int FirstExcelRow,
    int LastExcelRow,
    string Pressure,
    string Result,
    string CodeError)
{
    public string ExcelRow => FirstExcelRow == LastExcelRow ? $"{FirstExcelRow}" : $"{FirstExcelRow}–{LastExcelRow}";
}
