using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class MemberIdentifierSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.MemberIdentifier;
        public LocalDefinition Struct { get; private set; }
        public MemberIdentifierSyntax(LocalDefinition pStruct, string pValue) : base(pValue)
        {
            Struct = pStruct;
        }

        public override void Emit(ILRunner pRunner)
        {
            var t = Struct.Type;
            if (t.IsArray) t = t.GetElementType();
            var type = t.ToSystemType();

            System.Reflection.FieldInfo f = null;
            if (IsTypeBuilder(type)) f = TypeBuilder.GetField(type, type.GetGenericTypeDefinition().GetField(Value));
            else f = type.GetField(Value);

            pRunner.Emitter.Emit(OpCodes.Ldfld, f);
        }

        private bool IsTypeBuilder(Type pType)
        {
            bool isTypeBuilderInstantiation = false;
            if (pType.IsGenericType && !(pType is TypeBuilder))
            {
                foreach (var genericTypeArg in pType.GetGenericArguments())
                {
                    if (isTypeBuilderInstantiation = (
                        genericTypeArg is TypeBuilder ||
                        genericTypeArg is GenericTypeParameterBuilder ||
                        IsTypeBuilder(genericTypeArg)))
                        break;
                }
                isTypeBuilderInstantiation |= pType.GetGenericTypeDefinition() is TypeBuilder;
            }
            return isTypeBuilderInstantiation;
        }

        public override void PostEmitForAssignment(ILRunner pRunner)
        {
            var t = Struct.Type;
            if (t.IsArray) t = t.GetElementType();
            var type = t.ToSystemType();

            System.Reflection.FieldInfo f = null;
            if (IsTypeBuilder(type)) f = TypeBuilder.GetField(type, type.GetGenericTypeDefinition().GetField(Value));
            else f = type.GetField(Value);
            pRunner.Emitter.Emit(OpCodes.Stfld, f);
        }
    }
}
