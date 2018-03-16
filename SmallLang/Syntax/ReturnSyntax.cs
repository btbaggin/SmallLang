using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ReturnSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Return;
        public override SmallType Type
        {
            get
            {
                SmallType[] types = new SmallType[Values.Count];
                for(int i = 0; i < Values.Count; i++)
                {
                    types[i] = Values[i].Type;
                }
                return SmallType.CreateTupleOf(types);
            }
        }
        public IList<ExpressionSyntax> Values { get; private set; }
        public ReturnSyntax(IList<ExpressionSyntax> pValues)
        {
            Values = pValues;
            foreach(var v in Values)
            {
                v.Parent = this;
            }
        }

        public override void Emit(ILRunner pRunner)
        {
            foreach (var v in Values)
            {
                v.Emit(pRunner);
            }
            if (Values.Count > 1)
            {
                SmallType[] types = new SmallType[Values.Count];
                for (int i = 0; i < Values.Count; i++)
                {
                    types[i] = Values[i].Type;
                }
                var t = SmallType.CreateTupleOf(types).ToSystemType();

                Type[] systemTypes = new Type[types.Length];
                for(int i = 0; i < types.Length; i++)
                {
                    systemTypes[i] = types[i].ToSystemType();
                }
                var c = t.GetConstructor(systemTypes);
                pRunner.Emitter.Emit(OpCodes.Newobj, c);
            }

            pRunner.Emitter.Emit(OpCodes.Ret);
        }
    }
}
