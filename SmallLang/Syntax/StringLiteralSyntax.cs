using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class StringLiteralSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.StringLiteral;
        public override SmallType Type => SmallType.String;
        public StringLiteralSyntax(string pValue) : base(pValue)
        {
        }

        public override void Emit(ILRunner pRunner)
        {
            pRunner.Emitter.Emit(OpCodes.Ldstr, Value);
        }
    }
}
