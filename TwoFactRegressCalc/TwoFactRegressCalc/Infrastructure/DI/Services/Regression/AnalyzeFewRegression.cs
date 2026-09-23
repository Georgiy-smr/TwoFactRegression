using System.Windows;
using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Implements;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFact;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class AnalyzeFewRegression : IRegressionService
{
    private sealed record Variant(string Name, int MinPointCount, Func<List<DataTwoFact>, IEnumerable<double>> Compute);

    private readonly GausAlgorithm _gauss = new();
    private readonly QrFactorizedAlgorithm _qrFactorized = new();
    private readonly IReadOnlyList<Variant> _variants;

    public AnalyzeFewRegression()
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

    public IEnumerable<double> Get(IEnumerable<DataTwoFact> data)
    {
        var dataList = data.ToList();
        var results = new Dictionary<string, TwoFactorRegressionResult>();

        foreach (var variant in _variants)
        {
            if (dataList.Count < variant.MinPointCount)
                continue;

            var coefs = variant.Compute(dataList).ToArray();
            if (coefs.Any(double.IsNaN))
                continue;

            results[variant.Name] = new TwoFactorRegressionResult(coefs, dataList);
        }

        if (results.Count == 0)
        {
            MessageBox.Show("Ошибка расчетов");
            return Array.Empty<double>();
        }

        var best = results.MinBy(kv => kv.Value.MaxError);

        MessageBox.Show(
            $"MaxError: {best.Value.MaxError}\n{string.Join("\n", best.Value.Coefficients.Select((x, i) => $"a{i} : {x}"))}",
            best.Key,
            MessageBoxButton.OK);

        return best.Value.Coefficients;
    }
}
