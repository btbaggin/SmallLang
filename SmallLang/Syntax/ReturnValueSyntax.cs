using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class ReturnValueSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.ReturnValue;
        private SmallType _type;
        public override SmallType Type
        {
            get { return _type; }
        }
        public string Namespace { get; private set; }
        public string Value { get; private set; }
        public ReturnValueSyntax(string pNamespace, string pValue)
        {
            Namespace = pNamespace;
            Value = pValue;
        }

        public void SetType(SmallType pType)
        {
            _type = pType;
        }

        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
