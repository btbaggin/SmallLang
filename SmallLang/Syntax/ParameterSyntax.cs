using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class ParameterSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.Parameter;
        public bool IsRef { get; private set; }
        public string Namespace { get; private set; }
        public ParameterSyntax(bool pIsRef, string pNamespace, string pName) : base(pName)
        {
            IsRef = pIsRef;
            Namespace = pNamespace;
        }

        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
