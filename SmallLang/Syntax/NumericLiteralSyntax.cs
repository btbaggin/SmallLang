using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public enum NumberType
    {
        I16,
        I32,
        I64,
        Float,
        Double
    }

    public class NumericLiteralSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NumericLiteral;
        public override SmallType Type
        {
            get
            {
                switch(NumberType)
                {
                    case NumberType.I16: return SmallType.I16;
                    case NumberType.I32: return SmallType.I32;
                    case NumberType.I64: return SmallType.I64;
                    case NumberType.Float: return SmallType.Float;
                    case NumberType.Double: return SmallType.Double;
                    default: throw new NotImplementedException();
                }
            }
        }
        public NumberType NumberType { get; internal set; }
        public NumericLiteralSyntax(string pValue, NumberType pType) : base(pValue)
        {
            NumberType = pType;
        }

        public override void Emit(ILRunner pRunner)
        {
            switch (NumberType)
            {
                case NumberType.I16:
                    pRunner.Emitter.Emit(OpCodes.Ldc_I4, short.Parse(Value));
                    break;

                case NumberType.I32:
                    pRunner.EmitInt(int.Parse(Value));
                    break;

                case NumberType.I64:
                    pRunner.Emitter.Emit(OpCodes.Ldc_R8, long.Parse(Value));
                    break;

                case NumberType.Double:
                    pRunner.Emitter.Emit(OpCodes.Ldc_R8, double.Parse(Value));
                    break;

                case NumberType.Float:
                    pRunner.Emitter.Emit(OpCodes.Ldc_R4, float.Parse(Value));
                    break;
            }
        }
    }
}
