using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ArrayLiteralSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ArrayLiteral;
        public ExpressionSyntax Size { get; private set; }

        public ArrayLiteralSyntax(ExpressionSyntax pSize) : base("")
        {
            Size = pSize;
            Size.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Size.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Newarr, Type.GetElementType().ToSystemType());
        }
    }
}
