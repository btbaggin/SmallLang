﻿using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class DivisionExpressionSyntax : BinaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.DivisionExpression;
        public override SmallType Type
        {
            get { return GetReturnType(); }
        }

        public DivisionExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence, pLeft, pRight) { }

        public override void Emit(ILRunner pRunner)
        {
            Left.Emit(pRunner);
            Right.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Div);
        }

        private SmallType GetReturnType()
        {
            var ts = new SmallType[] { SmallType.I16, SmallType.I32, SmallType.I64, SmallType.Float, SmallType.Double };
            var t = new SmallType[,] { { SmallType.I16, SmallType.I32, SmallType.I64, SmallType.Float, SmallType.Double },
                               { SmallType.I32, SmallType.I32, SmallType.I64, SmallType.Float, SmallType.Double },
                               { SmallType.I64, SmallType.I64, SmallType.I64, SmallType.Float, SmallType.Double },
                               { SmallType.Float, SmallType.Float, SmallType.Float, SmallType.Float, SmallType.Double },
                               { SmallType.Double, SmallType.Double, SmallType.Double, SmallType.Double, SmallType.Double } };

            int i = Array.IndexOf(ts, Left.Type);
            int i2 = Array.IndexOf(ts, Right.Type);
            if (i == -1 || i2 == -1) return SmallType.Undefined;
            return t[i, i2];
        }
    }
}
