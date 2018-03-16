using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class MemberAccessSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.MemberAccess;
        public override SmallType Type
        {
            get { return Value.Type; }
        }
        public IdentifierSyntax Name { get; private set; }
        public new IdentifierSyntax Value { get; private set; }

        public MemberAccessSyntax(IdentifierSyntax pName, IdentifierSyntax pValue) : base("")
        {
            Name = pName;
            Value = pValue;
            Name.Parent = this;
            Value.Parent = this;
        }

        public override void Emit(ILRunner pRunner)
        {
            if (Name.Kind == SyntaxKind.ArrayAccessExpression) Name.LoadAddress = true;
            Name.Emit(pRunner);
            Value.Emit(pRunner);
        }

        public override void PreEmitForAssignment(ILRunner pRunner)
        {
            if(Name.Kind == SyntaxKind.NamespaceIdentifier)
            {
                Value.PreEmitForAssignment(pRunner);
            }
            else if(Value.Kind == SyntaxKind.ArrayAccessExpression)
            {
                Name.Emit(pRunner);
                Value.PreEmitForAssignment(pRunner);
            }
            else if(Name.Kind == SyntaxKind.ArrayAccessExpression)
            {
                Name.LoadAddress = true;
                Name.Emit(pRunner);
            }
            else
            {
                Name.LoadAddress = true;
                Name.PreEmitForAssignment(pRunner);
            }
        }

        public override void PostEmitForAssignment(ILRunner pRunner)
        {
            Value.PostEmitForAssignment(pRunner);
        }
    }
}
