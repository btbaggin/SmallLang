using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class ConstSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Const;
        public override SmallType Type
        {
            get { return Value.Type; }
        }
        public IdentifierSyntax Identifier { get; private set; }
        public IdentifierSyntax Value { get; private set; }

        public ConstSyntax(IdentifierSyntax pIdentifier, IdentifierSyntax pValue)
        {
            Identifier = pIdentifier;
            Value = pValue;
            Identifier.Parent = this;
            Value.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
