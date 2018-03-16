using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class CastSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Cast;
        public override SmallType Type => SmallType.Undefined;
        public ParameterSyntax Parameter { get; private set; }
        public ReturnValueSyntax ReturnValue { get; private set; }
        public BlockSyntax Body { get; private set; }
        private MethodDefinition _def;
        public CastSyntax(ParameterSyntax pFrom, ReturnValueSyntax pTo, BlockSyntax pBody)
        {
            Parameter = pFrom;
            ReturnValue = pTo;
            Body = pBody;
            Parameter.Parent = this;
            ReturnValue.Parent = this;
            Body.Parent = this;
        }

        internal void SetDefinition(MethodDefinition pDefinition)
        {
            _def = pDefinition;
        }

        public void EmitDefinition(ILRunner pRunner)
        {
            pRunner.EmitFunction(_def);
        }

        public override void Emit(ILRunner pRunner)
        {
            pRunner.SetCurrentMethod(_def);
            Body.Emit(pRunner);
        }

        public override SyntaxNode WithAttributes(SyntaxNode pNode)
        {
            _def = ((CastSyntax)pNode)._def;
            return base.WithAttributes(pNode);
        }
    }
}
