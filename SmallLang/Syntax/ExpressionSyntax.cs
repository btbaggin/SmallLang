using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public abstract class ExpressionSyntax : SyntaxNode
    {
        public int Precedence { get; private set; }
        private static ExpressionSyntax _sentinel = new SentinelExpressionSyntax();
        public static ExpressionSyntax Sentinel { get { return _sentinel; } }

        protected ExpressionSyntax(int pPrecedence)
        {
            Precedence = pPrecedence;
        }
    }

    public class SentinelExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.Expression;
        public override SmallType Type => SmallType.Undefined;
        public SentinelExpressionSyntax() : base(int.MaxValue) { }
        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
