using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ArrayAccessExpressionSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ArrayAccessExpression;
        public override SmallType Type
        {
            get { return Identifier.Type.GetElementType(); }
        }
        public IdentifierSyntax Identifier { get; private set; }
        public ExpressionSyntax Index { get; private set; }
        public ArrayAccessExpressionSyntax(IdentifierSyntax pIdentifier, ExpressionSyntax pIndex) : base(pIdentifier.Value)
        {
            Identifier = pIdentifier;
            Index = pIndex;
            Identifier.Parent = this;
            Index.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Identifier.Emit(pRunner);
            if (Identifier.Local.IsAddress) pRunner.Emitter.Emit(OpCodes.Ldind_Ref);
            Index.Emit(pRunner);
            if (Type.IsValueType && LoadAddress) pRunner.Emitter.Emit(OpCodes.Ldelema, Type.ToSystemType());
            else pRunner.Emitter.Emit(OpCodes.Ldelem, Type.ToSystemType());
        }

        public override void PreEmitForAssignment(ILRunner pRunner)
        {
            LoadAddress = true;
            Identifier.Emit(pRunner);
            if (Identifier.Local.IsAddress) pRunner.Emitter.Emit(OpCodes.Ldind_Ref);
            Index.Emit(pRunner);
        }

        public override void PostEmitForAssignment(ILRunner pRunner)
        {
            pRunner.Emitter.Emit(OpCodes.Stelem, Type.ToSystemType());
        }
    }
}
