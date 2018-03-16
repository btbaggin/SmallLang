using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class TernaryExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.TernaryExpression;
        public override SmallType Type
        {
            get { return Center.Type; }
        }
        public ExpressionSyntax Left { get; private set; }
        public ExpressionSyntax Center { get; private set; }
        public ExpressionSyntax Right { get; private set; }
        public TernaryExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pCenter, ExpressionSyntax pRight) : base(pPrecedence)
        {
            Left = pLeft;
            Center = pCenter;
            Right = pRight;
        }

        public void SetLeft(ExpressionSyntax pLeft)
        {
            pLeft.Parent = this;
            Left = pLeft;
        }

        public void SetCenter(ExpressionSyntax pCenter)
        {
            pCenter.Parent = this;
            Center = pCenter;
        }

        public void SetRight(ExpressionSyntax pRight)
        {
            pRight.Parent = this;
            Right = pRight;
        }

        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
