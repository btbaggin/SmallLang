using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class IncrementExpressionSyntax : UnaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.IncrementExpression;
        public override SmallType Type
        {
            get { return Value.Type; }
        }
        public IncrementExpressionSyntax(int pPrecedence, ExpressionSyntax pValue) : base(pPrecedence, pValue) { }

        public override void Emit(ILRunner pRunner)
        {
            var i = (IdentifierSyntax)Value;
            i.PreEmitForAssignment(pRunner);
            Value.Emit(pRunner);
            pRunner.EmitInt(1);
            pRunner.Emitter.Emit(OpCodes.Add);
            i.PostEmitForAssignment(pRunner);
        }
    }
}
