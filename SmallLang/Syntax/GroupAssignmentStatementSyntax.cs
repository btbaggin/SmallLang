using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class GroupAssignmentStatementSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.GroupAssignmentStatement;
        public override SmallType Type => SmallType.Undefined;
        public IdentifierGroupSyntax Identifier { get; private set; }
        public ExpressionSyntax Value { get; private set; }

        public GroupAssignmentStatementSyntax(IdentifierGroupSyntax pLeft, ExpressionSyntax pRight)
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
