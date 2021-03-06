﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class BitwiseAndExpressionSyntax : BinaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BitwiseAndExpression;
        public override SmallType Type => SmallType.I32;
        
        public BitwiseAndExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence, pLeft, pRight) { }

        public override void Emit(ILRunner pRunner)
        {
            Left.Emit(pRunner);
            Right.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.And);
        }
    }
}
