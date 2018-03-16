using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class ImportSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Import;
        public override SmallType Type => SmallType.Undefined;
        public string Path { get; private set; }
        public string Alias { get; private set; }
        public ImportSyntax(string pPath, string pAlias)
        {
            Path = pPath;
            Alias = pAlias;
        }

        public override void Emit(ILRunner pRunner)
        {
            throw new NotImplementedException();
        }
    }
}
