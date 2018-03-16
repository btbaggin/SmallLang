using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class BooleanLiteralSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BooleanLiteral;
        public override SmallType Type => SmallType.Boolean;

        public bool LiteralValue { get; private set; }
        public BooleanLiteralSyntax(string pValue) : base(pValue)
        {
            LiteralValue = bool.Parse(pValue);
        }

        public override void Emit(ILRunner pRunner)
        {
            pRunner.EmitBool(LiteralValue);
        }
    }
}
