using Regression.OutlierDetection;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Readers;

public interface IReadCalibrationPoints
{
    IAsyncEnumerable<CalibrationPoint> ReadCalibrationPointsAsync(string pathReadingFile);
}
