using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ObjectInitializerExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.DefaultInitializerExpression;
        public override SmallType Type
        {
            get { return Value.Type; }
        }
        public ExpressionSyntax Value { get; private set; }
        public IList<string> TypeHints { get; private set; }

        public ObjectInitializerExpressionSyntax(ExpressionSyntax pValue) : base(0)
        {
            Value = pValue;
        }

        public override void Emit(ILRunner pRunner)
        {
            pRunner.Emitter.Emit(OpCodes.Initobj, Value.Type.ToSystemType());
        }
    }
}
