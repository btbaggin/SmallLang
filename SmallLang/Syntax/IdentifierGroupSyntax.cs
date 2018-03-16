using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class IdentifierGroupSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.IdentifierGroup;
        public override SmallType Type
        {
            get
            {
                SmallType[] types = new SmallType[Identifiers.Count];
                for (int i = 0; i < Identifiers.Count; i++)
                {
                    types[i] = Identifiers[i].Type;
                }
                return SmallType.CreateTupleOf(types);
            }
        }

        public IList<IdentifierSyntax> Identifiers { get; private set; }
        public IdentifierGroupSyntax(IList<IdentifierSyntax> pIdentifiers) : base(0)
        {
            Identifiers = pIdentifiers;
            foreach(var i in Identifiers)
            {
                i.Parent = this;
            }
        }

        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
