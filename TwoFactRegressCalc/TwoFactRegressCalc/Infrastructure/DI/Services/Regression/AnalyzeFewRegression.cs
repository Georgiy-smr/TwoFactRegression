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
        IBasisExponents basis)
    {
        var dataList = data.ToList();
        var results = new Dictionary<Type, TwoFactorRegressionResult>();

        var expression = func(dataList);
        foreach (var regression in _regressions)
        {
            var coefs = regression.CalcCoefs(expression).ToArray();
            if (coefs.Any(double.IsNaN))
                continue;

            results[regression.GetType()] = new TwoFactorRegressionResult(coefs, dataList);
        }

        var solver = new PolynomialLeastSquaresSolver(basis);
        var leastSquaresCoefs = solver.GetValues(dataList).ToArray();
        if (!leastSquaresCoefs.Any(double.IsNaN))
            results[solver.GetType()] = new TwoFactorRegressionResult(leastSquaresCoefs, dataList);

        if (results.Count == 0)
        {
            MessageBox.Show("Ошибка расчетов");
            return Array.Empty<double>();
        }

        var best = results.OrderBy(kv => kv.Value.MaxError).First();

        MessageBox.Show(
            $"MaxError: {best.Value.MaxError}\n{string.Join("\n", best.Value.Coefficients.Select((x, i) => $"a{i} : {x}"))}",
            best.Key.Name,
            MessageBoxButton.OK);

        return best.Value.Coefficients;
    }
}
