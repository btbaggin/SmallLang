using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ForSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.For;
        public DeclarationStatementSyntax Declaration { get; private set; }
        public ExpressionSyntax Condition { get; private set; }
        public SyntaxNode PostLoop { get; private set; }
        public BlockSyntax Body { get; private set; }
        public override SmallType Type => SmallType.Undefined;

        public ForSyntax(DeclarationStatementSyntax pDeclaration, ExpressionSyntax pCondition, SyntaxNode pPostLoop, BlockSyntax pBody)
        {
            Declaration = pDeclaration;
            Condition = pCondition;
            PostLoop = pPostLoop;
            Body = pBody;
            Declaration.Parent = this;
            Condition.Parent = this;
            PostLoop.Parent = this;
            Body.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Declaration.Emit(pRunner);

            Label start = pRunner.Emitter.DefineLabel();
            pRunner.Emitter.MarkLabel(start);
            Condition.Emit(pRunner);

            Label end = pRunner.Emitter.DefineLabel();
            pRunner.Emitter.Emit(OpCodes.Brfalse, end);

            Body.Emit(pRunner);
            PostLoop.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Br, start);
            pRunner.Emitter.MarkLabel(end);

        }
    }
}
