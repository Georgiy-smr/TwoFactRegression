namespace TwoFactRegressCalc.Models;

public sealed record DatasetReviewRow(
    double NominalTemperature,
    int FirstExcelRow,
    string ExcelRow,
    double? Pressure,
    string Result,
    double? CodeError);
