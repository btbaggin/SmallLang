using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class NegateExpressionSyntax : UnaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NegateExpression;
        public override SmallType Type
        {
            get { return Value.Type; }
        }
        public NegateExpressionSyntax(int pPrecedence, ExpressionSyntax pValue) : base(pPrecedence, pValue) { }

        public override void Emit(ILRunner pRunner)
        {
            Value.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Neg);
        }
    }
}
