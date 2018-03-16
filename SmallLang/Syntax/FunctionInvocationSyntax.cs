using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class FunctionInvocationSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.FunctionInvocation;
        public IList<ArgumentExpressionSyntax> Arguments { get; private set; }

        internal string Namespace { get; set; }
        internal Type InstanceType { get; set; }

        public override SmallType Type
        {
            get
            {
                if (base.Type == null) return base.Type;

                if (base.Type.IsGenericTypeParameter)
                {
                    return SmallType.CreateGenericArgument(TypeParameters[0]); //TODO?
                }

                return base.Type;
            }
        }

        public FunctionInvocationSyntax(string pValue, IList<ArgumentExpressionSyntax> pArguments) : base(pValue)
        {
            Namespace = "";
            Arguments = pArguments;
            foreach(var a in Arguments)
            {
                a.Parent = this;
            }
        }
        public override void Emit(ILRunner pRunner)
        {
            MethodDefinition def = MetadataCache.GetMethod(this);
            foreach (var a in Arguments)
            {
                a.Emit(pRunner);
            }

            if (def.Name == "ToString")
            {
                Type[] types = new Type[Arguments.Count];
                for (int i = 0; i < Arguments.Count; i++)
                {
                    types[i] = Arguments[i].Type.ToSystemType();
                }
                var m = InstanceType.GetMethod(Value, types);
                pRunner.Emitter.Emit(OpCodes.Callvirt, m);
            }
            else
            {
                if (TypeParameters.Count == 0) pRunner.Emitter.Emit(OpCodes.Call, def.CallSite);
                else
                {
                    Type[] types = new Type[TypeParameters.Count];
                    for(int i = 0; i < TypeParameters.Count; i++)
                    {
                        types[i] = SmallType.FromString("", TypeParameters[i]).ToSystemType();
                        if(types[i] == null)
                        {
                            var t = pRunner.CurrentMethod.TypeHints;
                            for(int j = 0; j < t.Count; j++)
                            {
                                if (TypeParameters[i].Equals(t[j].Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    types[i] = t[j].ToSystemType();
                                }
                            }
                            
                        }
                    }
                    var cs = def.CallSite.MakeGenericMethod(types);
                    pRunner.Emitter.Emit(OpCodes.Call, cs);
                }
            }

            if (def.ReturnTypes.Length > 0)
            {
                //TODO Should move to some sort of visitor? This will fail eventually if I allow struct functions
                if (Parent.GetType() == typeof(BlockSyntax) || !string.IsNullOrEmpty(Namespace) && Parent.Parent.GetType() == typeof(BlockSyntax))
                    pRunner.Emitter.Emit(OpCodes.Pop);
            }
        }
        public override SyntaxNode WithAttributes(SyntaxNode pNode)
        {
            var f = (FunctionInvocationSyntax)pNode;
            Namespace = f.Namespace;
            InstanceType = f.InstanceType;
            TypeParameters = f.TypeParameters;
            return base.WithAttributes(pNode);
        }
    }
}
