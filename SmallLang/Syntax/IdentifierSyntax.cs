using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class IdentifierSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.Identifier;
        private SmallType _type;
        public override SmallType Type
        {
            get
            {
                if (Local == null) return _type;
                return Local.Type;
            }
        }
        public string Value { get; private set; }
        internal LocalDefinition Local { get; set; }
        internal IList<string> TypeParameters { get; set; }

        public IdentifierSyntax(string pValue) : base(0)
        {
            Value = pValue;
            _type = SmallType.FromString("", pValue);
            TypeParameters = new List<string>();
        }

        public void SetType(SmallType pType)
        {
            if(Local == null) _type = pType;
            else Local.Type = pType;
        }

        public void SetTypeHints(IList<string> pType)
        {
            TypeParameters = pType;
        }

        public override void Emit(ILRunner pRunner)
        {
            Local.Emit(pRunner, this);
        }

        public virtual void PreEmitForAssignment(ILRunner pRunner)
        {
            if(LoadAddress)
            {
                if(!Local.Parameter)
                {
                    var lb = Local.GetLocal(pRunner);
                    pRunner.Emitter.Emit(OpCodes.Ldloca_S, lb);
                }
                else
                {
                    var pb = Local.GetParameter(pRunner);
                    pRunner.Emitter.Emit(OpCodes.Ldarg, pb);
                }
            }
            else if(Local.Field)
            {
                pRunner.Emitter.Emit(OpCodes.Ldarg_0);
            }
        }

        public virtual void PostEmitForAssignment(ILRunner pRunner)
        {
            if(!Local.Field && !LoadAddress)
            {
                var lb = Local.GetLocal(pRunner);
                pRunner.Emitter.Emit(OpCodes.Stloc, lb);
            }
            else if(Local.Field)
            {
                pRunner.Emitter.Emit(OpCodes.Stfld, Local.GetField());
            }
        }

        public override SyntaxNode WithAttributes(SyntaxNode pNode)
        {
            Local = ((IdentifierSyntax)pNode).Local;
            TypeParameters = ((IdentifierSyntax)pNode).TypeParameters;
            return base.WithAttributes(pNode);
        }
    }
}
