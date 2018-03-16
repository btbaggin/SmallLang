﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class MulAssignmentExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.MulAssignmentExpression;
        public override SmallType Type
        {
            get { return Value.Type; }
        }
        public IdentifierSyntax Name { get; private set; }
        public ExpressionSyntax Value { get; private set; }
        public MulAssignmentExpressionSyntax(int pPrecedence, IdentifierSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence)
        {
            Name = pLeft;
            Value = pRight;
            Name.Parent = this;
            Value.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Name.PreEmitForAssignment(pRunner);
            Name.Emit(pRunner);
            Value.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Mul);
            Name.PostEmitForAssignment(pRunner);
        }
    }
}
