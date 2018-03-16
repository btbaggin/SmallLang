using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class BlockSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Block;
        public override SmallType Type => SmallType.Undefined;

        public IList<SyntaxNode> Statements { get; private set; }
        public BlockSyntax(IList<SyntaxNode> pStatements)
        {
            Statements = pStatements;
            foreach(var s in Statements)
            {
                s.Parent = this;
            }
        }

        public override void Emit(ILRunner pRunner)
        {
            foreach (var s in Statements)
            {
                s.Emit(pRunner);
            }
        }
    }
}
