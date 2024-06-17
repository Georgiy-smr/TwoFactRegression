using Regression.Two_factor_regression.Interfaces;
using LibImplements = Regression.Two_factor_regression.Implements;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal interface IRegression<in T> where T : struct
{
    IEnumerable<double> CalcCoefs(IPolynomialExpression regressionData);
}