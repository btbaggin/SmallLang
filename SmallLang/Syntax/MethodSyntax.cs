using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class MethodSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Method;
        public override SmallType Type => SmallType.Undefined;
        public bool External { get; private set; }
        public string Name { get; private set; }
        public IList<ParameterSyntax> Parameters { get; private set; }
        public IList<ReturnValueSyntax> ReturnValues { get; private set; }
        public BlockSyntax Body { get; private set; }
        public IList<string> TypeHints { get; private set; }
        private MethodDefinition _def;

        public MethodSyntax(string pName, IList<ParameterSyntax> pParameters, IList<ReturnValueSyntax> pReturnValues, BlockSyntax pBody, bool pIsExtern)
        {
            External = pIsExtern;
            Name = pName;
            Parameters = pParameters;
            ReturnValues = pReturnValues;
            Body = pBody;

            foreach(var p in Parameters)
            {
                p.Parent = this;
            }
            foreach(var r in ReturnValues)
            {
                r.Parent = this;
            }
            Body.Parent = this;
        }

        internal void SetDefinition(MethodDefinition pDefinition)
        {
            _def = pDefinition;
        }

        internal void SetTypeHints(IList<string> pHints)
        {
            TypeHints = pHints;
        }

        public void EmitDefinition(ILRunner pRunner)
        {
            pRunner.EmitFunction(_def);
        }

        public override void Emit(ILRunner pRunner)
        {
            pRunner.SetCurrentMethod(_def);
            if (_def.ExternMethod == null)
            {
                Body.Emit(pRunner);
                if (_def.ReturnTypes.Length == 0) pRunner.Emitter.Emit(OpCodes.Ret);
            }
        }

        public override SyntaxNode WithAttributes(SyntaxNode pNode)
        {
            TypeHints = ((MethodSyntax)pNode).TypeHints;
            _def = ((MethodSyntax)pNode)._def;
            return base.WithAttributes(pNode);
        }
    }
}
