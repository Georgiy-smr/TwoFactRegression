using System.Globalization;
using System.Windows.Data;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.View.Converters;

public class RegressionMethodDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TwoFactorRegressionResult candidate)
            return string.Empty;

        var method = candidate.Name switch
        {
            "Гаусс" =>
                "Гаусс: решает нормальные уравнения методом гауссова исключения. Прост и быстр, но при составлении " +
                "нормальных уравнений число обусловленности матрицы возводится в квадрат, поэтому с ростом степени метод становится менее устойчивым.",
            "QR (нормальные уравнения)" =>
                "QR (нормальные уравнения): решает те же нормальные уравнения, что и метод Гаусса, но через QR-разложение, " +
                "которое устойчивее прямого исключения — однако унаследует то же возведение числа обусловленности в квадрат при их составлении.",
            "QR-разложение матрицы плана (без нормальных уравнений)" =>
                "QR-разложение матрицы плана: применяет QR-разложение напрямую к матрице плана, не составляя нормальные уравнения. " +
                "Обычно самый устойчивый из трёх методов, особенно при высоких степенях.",
            _ => "Метод регрессии.",
        };

        var degree = candidate.Degree switch
        {
            2 => " 2-я степень, 9 коэффициентов: наиболее устойчив при малом числе точек, но может не уловить сильно нелинейную зависимость (недообучение).",
            3 => " 3-я степень, 16 коэффициентов: более гибкая модель, чем 2-я степень, требует больше точек для устойчивости.",
            4 => " 4-я степень, 25 коэффициентов: самая гибкая модель, но и наиболее склонная к переобучению — большой порядок величины " +
                 "коэффициентов или неожиданно большая MaxError говорят о том, что модель плохо обобщается.",
            _ => string.Empty,
        };

        return method + degree;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
