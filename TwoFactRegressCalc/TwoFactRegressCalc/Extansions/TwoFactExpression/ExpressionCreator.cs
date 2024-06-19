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
    }
}
