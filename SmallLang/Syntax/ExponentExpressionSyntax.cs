using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ExponentExpressionSyntax : BinaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ExponentExpression;
        public override SmallType Type => SmallType.Double;
        public ExponentExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence, pLeft, pRight) { }

        public override void Emit(ILRunner pRunner)
        {
            Left.Emit(pRunner);
            Right.Emit(pRunner);

            var m = typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) });
            pRunner.Emitter.Emit(OpCodes.Call, m);
        }
    }
}
