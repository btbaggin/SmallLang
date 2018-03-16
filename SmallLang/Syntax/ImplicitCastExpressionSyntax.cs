using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ImplicitCastExpressionSyntax : UnaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ImplicitCastExpression;
        private SmallType _type;
        public override SmallType Type
        {
            get { return _type; }
        }
        public ImplicitCastExpressionSyntax(int pPrecedence, ExpressionSyntax pValue) : base(pPrecedence, pValue) { }

        public void SetType(SmallType pType)
        {
            _type = pType;
        }

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
