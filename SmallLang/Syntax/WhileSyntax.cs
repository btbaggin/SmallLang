using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class WhileSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.While;
        public override SmallType Type => SmallType.Undefined;
        public ExpressionSyntax Condition { get; private set; }
        public BlockSyntax Body { get; private set; }

        public WhileSyntax(ExpressionSyntax pCondition, BlockSyntax pBody)
        {
            Condition = pCondition;
            Body = pBody;
            Condition.Parent = this;
            Body.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Label start = pRunner.Emitter.DefineLabel();
            pRunner.Emitter.MarkLabel(start);
            Condition.Emit(pRunner);

            Label end = pRunner.Emitter.DefineLabel();
            pRunner.Emitter.Emit(OpCodes.Brfalse, end);

            Body.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Br, start);
            pRunner.Emitter.MarkLabel(end);
        }
    }
}
