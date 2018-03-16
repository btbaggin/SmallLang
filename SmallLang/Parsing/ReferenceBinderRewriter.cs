using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Syntax;

namespace SmallLang.Parsing
{
    class MemberAccessContext
    {
        public LocalDefinition Member { get; set; }
        public string Namespace { get; set; }
        public MemberAccessContext()
        {
            Namespace = "";
        }
    }

    class ReferenceBinderRewriter : SyntaxRewriter
    {
        public override SyntaxNode Visit(MemberAccessSyntax pNode)
        {
            MemberAccessContext ma = GetValue<MemberAccessContext>("MemberContext", null);
            MemberAccessContext context = new MemberAccessContext();
            if (ma != null) context.Member = ma.Member;
            using (new ContextValue(this, "MemberContext", context))
            {
                var n = pNode.Name.Accept<IdentifierSyntax>(this);

                var v = pNode.Value.Accept<IdentifierSyntax>(this);
                if (v.GetType() == typeof(FunctionInvocationSyntax)) n.LoadAddress = true;

                return SyntaxFactory.MemberAccess(n, v);
            }
        }

        public override SyntaxNode Visit(ArrayAccessExpressionSyntax pNode)
        {
            var n = pNode.Identifier.Accept<IdentifierSyntax>(this);

            ExpressionSyntax i = null;
            using (new ContextValue(this, "MemberContext", new MemberAccessContext()))
            {
                i = pNode.Index.Accept<ExpressionSyntax>(this);
            }

            return SyntaxFactory.ArrayAccessExpression(n, i);
        }

        public override SyntaxNode Visit(IdentifierSyntax pNode)
        {
            var m = GetValue<MemberAccessContext>("MemberContext", null);
            if (m != null && MetadataCache.ImportedNamespaces().Contains(pNode.Value))
            {
                m.Namespace = pNode.Value;
                return SyntaxFactory.NamespaceIdentifier(pNode.Value);
            }
            
            if (m == null || m.Member == null)
            {
                if (GetValue("InDeclaration", false))
                {
                    if (MetadataCache.LocalExistsInThisScope(pNode.Value))
                        Compiler.ReportError(CompilerErrorType.DuplicateLocal, pNode.Parent, pNode.Value);
                    else
                    {
                        var space = m == null ? "" : m.Namespace;
                        var t = SmallType.FromString(space, pNode.Value);
                        if (pNode.TypeParameters.Count > 0)
                        {
                            SmallType[] typeArgs = new SmallType[pNode.TypeParameters.Count];
                            for(int i = 0; i < typeArgs.Length; i++)
                            {
                                typeArgs[i] = SmallType.FromString("", pNode.TypeParameters[i]);
                            }
                            t = t.MakeGenericType(typeArgs);
                        }

                        pNode.Local = MetadataCache.DefineLocal(pNode, t);
                    }
                }
                else
                {
                    var t = GetValue<SmallType>("StructType", null);
                    if (t != null)
                    {
                        var field = t.GetField(pNode.Value);
                        if (field == null)
                        {
                            Compiler.ReportError(CompilerErrorType.StructInvalidMember, pNode, t.Name, pNode.Value);
                        }
                        pNode.Local = MetadataCache.DefineField(pNode, field.Type, t);
                    }
                    else
                    {
                        if (!MetadataCache.LocalExists(pNode.Value))
                            Compiler.ReportError(CompilerErrorType.LocalNotDefined, pNode.Parent, pNode.Value);
                        else pNode.Local = MetadataCache.GetLocal(pNode.Value);
                    }
                }
                if(m != null) m.Member = pNode.Local;

                if (!pNode.Type.IsTupleType && pNode.Type.IsValueType && GetValue("LoadObject", false))
                    pNode.LoadAddress = true;
                
                return base.Visit(pNode);
            }

            var mi = SyntaxFactory.MemberIdentifier(m.Member, pNode.Value);
            if(m.Member.Type != SmallType.Undefined)
            {
                if (!m.Member.Type.FieldExists(pNode.Value))
                    Compiler.ReportError(CompilerErrorType.StructInvalidMember, pNode.Parent, m.Member.Value, pNode.Value);
                else
                {
                    var f = m.Member.Type.GetField(pNode.Value);
                    mi.Local = LocalDefinition.Create(mi, pNode.Value, f.Type);
                    m.Member = mi.Local;
                }
            }

            if (!mi.Type.IsTupleType && mi.Type.IsValueType && !GetValue("LoadObject", false))
                mi.LoadAddress = true;

            return mi; 
        }

