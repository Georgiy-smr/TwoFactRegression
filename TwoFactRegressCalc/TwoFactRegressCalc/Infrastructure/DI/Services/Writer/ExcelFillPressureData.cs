using System.IO;
using OfficeOpenXml;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Writer;

public class ExcelFillPressureAndTempData : IWriteData<AllSensorCoefficients>
{
    public async Task Write(AllSensorCoefficients data, string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var excelPackage = new ExcelPackage(new FileInfo(filePath));
        if (excelPackage.Workbook.Worksheets.FirstOrDefault() is not { } sheetMainParams)
            throw new NotImplementedException();
        await FillPressure(sheetMainParams, data.PressureCoefficients);
        await FillTemperature(sheetMainParams, data.TemperatureCoefficients);
        await excelPackage.SaveAsync();
    }
    private async Task FillPressure(ExcelWorksheet worksheet, IReadOnlyList<double> data)
    {
        await Task.Run(() =>
        {
            for (int row = 1; row <= data.Count; row++)
            {
                worksheet.Cells[row + 1, 5].Value = data[row - 1];
            }
        }).ConfigureAwait(false);
    }
    private async Task FillTemperature(ExcelWorksheet worksheet, IReadOnlyList<double> data)
    {
        await Task.Run(() =>
        {
            for (int row = 1; row <= data.Count; row++)
            {
                worksheet.Cells[row + 1, 6].Value = data[row - 1];
            }
        }).ConfigureAwait(false);
    }
}


public class ExcelFillPressureData : IWriteData<double[]>
{
    
    public async Task Write(double[] data, string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var excelPackage = new ExcelPackage(new FileInfo(filePath));
        if (excelPackage.Workbook.Worksheets.FirstOrDefault() is not { } sheetMainParams)
            throw new NotImplementedException();
        await Fill(sheetMainParams, data);
        await excelPackage.SaveAsync();
    }

    private async Task Fill (ExcelWorksheet worksheet, double[] data)
    {
        await Task.Run(() =>
        {
            for (int row = 1; row <= data.Length; row++)
            {
                worksheet.Cells[row + 1, 5].Value = data[row - 1];
            }
        }).ConfigureAwait(false);
    }
}