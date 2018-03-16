using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class DeclarationStatementSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.DeclarationStatement;
        public override SmallType Type => SmallType.Undefined;
        public IdentifierSyntax Identifier { get; private set; }
        public ExpressionSyntax Value { get; private set; }

        public DeclarationStatementSyntax(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            Identifier = pLeft;
            Value = pRight;
            if (Identifier != null) Identifier.Parent = this;
            if(Value != null) Value.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Identifier.PreEmitForAssignment(pRunner);
            Value.Emit(pRunner);
            Identifier.PostEmitForAssignment(pRunner);
            if(Identifier.Type.HasInitializer && Value.GetType() == typeof(ObjectInitializerExpressionSyntax))
            {
                if (Identifier.GetType() == typeof(MemberAccessSyntax))
                    pRunner.Emitter.Emit(OpCodes.Ldloca_S, ((MemberAccessSyntax)Identifier).Value.Local.GetLocal(pRunner));
                else
                    pRunner.Emitter.Emit(OpCodes.Ldloca_S, Identifier.Local.GetLocal(pRunner));

                pRunner.Emitter.Emit(OpCodes.Call, Value.Type.Initializer.Info);
            }
        }
    }
}
