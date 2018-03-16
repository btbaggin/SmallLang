using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ArgumentExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ArgumentExpression;
        public override SmallType Type => Value.Type;
        public bool IsRef { get; internal set; }
        public ExpressionSyntax Value { get; private set; }

        internal bool CreateCopy { get; set; }

        public ArgumentExpressionSyntax(bool pIsRef, ExpressionSyntax pValue) : base(0)
        {
            IsRef = pIsRef;
            Value = pValue;
            Value.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Value.LoadAddress = IsRef;
            Value.Emit(pRunner);
            if (CreateCopy) pRunner.Emitter.Emit(OpCodes.Ldind_Ref);
        }
    }
}
