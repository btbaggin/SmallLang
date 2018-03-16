using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ElseSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Else;
        public override SmallType Type => SmallType.Undefined;
        public IfSyntax If { get; private set; }
        public BlockSyntax Body { get; private set; }

        public ElseSyntax(IfSyntax pIf, BlockSyntax pBlock)
        {
            If = pIf;
            Body = pBlock;
            if (If != null) If.Parent = this;
            if (Body != null) Body.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            If?.Emit(pRunner);
            Body?.Emit(pRunner);
        }
    }
}
