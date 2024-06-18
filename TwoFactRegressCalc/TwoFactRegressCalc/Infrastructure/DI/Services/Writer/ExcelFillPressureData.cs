using System.IO;
using OfficeOpenXml;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Writer;

public class ExcelFillPressureAndTempData : IWriteData<IEnumerable<double[]>>
{
    public async Task Write(IEnumerable<double[]> data, string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var excelPackage = new ExcelPackage(new FileInfo(filePath));
        if (excelPackage.Workbook.Worksheets.FirstOrDefault() is not { } sheetMainParams)
            throw new NotImplementedException();
        await FillPressure(sheetMainParams, data.First());
        await FillTemperature(sheetMainParams, data.Last());
        await excelPackage.SaveAsync();
    }
    private async Task FillPressure(ExcelWorksheet worksheet, double[] data)
    {
        await Task.Run(() =>
        {
            for (int row = 1; row <= data.Length; row++)
            {
                worksheet.Cells[row + 1, 5].Value = data[row - 1];
            }
        }).ConfigureAwait(false);
    }
    private async Task FillTemperature(ExcelWorksheet worksheet, double[] data)
    {
        await Task.Run(() =>
        {
            for (int row = 1; row <= data.Length; row++)
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