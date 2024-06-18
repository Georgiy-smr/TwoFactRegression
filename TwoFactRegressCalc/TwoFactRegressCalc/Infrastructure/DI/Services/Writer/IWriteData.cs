using OfficeOpenXml;
using Regression.Two_factor_regression;
using System.IO;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Writer;

public interface IWriteData<in T>
{
    Task Write(T data, string filePath);
}

public class ExcelFillData : IWriteData<double[]>
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
                worksheet.Cells[row, 4].Value = data[row - 1];
            }
        }).ConfigureAwait(false);
    }
}