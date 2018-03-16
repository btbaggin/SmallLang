using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Syntax;

namespace SmallLang.Parsing
{
    partial class ValidationVisitor : SyntaxVisitor
    {
        public override void Visit(IdentifierSyntax pNode)
        {
            if (pNode.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UnknownType, pNode);
            base.Visit(pNode);
        }

        public override void Visit(StructSyntax pNode)
        {
            //Do nothing for structs
        }

        public override void Visit(BlockSyntax pNode)
        {
            for(int i = 0; i < pNode.Statements.Count; i++)
            {
                using (new ContextValue(this, "LastStatement", i == pNode.Statements.Count - 1))
                {
                    pNode.Statements[i].Accept(this);
                }
            }
        }

        public override void Visit(FunctionInvocationSyntax pNode)
        {
            var def = MetadataCache.FindBestOverload(pNode, out bool e);
            for(int i = 0; i < pNode.Arguments.Count; i++)
            {
                pNode.Arguments[i].IsRef = def.Parameters[i].IsRef;
                if (pNode.Arguments[i].Value.GetType() == typeof(IdentifierSyntax))
                {
                    var id = (IdentifierSyntax)pNode.Arguments[i].Value;
                    if (id.Local.IsAddress && !pNode.Arguments[i].IsRef) pNode.Arguments[i].CreateCopy = true;

                }
                pNode.Arguments[i].Accept(this);
            }
        }

        public override void Visit(IfSyntax pNode)
        {
            using (new ContextValue(this, "InIf", true))
            {
                base.Visit(pNode);
                pNode.ReturnsInBody = GetValue("LastStatement", false);
            }
        }

        public override void Visit(ReturnSyntax pNode)
        {
            SetValue("Returns", true);
            base.Visit(pNode);
        }

        public override void Visit(MethodSyntax pNode)
        { 
            if (!pNode.External && pNode.ReturnValues.Count > 0 && !ValidateReturnPaths(pNode))
                Compiler.ReportError(CompilerErrorType.PathNoReturnValue, pNode);
            base.Visit(pNode);
        }

        public override void Visit(DeclarationStatementSyntax pNode)
        {
            base.Visit(pNode);
            if (pNode.Identifier.GetType() == typeof(ArrayAccessExpressionSyntax))
                Compiler.ReportError(CompilerErrorType.InvalidArraySpecifier, pNode);
        }

        public override void Visit(ObjectInitializerExpressionSyntax pNode)
        {
            if (pNode.Value.Type.IsArray)
                Compiler.ReportError(CompilerErrorType.InvalidDefaultInitialize, pNode.Parent, pNode.Value.Type.ToString());
        }

        private bool ValidateReturnPaths(SyntaxNode pNode)
        {
            bool valid = false;
            switch (pNode)
            {
                case MethodSyntax m:
                    return ValidateReturnPaths(m.Body);

                case IfSyntax i:
                    valid = ValidateReturnPaths(i.Body);
                    if (i.Else != null) valid &= ValidateReturnPaths(i.Else);
                    else valid = false;

                    return valid;

                case ElseSyntax e:
                    if (e.If != null) return ValidateReturnPaths(e.If);
                    else return ValidateReturnPaths(e.Body);

                case WhileSyntax w:
                    return ValidateReturnPaths(w.Body);

                case BlockSyntax b:
                    valid = false;
                    foreach (var s in b.Statements)
                    {
                        valid |= ValidateReturnPaths(s);
                    }
                    return valid;


                case ReturnSyntax r:
                    return true;
            }

            return false;
        }
    }
}
