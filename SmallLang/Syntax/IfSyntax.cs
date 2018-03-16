using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class IfSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.If;
        public override SmallType Type => SmallType.Boolean;
        public ExpressionSyntax Condition { get; private set; }
        public BlockSyntax Body { get; private set; }
        public ElseSyntax Else { get; private set; }

        internal bool ReturnsInBody { get; set; }

        public IfSyntax(ExpressionSyntax pCondition, BlockSyntax pBody, ElseSyntax pElse)
        {
            Condition = pCondition;
            Body = pBody;
            Else = pElse;
            Condition.Parent = this;
            Body.Parent = this;
            if(Else != null) Else.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Condition.Emit(pRunner);
            Label end = pRunner.Emitter.DefineLabel();

            if (Else != null)
            {
                //If the condition is not true, jump to else
                Label e = pRunner.Emitter.DefineLabel();
                pRunner.Emitter.Emit(OpCodes.Brfalse, e);

                Body.Emit(pRunner);
                if(!ReturnsInBody) pRunner.Emitter.Emit(OpCodes.Br, end);

                pRunner.Emitter.MarkLabel(e);
                Else.Emit(pRunner);
            }
            else
            {
                if(!ReturnsInBody) pRunner.Emitter.Emit(OpCodes.Brfalse, end);
                Body.Emit(pRunner);
            }

            if(!ReturnsInBody) pRunner.Emitter.MarkLabel(end);
        }
    }
}
