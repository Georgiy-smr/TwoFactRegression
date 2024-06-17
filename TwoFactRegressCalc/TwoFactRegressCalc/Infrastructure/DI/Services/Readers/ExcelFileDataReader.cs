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
        public async IAsyncEnumerable<DataTwoFact> ReadAsync(string pathReadingFile)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var excelPackage = new ExcelPackage(new FileInfo(pathReadingFile));
            if (excelPackage.Workbook.Worksheets.FirstOrDefault() is not { } sheetMainParams)
                throw new NotImplementedException();
            var massiv = ReadWorksheetAsync(sheetMainParams);
            //var list = new List<DataTwoFact>();
            await foreach (var VARIABLE in massiv)
                yield return VARIABLE;
        
        }

        private async IAsyncEnumerable<DataTwoFact> ReadWorksheetAsync(ExcelWorksheet worksheet)
        {
          
                int rowCount = worksheet.Dimension.Rows;

                bool resultTryParseX1 = false;
                bool resultTryParseX2 = false;
                bool resultTryParseY = false;
                double currentX1 = 0;
                double currentX2 = 0;
                double currentY = 0;
                for (int row = 1; row <= rowCount; row++)
                {
                    await Task.Run(() =>
                    {
                        resultTryParseX1 = double.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out currentX1);
                        resultTryParseX2 = double.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out currentX2);
                        resultTryParseY = double.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out currentY);
                    });
                    if (resultTryParseX1 && resultTryParseX2 && resultTryParseY)
                        yield return new DataTwoFact() { X1 = currentX1, X2 = currentX2, Y = currentY };
                    else yield break;
                }
        }


    }
}
