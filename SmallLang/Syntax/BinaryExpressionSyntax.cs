using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Syntax
{
    public abstract class BinaryExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
        public ExpressionSyntax Left { get; private set; }
        public ExpressionSyntax Right { get; private set; }
        protected BinaryExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence)
        {
            Left = pLeft;
            Right = pRight;
        }

        internal void SetLeft(ExpressionSyntax pLeft)
        {
            pLeft.Parent = this;
            Left = pLeft;
        }

        internal void SetRight(ExpressionSyntax pRight)
        {
            pRight.Parent = this;
            Right = pRight;
        }
    }
}
