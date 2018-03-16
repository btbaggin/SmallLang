using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Syntax;

namespace SmallLang.Parsing
{
    class GroupAssignmentSyntaxRewriter : SyntaxRewriter
    {
        Dictionary<string, IdentifierSyntax> _consts;
        public override SyntaxNode Visit(WorkspaceSyntax pNode)
        {
            _consts = new Dictionary<string, IdentifierSyntax>(StringComparer.OrdinalIgnoreCase);
            foreach(var c in pNode.Consts)
            {
                _consts.Add(c.Identifier.Value, c.Value);
            }
            return base.Visit(pNode);
        }
        public override SyntaxNode Visit(IdentifierSyntax pNode)
        {
            if(_consts.ContainsKey(pNode.Value))
            {
                return _consts[pNode.Value];
            }
            return base.Visit(pNode);
        }


        public override SyntaxNode Visit(GroupAssignmentStatementSyntax pNode)
        {
            if (pNode.Identifier.Identifiers.Count > 1)
            {
                List<SyntaxNode> statements = new List<SyntaxNode>();
                var exp = pNode.Value.Accept<ExpressionSyntax>(this);

                //FunctionInvocationSyntax will return null for type because it hasn't been bound to the call site yet
                var tempVarName = (exp.Type == null ? "obj" : exp.Type.ToString()) + exp.GetHashCode();
                var tempVar = SyntaxFactory.Identifier(tempVarName);
                statements.Add(SyntaxFactory.DeclarationStatement(tempVar, exp)); //Assign the temp var

                //Assign each actual variable from the temp var
                for (int i = 0; i < pNode.Identifier.Identifiers.Count; i++)
                {
                    if (pNode.Identifier.Identifiers[i].GetType() == typeof(ValueDiscardSyntax)) continue;

                    var iden = pNode.Identifier.Identifiers[i].Accept<IdentifierSyntax>(this);
                    if (exp.Type == SmallType.Undefined || exp.Type.IsTupleType)
                    {
                        var id = SyntaxFactory.Identifier("Item" + (i + 1).ToString());
                        statements.Add(SyntaxFactory.AssignmentStatement(iden, 
                                                                         SyntaxFactory.MemberAccess(SyntaxFactory.Identifier(tempVarName), 
                                                                                                   id)).WithAttributes(pNode));
                    }
                    else
                    {
                        statements.Add(SyntaxFactory.AssignmentStatement(iden, 
                                                                         SyntaxFactory.Identifier(tempVarName)).WithAttributes(pNode));
                    }
                }

                return SyntaxFactory.Block(statements).WithAttributes(pNode);
            }
            return base.Visit(pNode);
        }

        public override SyntaxNode Visit(GroupDeclarationStatementSyntax pNode)
        {
            if (pNode.Identifier.Identifiers.Count > 1)
            {
                List<SyntaxNode> statements = new List<SyntaxNode>();
                var exp = pNode.Value.Accept<ExpressionSyntax>(this);

                //FunctionInvocationSyntax will return null for type because it hasn't been bound to the call site yet
                var tempVarName = (exp.Type == null ? "obj" : exp.Type.ToString()) + exp.GetHashCode();
                var tempVar = SyntaxFactory.Identifier(tempVarName);
                statements.Add(SyntaxFactory.DeclarationStatement(tempVar, exp)); //Assign the temp var

                //Assign each actual variable from the temp var
                for (int i = 0; i < pNode.Identifier.Identifiers.Count; i++)
                {
                    if (pNode.Identifier.Identifiers[i].GetType() == typeof(ValueDiscardSyntax)) continue;

                    var iden = pNode.Identifier.Identifiers[i].Accept<IdentifierSyntax>(this);
                    if (exp.Type == SmallType.Undefined || exp.Type.IsTupleType)
                    {
                        var id = SyntaxFactory.Identifier("Item" + (i + 1).ToString());
                        statements.Add(SyntaxFactory.DeclarationStatement(iden, 
                                                                          SyntaxFactory.MemberAccess(SyntaxFactory.Identifier(tempVarName), 
                                                                                                     id)).WithAttributes(pNode));
                    }
                    else
                    {
                        statements.Add(SyntaxFactory.DeclarationStatement(iden, 
                                                                          SyntaxFactory.Identifier(tempVarName)).WithAttributes(pNode));
                    }
                }

                return SyntaxFactory.Block(statements).WithAttributes(pNode);
            }
            return base.Visit(pNode);
        }
    }
}
