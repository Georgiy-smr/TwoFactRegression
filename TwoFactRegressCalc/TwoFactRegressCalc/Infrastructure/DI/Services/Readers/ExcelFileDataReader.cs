using Regression.Two_factor_regression;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Readers
{
    internal class ExcelFileDataReader : IReadData<DataTwoFact>
    {
        public async IAsyncEnumerable<DataTwoFact> ReadAsync(string pathReadingFile, PhysicalValue tValue)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var excelPackage = new ExcelPackage(new FileInfo(pathReadingFile));
            if (excelPackage.Workbook.Worksheets.FirstOrDefault() is not { } sheetMainParams)
                throw new NotImplementedException();

            int x1 = tValue switch
            {
                PhysicalValue.Pressure => 1,
                PhysicalValue.Temperature => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(tValue), tValue, null)
            };
            int x2 = tValue switch
            {
                PhysicalValue.Pressure => 2,
                PhysicalValue.Temperature => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(tValue), tValue, null)
            };
            int y = tValue switch
            {
                PhysicalValue.Pressure => 3,
                PhysicalValue.Temperature => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(tValue), tValue, null)
            };
            await foreach (var dataTwoFact in ReadWorksheetAsync(sheetMainParams, x1Column:x1, x2Column:x2, yColumn:y))
                yield return dataTwoFact;
        }

        private async IAsyncEnumerable<DataTwoFact> ReadWorksheetAsync(ExcelWorksheet worksheet,
                                                                        int x1Column = 1,
                                                                        int x2Column = 2, 
                                                                        int yColumn = 3)
        {
            int rowCount = worksheet.Dimension.Rows;
                if(rowCount <= 0)
                    yield break;
                bool resultTryParseX1 = false;
                bool resultTryParseX2 = false;
                bool resultTryParseY = false;
                double currentX1 = 0;
                double currentX2 = 0;
                double currentY = 0;
                for (int row = 2; row <= rowCount; row++)
                {
                    await Task.Run(() =>
                    {
                        resultTryParseX1 = double.TryParse(worksheet.Cells[row, x1Column].Value?.ToString(), out currentX1);
                        resultTryParseX2 = double.TryParse(worksheet.Cells[row, x2Column].Value?.ToString(), out currentX2);
                        resultTryParseY = double.TryParse(worksheet.Cells[row, yColumn].Value?.ToString(), out currentY);
                    });
                    if (resultTryParseX1 && resultTryParseX2 && resultTryParseY)
                        yield return new DataTwoFact() { X1 = currentX1, X2 = currentX2, Y = currentY };
                    else yield break;
                }
        }


    }
}
