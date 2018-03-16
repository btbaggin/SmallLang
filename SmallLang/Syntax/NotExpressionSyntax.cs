using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class NotExpressionSyntax : UnaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NotExpression;
        public override SmallType Type => SmallType.Boolean;
        public NotExpressionSyntax(int pPrecedence, ExpressionSyntax pValue) : base(pPrecedence, pValue) { }

        public override void Emit(ILRunner pRunner)
        {
            Value.Emit(pRunner);
            pRunner.EmitBool(false);
            pRunner.Emitter.Emit(OpCodes.Ceq);
        }
    }
}
