using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class LengthQueryExpressionSyntax : IdentifierSyntax
    {
        public override SmallType Type => SmallType.I32;
        public override SyntaxKind Kind => SyntaxKind.LengthQuery;
        public IdentifierSyntax Array { get; private set; }

        public LengthQueryExpressionSyntax(IdentifierSyntax pValue) : base("len")
        {
            Array = pValue;
        }

        public override void Emit(ILRunner pRunner)
        {
            Array.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Ldlen);
        }
    }
}
