using Regression.Two_factor_regression;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Tests;

public class TwoFactorRegressionResultTests
{
    [Fact]
    public void Constructor_ExposesNameUnchanged()
    {
        var data = new List<DataTwoFact>
        {
            new() { X1 = 1, X2 = 1, Y = 1 },
            new() { X1 = 2, X2 = 1, Y = 2 },
            new() { X1 = 1, X2 = 2, Y = 3 },
        };
        var coefficients = Enumerable.Range(0, 9).Select(i => (double)i).ToList();

        var result = new TwoFactorRegressionResult("QR (normal equations)", coefficients, data);

        Assert.Equal("QR (normal equations)", result.Name);
    }
}
