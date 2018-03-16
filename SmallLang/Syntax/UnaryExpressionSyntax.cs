using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Syntax
{
    public abstract class UnaryExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
        public ExpressionSyntax Value { get; private set; }
        protected UnaryExpressionSyntax(int pPrecedence, ExpressionSyntax pValue) : base(pPrecedence)
        {
            Value = pValue;
        }

        internal void SetValue(ExpressionSyntax pValue)
        {
            pValue.Parent = this;
            Value = pValue;
        }
    }
}
