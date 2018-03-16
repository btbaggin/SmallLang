using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class StructSyntax : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.Struct;
        public override SmallType Type => SmallType.Undefined;
        public string Name { get; private set; }
        public string Prefix { get; private set; }
        public IList<string> TypeArgs { get; private set; }
        public IList<FieldDeclarationSyntax> Fields { get; private set; }
        public BlockSyntax Initializer { get; private set; }

        public SmallType StructType { get; private set; }

        public StructSyntax(string pName, string pPrefix, IList<string> pTypeArgs, IList<FieldDeclarationSyntax> pFields, BlockSyntax pInitializer)
        {
            Name = pName;
            Prefix = pPrefix;
            TypeArgs = pTypeArgs;
            Fields = pFields;
            foreach(var f in Fields)
            {
                f.Parent = this;
            }
            Initializer = pInitializer;
            if(Initializer != null) Initializer.Parent = this;
        }

        internal void SetType(SmallType pDef)
        {
            StructType = pDef;
        }

        public override void Emit(ILRunner pRunner)
        {
            pRunner.EmitStruct(StructType);
        }

        public void EmitInitializers(ILRunner pRunner)
        {
            if (StructType.HasInitializer) StructType.Initializer.Emit(pRunner);
        }

        public override SyntaxNode WithAttributes(SyntaxNode pNode)
        {
            StructType = ((StructSyntax)pNode).StructType;
            return base.WithAttributes(pNode);
        }
    }
}