        public override SyntaxNode Visit(FunctionInvocationSyntax pNode)
        {
            var o = GetValue<MemberAccessContext>("MemberContext", null);
            if (o != null)
            {
                pNode.Namespace = o.Namespace;
                o.Namespace = "";
                if(o.Member != null) pNode.InstanceType = o.Member.Type.ToSystemType();
            }


            if (!MetadataCache.MethodExists(pNode.Namespace, pNode.Value))
                Compiler.ReportError(CompilerErrorType.MethodNotDefined, pNode, pNode.Value, pNode.Namespace);
            else
            {
                var md = MetadataCache.FindBestOverload(pNode, out bool e);
                if (md.ExternMethod != null && md.ExternMethod.IsPrivate)
                    Compiler.ReportError(CompilerErrorType.MethodNotExported, pNode, pNode.Value);

                if (md.Parameters.Length != pNode.Arguments.Count)
                    Compiler.ReportError(CompilerErrorType.NoMethodOverload, pNode, pNode.Value, pNode.Arguments.Count.ToString());

                if (o != null && md.ReturnTypes.Length > 0)
                    o.Member = LocalDefinition.Create(pNode, "__" + pNode.Value + "__", md.GetReturnType());
            }

            List<ArgumentExpressionSyntax> arguments = new List<ArgumentExpressionSyntax>();
            foreach (var a in pNode.Arguments)
            {
                using (new ContextValue(this, "MemberContext", new MemberAccessContext()))
                {
                    arguments.Add(a.Accept<ArgumentExpressionSyntax>(this));
                }
            }
            return SyntaxFactory.FunctionInvocation(pNode.Value, arguments).WithAttributes(pNode);
        }

        public override SyntaxNode Visit(ParameterSyntax pNode)
        {
            if (MetadataCache.LocalExistsInThisScope(pNode.Value))
                Compiler.ReportError(CompilerErrorType.DuplicateLocal, pNode.Parent, pNode.Value);
            else
                pNode.Local = MetadataCache.DefineParameter(pNode, pNode.Type);

            return base.Visit(pNode);
        }

        public override SyntaxNode Visit(DeclarationStatementSyntax pNode)
        {
            var v = pNode.Value.Accept<ExpressionSyntax>(this);

            IdentifierSyntax n = null;
            using (new ContextValue(this, "InDeclaration", true))
            {
                using (new ContextValue(this, "LoadObject", pNode.Value.GetType() == typeof(ObjectInitializerExpressionSyntax)))
                {
                    n = pNode.Identifier.Accept<IdentifierSyntax>(this);
                }
            }

            return SyntaxFactory.DeclarationStatement(n, v);
        }
        
        public override SyntaxNode Visit(AssignmentStatementSyntax pNode)
        {
            IdentifierSyntax n = null;
            n = pNode.Identifier.Accept<IdentifierSyntax>(this);

            var v = pNode.Value.Accept<ExpressionSyntax>(this);
            return SyntaxFactory.AssignmentStatement(n, v);
        }

        public override SyntaxNode Visit(StructSyntax pNode)
        {
            using (new ContextValue(this, "StructType", pNode.StructType))
            {
                return SyntaxFactory.Struct(pNode.Name, pNode.Prefix, pNode.TypeArgs, pNode.Fields, pNode.Initializer?.Accept<BlockSyntax>(this)).WithAttributes(pNode);
            }
        }

        public override SyntaxNode Visit(IfSyntax pNode)
        {
            using (new MetadataCache.LocalScope())
            {
                return base.Visit(pNode);
            }
        }

        public override SyntaxNode Visit(ElseSyntax pNode)
        {
            using (new MetadataCache.LocalScope())
            {
                return base.Visit(pNode);
            }
        }

        public override SyntaxNode Visit(WhileSyntax pNode)
        {
            using (new MetadataCache.LocalScope())
            {
                return base.Visit(pNode);
            }
        }

        public override SyntaxNode Visit(ForSyntax pNode)
        {
            using (new MetadataCache.LocalScope())
            {
                return base.Visit(pNode);
            }
        }

        public override SyntaxNode Visit(MethodSyntax pNode)
        {
            using (new MetadataCache.LocalScope())
            {
                return base.Visit(pNode);
            }
        }

        public override SyntaxNode Visit(CastSyntax pNode)
        {
            using (new MetadataCache.LocalScope())
            {
                return base.Visit(pNode);
            }
        }
    }
}
