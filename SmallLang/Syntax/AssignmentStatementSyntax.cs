using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class AssignmentStatementSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.AssignmentStatement;
        public override SmallType Type => SmallType.Undefined;
        public IdentifierSyntax Identifier { get; private set; }
        public ExpressionSyntax Value { get; private set; }

        public AssignmentStatementSyntax(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            Identifier = pLeft;
            Value = pRight;
            Identifier.Parent = this;
            Value.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Identifier.PreEmitForAssignment(pRunner);
            Value.Emit(pRunner);
            Identifier.PostEmitForAssignment(pRunner);
        }
    }
}
