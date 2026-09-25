namespace TwoFactRegressCalc.Models;

public static class RegressionMethodNames
{
    public const string Gauss = "Гаусс";
    public const string QrNormalEquations = "QR (нормальные уравнения)";
    public const string QrDesignMatrix = "QR-разложение матрицы плана (без нормальных уравнений)";
    public const string Minimax = "Минимакс (алгоритм Лоусона)";
}
