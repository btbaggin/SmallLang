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
    public class CatAssignmentExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.CatAssignmentExpression;
        public override SmallType Type
        {
            get { return Value.Type; }
        }
        public IdentifierSyntax Name { get; private set; }
        public ExpressionSyntax Value { get; private set; }
        public CatAssignmentExpressionSyntax(int pPrecedence, IdentifierSyntax pLeft, ExpressionSyntax pRight) : base(pPrecedence)
        {
            Name = pLeft;
            Value = pRight;
            Name.Parent = this;
            Value.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            Name.PreEmitForAssignment(pRunner);
            Name.Emit(pRunner);
            Value.Emit(pRunner);

            var a = Assembly.Load(new AssemblyName("mscorlib"));
            var t = a.GetType("System.String");
            var m = t.GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
            pRunner.Emitter.Emit(OpCodes.Call, m);
            
            Name.PostEmitForAssignment(pRunner);
        }
    }
}
