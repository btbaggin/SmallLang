using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class DecrementExpressionSyntax : UnaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.DecrementExpression;
        public override SmallType Type
        {
            get { return Value.Type; }
        }
        public DecrementExpressionSyntax(int pPrecedence, ExpressionSyntax pValue) : base(pPrecedence, pValue) { }


        public override void Emit(ILRunner pRunner)
        {
            var i = (IdentifierSyntax)Value;
            i.PreEmitForAssignment(pRunner);
            Value.Emit(pRunner);
            pRunner.EmitInt(1);
            pRunner.Emitter.Emit(OpCodes.Sub);
            i.PostEmitForAssignment(pRunner);
        }
    }
}
