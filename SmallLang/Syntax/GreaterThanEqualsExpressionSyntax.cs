using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class GreaterThanEqualsExpressionSyntax : BinaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.GreaterThanEqualsExpression;
        public override SmallType Type => SmallType.Boolean;
        public GreaterThanEqualsExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence, pLeft, pRight) { }

        public override void Emit(ILRunner pRunner)
        {
            Left.Emit(pRunner);
            Right.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Clt);
            pRunner.EmitBool(false);
            pRunner.Emitter.Emit(OpCodes.Ceq);
        }
    }
}
