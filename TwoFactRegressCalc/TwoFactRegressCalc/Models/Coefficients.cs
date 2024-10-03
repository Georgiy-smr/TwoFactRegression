using System.Globalization;

namespace TwoFactRegressCalc.Models;

public class Coefficients
{
   
    public string a0 { get; set; }
    public string a1 { get; set; }
    public string a2 { get; set; }
    public string a3 { get; set; }
    public string a4 { get; set; }
    public string a5 { get; set; }
    public string a6 { get; set; }
    public string a7 { get; set; }
    public string a8 { get; set; }
    public string a9 { get; set; }
    public string a10 { get; set; }
    public string a11 { get; set; }
    public string a12 { get; set; }
    public string a13 { get; set; }
    public string a14 { get; set; }
    public string a15 { get; set; }

    public string b0 { get; set; }
    public string b1 { get; set; }
    public string b2 { get; set; }
    public string b3 { get; set; }
    public string b4 { get; set; }
    public string b5 { get; set; }
    public string b6 { get; set; }
    public string b7 { get; set; }
    public string b8 { get; set; }



    public Coefficients(double a0, double a1, double a2, double a3, double a4, double a5, double a6, double a7, double a8, double a9, double a10, double a11, double a12, double a13, double a14, double a15, double b0, double b1, double b2, double b3, double b4, double b5, double b6, double b7, double b8)
    {
        this.a0 = a0.ToString("F25", CultureInfo.InvariantCulture);
        this.a1 = a1.ToString("F25", CultureInfo.InvariantCulture);
        this.a2 = a2.ToString("F25", CultureInfo.InvariantCulture);
        this.a3 = a3.ToString("F25", CultureInfo.InvariantCulture);
        this.a4 = a4.ToString("F25", CultureInfo.InvariantCulture);
        this.a5 = a5.ToString("F25", CultureInfo.InvariantCulture);
        this.a6 = a6.ToString("F25", CultureInfo.InvariantCulture);
        this.a7 = a7.ToString("F25", CultureInfo.InvariantCulture);
        this.a8 = a8.ToString("F25", CultureInfo.InvariantCulture);
        this.a9 = a9.ToString("F25", CultureInfo.InvariantCulture);
        this.a10 = a10.ToString("F25", CultureInfo.InvariantCulture);
        this.a11 = a11.ToString("F25", CultureInfo.InvariantCulture);
        this.a12 = a12.ToString("F25", CultureInfo.InvariantCulture);
        this.a13 = a13.ToString("F25", CultureInfo.InvariantCulture);
        this.a14 = a14.ToString("F25", CultureInfo.InvariantCulture);
        this.a15 = a15.ToString("F25", CultureInfo.InvariantCulture);
        this.b0 = b0.ToString("F25", CultureInfo.InvariantCulture);
        this.b1 = b1.ToString("F25", CultureInfo.InvariantCulture);
        this.b2 = b2.ToString("F25", CultureInfo.InvariantCulture);
        this.b3 = b3.ToString("F25", CultureInfo.InvariantCulture);
        this.b4 = b4.ToString("F25", CultureInfo.InvariantCulture);
        this.b5 = b5.ToString("F25", CultureInfo.InvariantCulture);
        this.b6 = b6.ToString("F25", CultureInfo.InvariantCulture);
        this.b7 = b7.ToString("F25", CultureInfo.InvariantCulture);
        this.b8 = b8.ToString("F25", CultureInfo.InvariantCulture);
    }
}