using OfficeOpenXml;
using Regression.OutlierDetection;
using System.IO;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Readers
{
    internal class ExcelFileDataReader : IReadData<CalibrationPoint>
    {
        public async IAsyncEnumerable<CalibrationPoint> ReadAsync(string pathReadingFile)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var excelPackage = new ExcelPackage(new FileInfo(pathReadingFile));
            if (excelPackage.Workbook.Worksheets.FirstOrDefault() is not { } worksheet)
                throw new NotImplementedException();

            if (worksheet.Dimension is not { Rows: > 0 } dimension)
                yield break;

            for (int row = 2; row <= dimension.Rows; row++)
            {
                var point = await Task.Run(() => TryReadCalibrationPoint(worksheet, row));
                if (point is null)
                    yield break;
                yield return point;
            }
        }

        private static CalibrationPoint? TryReadCalibrationPoint(ExcelWorksheet worksheet, int row)
        {
            if (double.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out var pressureCode)
                && double.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var temperatureCode)
                && double.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var pressure)
                && double.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var temperature))
                return new CalibrationPoint(pressureCode, temperatureCode, pressure, temperature);
            return null;
        }
    }
}
