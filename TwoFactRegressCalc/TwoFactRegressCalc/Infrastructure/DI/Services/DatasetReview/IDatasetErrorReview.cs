using Regression.OutlierDetection;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.DatasetReview;

public interface IDatasetErrorReview
{
    void Review(IReadOnlyList<CalibrationPoint> dataset, double accuracyClassPercent);
}
