namespace TwoFactRegressCalc.Infrastructure.DI.Services.DatasetReview;

public sealed class DatasetReviewCancelledException : OperationCanceledException
{
    public DatasetReviewCancelledException()
        : base("Calculation was cancelled after the calibration dataset review.")
    {
    }
}
