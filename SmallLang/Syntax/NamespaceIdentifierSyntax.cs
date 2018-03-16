using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class NamespaceIdentifierSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NamespaceIdentifier;
        public override SmallType Type => SmallType.Undefined;
        public NamespaceIdentifierSyntax(string pValue) : base(pValue) { }
        public override void Emit(ILRunner pRunner)
        {
            //Namespaces do not need to emit anything
        }
        public override void PreEmitForAssignment(ILRunner pRunner)
        {
            //Namespaces do not need to emit anything
        }
        public override void PostEmitForAssignment(ILRunner pRunner)
        {
            //Namespaces do not need to emit anything
        }
    }
}
