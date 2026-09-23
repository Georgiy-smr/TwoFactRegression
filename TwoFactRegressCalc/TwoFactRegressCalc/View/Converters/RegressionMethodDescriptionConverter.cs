using System.Globalization;
using System.Windows.Data;

namespace TwoFactRegressCalc.View.Converters;

/// <summary>
/// Converts a TwoFactorRegressionResult.Name (e.g. "Gauss (3rd order)") into a tooltip
/// describing the method's tradeoffs, for the regression result picker.
/// </summary>
public class RegressionMethodDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string name)
            return string.Empty;

        var method = name switch
        {
            _ when name.StartsWith("Gauss") =>
                "Gauss: solves the normal equations by Gaussian elimination. Simple and fast, but forming " +
                "the normal equations squares the matrix's condition number, so it gets numerically shakier as the order grows.",
            _ when name.Contains("normal equations") =>
                "QR (normal equations): solves the same normal equations as Gauss, but via QR decomposition, " +
                "which is more numerically stable than plain elimination — it still inherits the squared condition number from forming them, though.",
            _ when name.Contains("design matrix") =>
                "QR (design matrix): applies QR decomposition directly to the design matrix, without ever forming " +
                "the normal equations. Usually the most numerically stable of the three, especially at higher orders.",
            _ => "Regression method.",
        };

        var order = name switch
        {
            _ when name.Contains("2nd order") || name.Contains("SecondOrderBasisExponents") =>
                " 2nd order, 9 terms: most robust with few points, but may underfit a strongly curved relationship.",
            _ when name.Contains("3rd order") || name.Contains("ThirdOrderBasisExponents") =>
                " 3rd order, 16 terms: more flexible than 2nd order, needs more points to stay well-conditioned.",
            _ when name.Contains("FourthOrderBasisExponents") =>
                " 4th order, 25 terms: most flexible, but also most prone to overfitting — a high order of magnitude " +
                "among the coefficients, or a MaxError that looks unexpectedly large, is a sign this fit won't generalize well.",
            _ => string.Empty,
        };

        return method + order;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
