using Microsoft.Extensions.DependencyInjection;
using TwoFactRegressCalc.Infrastructure.DI;
using TwoFactRegressCalc.Infrastructure.DI.Services.DatasetReview;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

namespace TwoFactRegressCalc.Tests;

public class ServiceRegistrarTests
{
    [Fact]
    public void Regression_ResolvesChainWithDatasetReviewAsOutermostStep()
    {
        var services = new ServiceCollection();
        services.Regression();
        services.JsonFileService();
        services.AddLogging();

        using var provider = services.BuildServiceProvider();

        Assert.IsType<DatasetReviewStep>(provider.GetRequiredService<IRegressionCalculator>());
    }
}
