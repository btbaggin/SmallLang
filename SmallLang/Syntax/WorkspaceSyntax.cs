using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public class WorkspaceSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Workspace;
        public override SmallType Type => SmallType.Undefined;
        public IList<ImportSyntax> Imports { get; private set; }
        public IList<MethodSyntax> Methods { get; private set; }
        public IList<CastSyntax> Casts { get; private set; }
        public IList<StructSyntax> Structs { get; private set; }
        public IList<ConstSyntax> Consts { get; private set; }
        public WorkspaceSyntax(IList<ImportSyntax> pImports, 
                               IList<MethodSyntax> pMethods, 
                               IList<CastSyntax> pCasts, 
                               IList<StructSyntax> pStructs, 
                               IList<ConstSyntax> pConsts)
        {
            Imports = pImports;
            Methods = pMethods;
            Casts = pCasts;
            Structs = pStructs;
            Consts = pConsts;
            foreach(var i in Imports)
            {
                i.Parent = this;
            }
            foreach(var m in Methods)
            {
                m.Parent = this;
            }
            foreach(var c in Casts)
            {
                c.Parent = this;
            }
            foreach(var s in Structs)
            {
                s.Parent = this;
            }
            foreach(var c in Consts)
            {
                c.Parent = this;
            }
        }

        public override void Emit(ILRunner pRunner)
        {
            foreach(var s in Structs)
            {
                s.Emit(pRunner);
            }

            foreach (var f in Methods)
            {
                f.EmitDefinition(pRunner);
            }
            foreach (var c in Casts)
            {
                c.EmitDefinition(pRunner);
            }

            foreach(var s in Structs)
            {
                s.EmitInitializers(pRunner);
                s.StructType.Create();
            }

            foreach (var f in Methods)
            {
                f.Emit(pRunner);
            }
            foreach (var c in Casts)
            {
                c.Emit(pRunner);
            }
            pRunner.Finish();
        }
    }
}
