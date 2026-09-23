using System.Windows;
using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFact;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class AnalyzeFewRegression : IRegressionService
{
    private readonly IEnumerable<IRegression<DataTwoFact>> _regressions = new List<IRegression<DataTwoFact>>
    {
        new GausAlgorithm(),
        new QrFactorizedAlgorithm(),
    };

    public IEnumerable<double> Get(
        IEnumerable<DataTwoFact> data,
        Func<IEnumerable<DataTwoFact>, IPolynomialExpression> func,
        params IBasisExponents[] bases)
    {
        var dataList = data.ToList();
        var results = new Dictionary<string, TwoFactorRegressionResult>();

        var expression = func(dataList);
        foreach (var regression in _regressions)
        {
            var coefs = regression.CalcCoefs(expression).ToArray();
            if (coefs.Any(double.IsNaN))
                continue;

            results[regression.Name] = new TwoFactorRegressionResult(coefs, dataList);
        }

        foreach (var basis in bases)
        {
            // PolynomialLeastSquaresSolver fits via MathNet's MultipleRegression.QR on the
            // (centered/scaled) design matrix directly, unlike QrFactorizedAlgorithm which
            // QR-solves the normal equations - distinct enough numerically to label separately.
            var solver = new PolynomialLeastSquaresSolver(basis);
            var coefs = solver.GetValues(dataList).ToArray();
            if (coefs.Any(double.IsNaN))
                continue;

            results[$"QR (design matrix, {basis.GetType().Name})"] = new TwoFactorRegressionResult(coefs, dataList);
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
