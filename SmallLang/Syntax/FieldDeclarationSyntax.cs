using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class FieldDeclarationSyntax : IdentifierSyntax
    {
        public override SmallType Type
        {
            get { return SmallType.Create(Namespace, Value); }
        }
        public override SyntaxKind Kind => SyntaxKind.FieldDeclaration;
        public string Namespace { get; private set; }

        public FieldDeclarationSyntax(string pNamespace, string pValue) : base(pValue)
        {
            Namespace = pNamespace;
        }

        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
