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

            var points = await Task.Run(() =>
            {
                var read = new List<CalibrationPoint>();
                // The first row that doesn't parse ends the dataset.
                for (int row = 2; row <= dimension.Rows && TryReadRow(worksheet, row, out var v); row++)
                    read.Add(new CalibrationPoint(v[0], v[1], v[2], v[3]));
                return read;
            });

            foreach (var point in points)
                yield return point;
        }

        // Columns A-D: pressure code, temperature code, pressure, temperature.
        private bool TryReadRow(ExcelWorksheet worksheet, int row, out double[] values)
        {
            values = new double[4];
            for (int column = 0; column < values.Length; column++)
            {
                if (!double.TryParse(Convert.ToString(worksheet.Cells[row, column + 1].Value), out values[column]))
                    return false;
            }
            return true;
        }
    }
}
