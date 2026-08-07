using MathNet.Symbolics;
using Regression.Two_factor_regression.Implements;
using Regression.Two_factor_regression.Interfaces;
using Regression.Two_factor_regression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFactRegressCalc.Extansions.TwoFactExpression
{
    internal static class ExpressionCreator
    {
        public static IPolynomialExpression CreateTwoOrderPolynomialExpression(
     this IEnumerable<DataTwoFact> dataTwoFacts)
        {
            if (dataTwoFacts == null)
                throw new ArgumentNullException(nameof(dataTwoFacts));
            if (dataTwoFacts.Count<DataTwoFact>() < 9)
                throw new ArgumentOutOfRangeException(nameof(dataTwoFacts));
            SymbolicExpression[] varExpr = new List<string>()
      { "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8"
      }.Select<string, SymbolicExpression>(new Func<string, SymbolicExpression>(SymbolicExpression.Variable)).ToArray<SymbolicExpression>();
            return (IPolynomialExpression)new VariableExpression(dataTwoFacts.Select<DataTwoFact, SymbolicExpression>((Func<DataTwoFact, SymbolicExpression>)(item => (SymbolicExpression)item.Y + 
                (varExpr[0] +
                 varExpr[1] * (SymbolicExpression)item.X2 +
                 varExpr[2] * (SymbolicExpression)item.X2 * (SymbolicExpression)item.X2 +
                 varExpr[3] * (SymbolicExpression)item.X1 +
                 varExpr[4] * (SymbolicExpression)item.X1 * (SymbolicExpression)item.X1 +
                 varExpr[5] * (SymbolicExpression)item.X1 * (SymbolicExpression)item.X2 +
                 varExpr[6] * (SymbolicExpression)item.X2 * (SymbolicExpression)item.X1 * (SymbolicExpression)item.X1 +
                 varExpr[7] * (SymbolicExpression)item.X2 * (SymbolicExpression)item.X2 * (SymbolicExpression)item.X1 +
                 varExpr[8] * (SymbolicExpression)item.X1 * (SymbolicExpression)item.X1 * (SymbolicExpression)item.X2 * (SymbolicExpression)item.X2)
                )).Aggregate<SymbolicExpression, SymbolicExpression>((SymbolicExpression)0, (Func<SymbolicExpression, SymbolicExpression, SymbolicExpression>)((current, expr) => current + expr * expr)), (IEnumerable<SymbolicExpression>)varExpr);
        }
        public static IPolynomialExpression CreateFourthOrderPolynomialExpression(
    this IEnumerable<DataTwoFact> dataTwoFacts)
        {
            if (dataTwoFacts == null) throw new ArgumentNullException(nameof(dataTwoFacts));
            if (dataTwoFacts.Count() < 25)
                throw new ArgumentOutOfRangeException(nameof(dataTwoFacts));

            var a0 = SymbolicExpression.Variable("a0");
            var a1 = SymbolicExpression.Variable("a1");
            var a2 = SymbolicExpression.Variable("a2");
            var a3 = SymbolicExpression.Variable("a3");
            var a4 = SymbolicExpression.Variable("a4");
            var a5 = SymbolicExpression.Variable("a5");
            var a6 = SymbolicExpression.Variable("a6");
            var a7 = SymbolicExpression.Variable("a7");
            var a8 = SymbolicExpression.Variable("a8");
            var a9 = SymbolicExpression.Variable("a9");
            var a10 = SymbolicExpression.Variable("a10");
            var a11 = SymbolicExpression.Variable("a11");
            var a12 = SymbolicExpression.Variable("a12");
            var a13 = SymbolicExpression.Variable("a13");
            var a14 = SymbolicExpression.Variable("a14");
            var a15 = SymbolicExpression.Variable("a15");
            var a16 = SymbolicExpression.Variable("a16");
            var a17 = SymbolicExpression.Variable("a17");
            var a18 = SymbolicExpression.Variable("a18");
            var a19 = SymbolicExpression.Variable("a19");
            var a20 = SymbolicExpression.Variable("a20");
            var a21 = SymbolicExpression.Variable("a21");
            var a22 = SymbolicExpression.Variable("a22");
            var a23 = SymbolicExpression.Variable("a23");
            var a24 = SymbolicExpression.Variable("a24");
            var varExpr = new[] 
            { 
                a0,  a1,  a2,  a3,  a4, 
                a5,  a6,  a7,  a8,  a9, 
                a10, a11, a12, a13, a14, 
                a15, a16, a17, a18, a19, 
                a20, a21, a22, a23, a24
            };

            var poly = dataTwoFacts
                .Select(item =>
                    item.Y
                    // i = 0
                    + a0
                    + a1 * item.X2
                    + a2 * item.X2 * item.X2
                    + a3 * item.X2 * item.X2 * item.X2
                    + a4 * item.X2 * item.X2 * item.X2 * item.X2
                    // i = 1
                    + a5 * item.X1
                    + a6 * item.X1 * item.X2
                    + a7 * item.X1 * item.X2 * item.X2
                    + a8 * item.X1 * item.X2 * item.X2 * item.X2
                    + a9 * item.X1 * item.X2 * item.X2 * item.X2 * item.X2
                    // i = 2
                    + a10 * item.X1 * item.X1
                    + a11 * item.X1 * item.X1 * item.X2
                    + a12 * item.X1 * item.X1 * item.X2 * item.X2
                    + a13 * item.X1 * item.X1 * item.X2 * item.X2 * item.X2
                    + a14 * item.X1 * item.X1 * item.X2 * item.X2 * item.X2 * item.X2
                    // i = 3
                    + a15 * item.X1 * item.X1 * item.X1
                    + a16 * item.X1 * item.X1 * item.X1 * item.X2
                    + a17 * item.X1 * item.X1 * item.X1 * item.X2 * item.X2
                    + a18 * item.X1 * item.X1 * item.X1 * item.X2 * item.X2 * item.X2
                    + a19 * item.X1 * item.X1 * item.X1 * item.X2 * item.X2 * item.X2 * item.X2
                    // i = 4
                    + a20 * item.X1 * item.X1 * item.X1 * item.X1
                    + a21 * item.X1 * item.X1 * item.X1 * item.X1 * item.X2
                    + a22 * item.X1 * item.X1 * item.X1 * item.X1 * item.X2 * item.X2
                    + a23 * item.X1 * item.X1 * item.X1 * item.X1 * item.X2 * item.X2 * item.X2
                    + a24 * item.X1 * item.X1 * item.X1 * item.X1 * item.X2 * item.X2 * item.X2 * item.X2)
                .Aggregate<SymbolicExpression?, SymbolicExpression>(0, (current, expr) => current + expr * expr);

            return new VariableExpression(poly, varExpr);
        }
    }
}
