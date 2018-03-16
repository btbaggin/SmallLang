using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class ConcatenateExpressionSyntax : BinaryExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ConcatenateExpression;
        public override SmallType Type => SmallType.String;
        public ConcatenateExpressionSyntax(int pPrecedence, ExpressionSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence, pLeft, pRight) { }

        public override void Emit(ILRunner pRunner)
        {
            Left.Emit(pRunner);
            Right.Emit(pRunner);

            var a = Assembly.Load(new AssemblyName("mscorlib"));
            var t = a.GetType("System.String");
            var m = t.GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
            pRunner.Emitter.Emit(OpCodes.Call, m);
        }
    }
}
