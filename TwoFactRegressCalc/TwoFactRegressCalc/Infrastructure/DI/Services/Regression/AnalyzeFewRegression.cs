using System.CodeDom;
using System.Windows;
using Regression.Two_factor_regression;
using Regression.Two_factor_regression.Interfaces;
using TwoFactRegressCalc.Infrastructure.DI.Services.Regression.TwoFact;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

internal class AnalyzeFewRegression : IRegressionService
{
    private readonly IEnumerable<IRegression<DataTwoFact>> _regressions;

    public AnalyzeFewRegression(IEnumerable<IRegression<DataTwoFact>> regressions)
    {
        _regressions = regressions;
    }

    public IEnumerable<double> Get(IEnumerable<DataTwoFact> data, Func<IEnumerable<DataTwoFact>, IPolynomialExpression> func)
    {
        IEnumerable<double> coefs = default!;
        double maxErrorRegress = 0;

        Type typeRegression = typeof(object);

        foreach (var regression in _regressions)
        {
            var tempCoefs = regression.CalcCoefs(func(data));

            if(tempCoefs.Any(double.IsNaN))
                continue;

            var tempMax = GetMaxErrorRegress(data, tempCoefs.ToArray());

            if (maxErrorRegress != 0 && maxErrorRegress <= tempMax) 
                continue;

            typeRegression = regression.GetType();

            maxErrorRegress = tempMax;
            coefs = tempCoefs;
        }
        if(coefs is null) 
            MessageBox.Show("Ошибка расчетов");

        string caption = string.Empty;

        if (typeRegression == typeof(QrFactorizedAlgorithm))
        {
            caption = "QrFactorizedAlgorithm";
        }
        if (typeRegression == typeof(GausAlgorithm))
        {
            caption = "GausAlgorithm";
        }
        MessageBox.Show($"MaxError: {maxErrorRegress}\n{string.Join("\n", coefs.Select((x, i) => $"a{i} : {x}"))}", caption, MessageBoxButton.OK);
        return coefs;
    }

    private double GetMaxErrorRegress(IEnumerable<DataTwoFact> data, double[] coeffs)
    {
        return data.Select(x1x2y => CalcResDelta(x1x2y, coeffs.ToArray())).Max();
    }

    private double CalcResDelta(DataTwoFact data, double[] resultCheckedCoef)
    {
        double res = 0;
        
        if(resultCheckedCoef.Length == 16)
            for (int i = 0; i < resultCheckedCoef.Count(); i++) 
            { 
                switch (i) 
                { 
                    case 0:
                        res += resultCheckedCoef[i];
                        break;
                    case 1:
                        //a1* item.X2
                        res += resultCheckedCoef[i] * data.X2;
                        break;
                    case 2:
                        //a2* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2;
                        break;
                    case 3:
                        //a3* item.X1 +
                        res += resultCheckedCoef[i] * data.X1;
                        break;
                    case 4:
                        //   a4 * item.X1 * item.X1 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1;
                        break;
                    case 5:
                        //a5 * item.X1 * item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X2;
                        break;
                    case 6:
                        // a6* item.X2* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X1 * data.X1;
                        break;
                    case 7:
                        //a7* item.X2* item.X2* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X1;
                        break;
                    case 8:
                        //a8* item.X1* item.X1* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2 * data.X2;
                        break;
                    case 9:
                        //a9* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1;
                        break;
                    case 10:
                        //a10* item.X2* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                    case 11:
                        //a11* item.X2* item.X2* item.X1* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                    case 12:
                        //a12* item.X2* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2;
                        break;
                    case 13:
                        //a13* item.X2* item.X2* item.X2* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1;
                        break;
                    case 14:
                        //a14* item.X2* item.X2* item.X2* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1 * data.X1;
                        break;
                    case 15:
                        //a15* item.X2* item.X2* item.X2* item.X1* item.X1* item.X1);
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X1 * data.X1 * data.X1;
                        break;
                }
            }
        else if (resultCheckedCoef.Length == 9)
        {
            for (int i = 0; i < resultCheckedCoef.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        res += resultCheckedCoef[i];
                        break;
                    case 1:
                        //a1* item.X2
                        res += resultCheckedCoef[i] * data.X2;
                        break;
                    case 2:
                        //a2* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2;
                        break;
                    case 3:
                        //a3* item.X1 +
                        res += resultCheckedCoef[i] * data.X1;
                        break;
                    case 4:
                        //   a4 * item.X1 * item.X1 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1;
                        break;
                    case 5:
                        //a5 * item.X1 * item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X2;
                        break;
                    case 6:
                        // a6* item.X2* item.X1* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X1 * data.X1;
                        break;
                    case 7:
                        //a7* item.X2* item.X2* item.X1 +
                        res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X1;
                        break;
                    case 8:
                        //a8* item.X1* item.X1* item.X2* item.X2 +
                        res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2 * data.X2;
                        break;
                }
            }
        }
        else if (resultCheckedCoef.Length == 25)
        {
            for (int i = 0; i < resultCheckedCoef.Count(); i++)
            {
                switch (i)
                {
                    case 0: res += resultCheckedCoef[i]; break;
                    case 1: res += resultCheckedCoef[i] * data.X2; break;
                    // X2
                    case 2: res += resultCheckedCoef[i] * data.X2 * data.X2; break;
                    // X2^2
                    case 3: res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2; break;
                    // X2^3
                    case 4: res += resultCheckedCoef[i] * data.X2 * data.X2 * data.X2 * data.X2; break;
                    // X2^4
                               
                    case 5: res += resultCheckedCoef[i] * data.X1; break;
                    // X1
                    case 6: res += resultCheckedCoef[i] * data.X1 * data.X2; break;
                    // X1*X2
                    case 7: res += resultCheckedCoef[i] * data.X1 * data.X2 * data.X2; break;
                    // X1*X2^2
                    case 8: res += resultCheckedCoef[i] * data.X1 * data.X2 * data.X2 * data.X2; break; // X1*X2^3
                    case 9: res += resultCheckedCoef[i] * data.X1 * data.X2 * data.X2 * data.X2 * data.X2; break; // X1*X2^4
                                                                                                                  // i = 2 (X1^2)
                    case 10: res += resultCheckedCoef[i] * data.X1 * data.X1; break; // X1^2
                    case 11: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2; break; // X1^2*X2
                    case 12: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2 * data.X2; break; // X1^2*X2^2
                    case 13: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2 * data.X2 * data.X2; break; // X1^2*X2^3
                    case 14: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X2 * data.X2 * data.X2 * data.X2; break; // X1^2*X2^4
                                                                                                                             // i = 3 (X1^3)
                    case 15: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1; break; // X1^3
                    case 16: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X2; break; // X1^3*X2
                    case 17: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X2 * data.X2; break; // X1^3*X2^2
                    case 18: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X2 * data.X2 * data.X2; break; // X1^3*X2^3
                    case 19: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X2 * data.X2 * data.X2 * data.X2; break; // X1^3*X2^4
                                                                                                                                       // i = 4 (X1^4)
                    case 20: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X1; break; // X1^4
                    case 21: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X1 * data.X2; break; // X1^4*X2
                    case 22: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X1 * data.X2 * data.X2; break; // X1^4*X2^2
                    case 23: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X1 * data.X2 * data.X2 * data.X2; break; // X1^4*X2^3
                    case 24: res += resultCheckedCoef[i] * data.X1 * data.X1 * data.X1 * data.X1 * data.X2 * data.X2 * data.X2 * data.X2; break; // X1^4*X2^4
                }
            }
        }
        return Math.Abs(data.Y - res);
    }



}