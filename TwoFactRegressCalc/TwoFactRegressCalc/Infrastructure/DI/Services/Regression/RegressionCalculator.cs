using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Implements;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFact;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class RegressionCalculator : IRegressionCalculator
{
    private sealed record Variant(string Name, int MinPointCount, Func<List<DataTwoFact>, IEnumerable<double>> Compute);

    private readonly GausAlgorithm _gauss = new();
    private readonly QrFactorizedAlgorithm _qrFactorized = new();
    private readonly IReadOnlyList<Variant> _variants;

    public RegressionCalculator()
    {
        _variants = new List<Variant>
        {
            // Coefficient count = (order+1)^2, so that many points are needed at minimum to fit it.
            new("QR (design matrix, FourthOrderBasisExponents)", 25,
                data => new PolynomialLeastSquaresSolver(new FourthOrderBasisExponents()).GetValues(data)),
            new($"{_gauss.Name} (3rd order)", 16,
                data => _gauss.CalcCoefs(data.CreateThirdOrderPolynomialExpression())),
            new($"{_qrFactorized.Name} (3rd order)", 16,
                data => _qrFactorized.CalcCoefs(data.CreateThirdOrderPolynomialExpression())),
            new("QR (design matrix, ThirdOrderBasisExponents)", 16,
                data => new PolynomialLeastSquaresSolver(new ThirdOrderBasisExponents()).GetValues(data)),
            new($"{_gauss.Name} (2nd order)", 9,
                data => _gauss.CalcCoefs(data.CreateTwoOrderPolynomialExpression())),
            new($"{_qrFactorized.Name} (2nd order)", 9,
                data => _qrFactorized.CalcCoefs(data.CreateTwoOrderPolynomialExpression())),
            new("QR (design matrix, SecondOrderBasisExponents)", 9,
                data => new PolynomialLeastSquaresSolver(new SecondOrderBasisExponents()).GetValues(data)),
        };
    }

    public IEnumerable<TwoFactorRegressionResult> Calculate(IEnumerable<DataTwoFact> data)
    {
        var dataList = data.ToList();
        var results = new List<TwoFactorRegressionResult>();

        foreach (var variant in _variants)
        {
            if (dataList.Count < variant.MinPointCount)
                continue;

            var coefs = variant.Compute(dataList).ToArray();
            if (coefs.Any(double.IsNaN))
                continue;

            results.Add(new TwoFactorRegressionResult(variant.Name, coefs, dataList));
        }

        return results;
    }
}
