using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public sealed class GroupDeclarationStatementSyntax : DeclarationStatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.GroupDeclarationStatement;
        public override SmallType Type => SmallType.Undefined;
        public new IdentifierGroupSyntax Identifier { get; private set; }
        public new ExpressionSyntax Value { get; private set; }

        public GroupDeclarationStatementSyntax(IdentifierGroupSyntax pLeft, ExpressionSyntax pRight) : base(null, null)
        {
            Identifier = pLeft;
            Value = pRight;
            Identifier.Parent = this;
            Value.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
