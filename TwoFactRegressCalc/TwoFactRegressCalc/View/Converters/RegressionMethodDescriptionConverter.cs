using System.Globalization;
using System.Windows.Data;
using TwoFactRegressCalc.Models;

namespace TwoFactRegressCalc.View.Converters;

public class RegressionMethodDescriptionConverter : IValueConverter
{
    public const string DefaultDescription = "Метод регрессии.";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TwoFactorRegressionResult candidate)
            return string.Empty;

        var method = candidate.Name switch
        {
            RegressionMethodNames.Gauss =>
                "Гаусс: решает нормальные уравнения методом гауссова исключения. Прост и быстр, но при составлении " +
                "нормальных уравнений число обусловленности матрицы возводится в квадрат, поэтому с ростом степени метод становится менее устойчивым.",
            RegressionMethodNames.QrNormalEquations =>
                "QR (нормальные уравнения): решает те же нормальные уравнения, что и метод Гаусса, но через QR-разложение, " +
                "которое устойчивее прямого исключения — однако унаследует то же возведение числа обусловленности в квадрат при их составлении.",
            RegressionMethodNames.QrDesignMatrix =>
                "QR-разложение матрицы плана: применяет QR-разложение напрямую к матрице плана, не составляя нормальные уравнения. " +
                "Самый устойчивый из методов наименьших квадратов, особенно при высоких степенях. " +
                "Рекомендуется по умолчанию: лучше всех обобщает между температурами калибровки.",
            RegressionMethodNames.Minimax =>
                "Минимакс (алгоритм Лоусона). ➕ Наименьшая максимальная ошибка на точках калибровки " +
                "(на эталонных данных 223/224 примерно на 35% ниже, чем у МНК). " +
                "➖ Между температурами калибровки ошибается сильнее МНК; чувствителен к отдельным шумным точкам. " +
                "Рекомендуется, когда приёмка идёт строго по точкам калибровки.",
            _ => DefaultDescription,
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
