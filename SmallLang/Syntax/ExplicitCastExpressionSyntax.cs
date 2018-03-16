using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ExplicitCastExpressionSyntax : UnaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ExplicitCastExpression;
        public override SmallType Type
        {
            get { return SmallType.FromString(TypeNamspace, TypeName); }
        }
        public string TypeNamspace { get; internal set; }
        public string TypeName { get; internal set; }
        public ExplicitCastExpressionSyntax(int pPrecedence, ExpressionSyntax pValue) : base(pPrecedence, pValue) { }

        public override void Emit(ILRunner pRunner)
        {
            if (Type == Value.Type) Value.Emit(pRunner);
            else if (Type == SmallType.Object) pRunner.Emitter.Emit(OpCodes.Box, Value.Type.ToSystemType());
            else
            {
                Value.Emit(pRunner);
                if (Type == SmallType.I16) pRunner.Emitter.Emit(OpCodes.Conv_I2);
                else if (Type == SmallType.I32) pRunner.Emitter.Emit(OpCodes.Conv_I4);
                else if (Type == SmallType.I64) pRunner.Emitter.Emit(OpCodes.Conv_I8);
                else if (Type == SmallType.Double) pRunner.Emitter.Emit(OpCodes.Conv_R8);
                else if (Type == SmallType.Float) pRunner.Emitter.Emit(OpCodes.Conv_R4);
                else
                {
                    var def = MetadataCache.GetCast(Value.Type, Type);
                    pRunner.Emitter.Emit(OpCodes.Call, def.CallSite);
                }
            }
        }
    }
}
