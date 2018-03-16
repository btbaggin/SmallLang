using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class OrExpressionSyntax : BinaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.OrExpression;
        public override SmallType Type => SmallType.Boolean;
        public OrExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence, pLeft, pRight) { }

        public override void Emit(ILRunner pRunner)
        {
            Label end = pRunner.Emitter.DefineLabel();
            Label lbFalse = pRunner.Emitter.DefineLabel();
            Label lbTrue = pRunner.Emitter.DefineLabel();

            Left.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Brtrue_S, lbTrue);

            Right.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Brfalse_S, lbFalse);

            pRunner.Emitter.MarkLabel(lbTrue);
            pRunner.EmitBool(true);
            pRunner.Emitter.Emit(OpCodes.Br_S, end);

            pRunner.Emitter.MarkLabel(lbFalse);
            pRunner.EmitBool(false);
            pRunner.Emitter.MarkLabel(end);
        }
    }
}
