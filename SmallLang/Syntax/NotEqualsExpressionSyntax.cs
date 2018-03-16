using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class NotEqualsExpressionSyntax : BinaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NotEqualsExpression;
        public override SmallType Type => SmallType.Boolean;
        public NotEqualsExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence, pLeft, pRight) { }

        public override void Emit(ILRunner pRunner)
        {
            Left.Emit(pRunner);
            Right.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Ceq);
            pRunner.EmitBool(false);
            pRunner.Emitter.Emit(OpCodes.Ceq);
        }
    }
}
