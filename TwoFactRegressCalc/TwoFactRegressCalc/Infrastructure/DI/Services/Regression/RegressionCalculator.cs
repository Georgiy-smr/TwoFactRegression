using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Implements;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFact;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class RegressionCalculator : IRegressionCalculator
{
    private sealed record Variant(string Name, int Order, int MinPointCount, Func<List<DataTwoFact>, IEnumerable<double>> Compute);

    private readonly GausAlgorithm _gauss = new();
    private readonly QrFactorizedAlgorithm _qrFactorized = new();
    private readonly IReadOnlyList<Variant> _variants;

    public RegressionCalculator()
    {
        _variants = new List<Variant>
        {
            // Coefficient count = (order+1)^2, so that many points are needed at minimum to fit it.
            new("QR-разложение матрицы плана (без нормальных уравнений)", 4, 25,
                data => new PolynomialLeastSquaresSolver(new FourthOrderBasisExponents()).GetValues(data)),
            new(_gauss.Name, 4, 25,
                data => _gauss.CalcCoefs(data.CreateFourthOrderPolynomialExpression())),
            new(_qrFactorized.Name, 4, 25,
                data => _qrFactorized.CalcCoefs(data.CreateFourthOrderPolynomialExpression())),
            new(_gauss.Name, 3, 16,
                data => _gauss.CalcCoefs(data.CreateThirdOrderPolynomialExpression())),
            new(_qrFactorized.Name, 3, 16,
                data => _qrFactorized.CalcCoefs(data.CreateThirdOrderPolynomialExpression())),
            new("QR-разложение матрицы плана (без нормальных уравнений)", 3, 16,
                data => new PolynomialLeastSquaresSolver(new ThirdOrderBasisExponents()).GetValues(data)),
            new(_gauss.Name, 2, 9,
                data => _gauss.CalcCoefs(data.CreateTwoOrderPolynomialExpression())),
            new(_qrFactorized.Name, 2, 9,
                data => _qrFactorized.CalcCoefs(data.CreateTwoOrderPolynomialExpression())),
            new("QR-разложение матрицы плана (без нормальных уравнений)", 2, 9,
                data => new PolynomialLeastSquaresSolver(new SecondOrderBasisExponents()).GetValues(data)),
        };
    }

    public IEnumerable<TwoFactorRegressionResult> Calculate(IEnumerable<DataTwoFact> data, PhysicalValue physicalValue)
    {
        var dataList = data.ToList();
        var results = new List<TwoFactorRegressionResult>();

        // Temperature is always fit at 2nd order - the client doesn't need a degree choice for it.
        var applicableVariants = physicalValue == PhysicalValue.Temperature
            ? _variants.Where(v => v.Order == 2)
            : _variants;

        foreach (var variant in applicableVariants)
        {
            if (dataList.Count < variant.MinPointCount)
                continue;

            var coefs = variant.Compute(dataList).ToArray();
            if (coefs.Any(double.IsNaN))
                continue;

            results.Add(new TwoFactorRegressionResult(variant.Name, variant.Order, coefs, dataList));
        }

        return results;
    }
}
