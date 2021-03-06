﻿using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class EqualsExpressionSyntax : BinaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.EqualsExpression;
        public override SmallType Type => SmallType.Boolean;

        public EqualsExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence, pLeft, pRight) { }

        public override void Emit(ILRunner pRunner)
        {
            Left.Emit(pRunner);
            Right.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Ceq);
        }
    }
}
