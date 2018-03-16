using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class ValueDiscardSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ValueDiscard;
        public override SmallType Type => SmallType.Undefined;
        public ValueDiscardSyntax() : base("") { }
        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
