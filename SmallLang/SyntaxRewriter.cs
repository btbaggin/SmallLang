using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang.Syntax
{
    public abstract class SyntaxRewriter : IVistorContext
    {
        #region IVisitorContext
        readonly Dictionary<string, Stack<object>> _context = new Dictionary<string, Stack<object>>();
        public T GetValue<T>(string pKey, T pDefault)
        {
            if (!_context.ContainsKey(pKey) || _context[pKey].Count == 0) return pDefault;
            return (T)_context[pKey].Peek();
        }

        public void AddValue(string pKey, object pValue)
        {
            if (!_context.ContainsKey(pKey)) _context.Add(pKey, new Stack<object>());
            _context[pKey].Push(pValue);
        }

        public void SetValue(string pKey, object pValue)
        {
            if (_context.ContainsKey(pKey))
            {
                if(_context[pKey].Count > 0) _context[pKey].Pop();
                _context[pKey].Push(pValue);
            }
        }

        public void RemoveValue(string pKey)
        {
            if (_context.ContainsKey(pKey)) _context[pKey].Pop();
        }
        #endregion

        public virtual SyntaxNode Visit(SyntaxNode pNode) { throw new NotImplementedException(); }
        public virtual SyntaxNode Visit(BinaryExpressionSyntax pNode) { throw new NotImplementedException(); }
        public virtual SyntaxNode Visit(ExpressionSyntax pNode) { throw new NotImplementedException(); }
        public virtual SyntaxNode Visit(UnaryExpressionSyntax pNode) { throw new NotImplementedException(); }

        public virtual SyntaxNode Visit(AddAssignmentExpressionSyntax pNode)
        {
            return SyntaxFactory.AddAssignmentExpression(pNode.Name.Accept<IdentifierSyntax>(this),
                                                         pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(AdditionExpressionSyntax pNode)
        {
            return SyntaxFactory.AdditionExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                    pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(AndExpressionSyntax pNode)
        {
            return SyntaxFactory.AndExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                               pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ArgumentExpressionSyntax pNode)
        {
            return SyntaxFactory.ArgumentExpression(pNode.IsRef, pNode.Value.Accept<ExpressionSyntax>(this));
        }
        public virtual SyntaxNode Visit(ArrayAccessExpressionSyntax pNode)
        {
            return SyntaxFactory.ArrayAccessExpression(pNode.Identifier.Accept<IdentifierSyntax>(this),
                                                       pNode.Index?.Accept<ExpressionSyntax>(this));
        }
        public virtual SyntaxNode Visit(ArrayLiteralSyntax pNode)
        {
            return SyntaxFactory.ArrayLiteral(pNode.Size.Accept<ExpressionSyntax>(this));
        }
        public virtual SyntaxNode Visit(AssignmentStatementSyntax pNode)
        {
            return SyntaxFactory.AssignmentStatement(pNode.Identifier.Accept<IdentifierSyntax>(this),
                                                     pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(BitwiseAndExpressionSyntax pNode)
        {
            return SyntaxFactory.BitwiseAndExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                      pNode.Right.Accept<ExpressionSyntax>(this));
        }
        public virtual SyntaxNode Visit(BitwiseOrExpressionSyntax pNode)
        {
            return SyntaxFactory.BitwiseOrExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                     pNode.Right.Accept<ExpressionSyntax>(this));
        }
        public virtual SyntaxNode Visit(BlockSyntax pNode)
        {
            List<SyntaxNode> statements = new List<SyntaxNode>(pNode.Statements.Count);
            foreach (var s in pNode.Statements)
            {
                statements.Add(s.Accept<SyntaxNode>(this));
            }
            return SyntaxFactory.Block(statements).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(BooleanLiteralSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(CastSyntax pNode)
        {
            return SyntaxFactory.Cast(pNode.Parameter.Accept<ParameterSyntax>(this),
                                      pNode.ReturnValue.Accept<ReturnValueSyntax>(this),
                                      pNode.Body.Accept<BlockSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(CatAssignmentExpressionSyntax pNode)
        {
            return SyntaxFactory.CatAssignmentExpression(pNode.Name.Accept<IdentifierSyntax>(this),
                                                         pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ConcatenateExpressionSyntax pNode)
        {
            return SyntaxFactory.ConcatenateExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                       pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ConstSyntax pNode)
        {
            return SyntaxFactory.Const(pNode.Identifier.Accept<IdentifierSyntax>(this),
                                       pNode.Value.Accept<IdentifierSyntax>(this));
        }
        public virtual SyntaxNode Visit(DateLiteralSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(DeclarationStatementSyntax pNode)
        {
            return SyntaxFactory.DeclarationStatement(pNode.Identifier.Accept<IdentifierSyntax>(this),
                                                      pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(DecrementExpressionSyntax pNode)
        {
            return SyntaxFactory.DecrementExpression(pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ObjectInitializerExpressionSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(DivAssignmentExpressionSyntax pNode)
        {
            return SyntaxFactory.DivAssignmentExpression(pNode.Name.Accept<IdentifierSyntax>(this),
                                                         pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(DivisionExpressionSyntax pNode)
        {
            return SyntaxFactory.DivisionExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                    pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ElseSyntax pNode)
        {
            return SyntaxFactory.Else(pNode.If?.Accept<IfSyntax>(this),
                                      pNode.Body?.Accept<BlockSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(EqualsExpressionSyntax pNode)
        {
            return SyntaxFactory.EqualsExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                  pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ExplicitCastExpressionSyntax pNode)
        {
            return SyntaxFactory.ExplicitCastExpression(pNode.Value.Accept<ExpressionSyntax>(this), pNode.Type).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ExponentExpressionSyntax pNode)
        {
            return SyntaxFactory.ExponentExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                    pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(FieldDeclarationSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(ForSyntax pNode)
        {
            return SyntaxFactory.For(pNode.Declaration.Accept<DeclarationStatementSyntax>(this),
                                     pNode.Condition.Accept<ExpressionSyntax>(this),
                                     pNode.PostLoop.Accept<ExpressionSyntax>(this),
                                     pNode.Body.Accept<BlockSyntax>(this));
        }
        public virtual SyntaxNode Visit(FunctionInvocationSyntax pNode)
        {
            List<ArgumentExpressionSyntax> arguments = new List<ArgumentExpressionSyntax>();
            foreach (var a in pNode.Arguments)
            {
                arguments.Add(a.Accept<ArgumentExpressionSyntax>(this));
            }
            return SyntaxFactory.FunctionInvocation(pNode.Value, arguments).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(GreaterThanEqualsExpressionSyntax pNode)
        {
            return SyntaxFactory.GreaterThanEqualsExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                             pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(GreaterThanExpressionSyntax pNode)
        {
            return SyntaxFactory.GreaterThanExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                       pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(GroupAssignmentStatementSyntax pNode)
        {
            return SyntaxFactory.GroupAssignmentStatement(pNode.Identifier.Accept<IdentifierGroupSyntax>(this),
                                                          pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(GroupDeclarationStatementSyntax pNode)
        {
            return SyntaxFactory.GroupAssignmentStatement(pNode.Identifier.Accept<IdentifierGroupSyntax>(this),
                                                          pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(IdentifierGroupSyntax pNode)
        {
            List<IdentifierSyntax> identifiers = new List<IdentifierSyntax>();
            foreach (var i in pNode.Identifiers)
            {
                identifiers.Add(i.Accept<IdentifierSyntax>(this));
            }
            return SyntaxFactory.IdentifierGroup(identifiers).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(IdentifierSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(IfSyntax pNode)
        {
            return SyntaxFactory.If(pNode.Condition.Accept<ExpressionSyntax>(this),
                                    pNode.Body.Accept<BlockSyntax>(this),
                                    pNode.Else?.Accept<ElseSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ImplicitCastExpressionSyntax pNode)
        {
            return SyntaxFactory.ImplicitCast(pNode.Value.Accept<ExpressionSyntax>(this), pNode.Type).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ImportSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(IncrementExpressionSyntax pNode)
        {
            return SyntaxFactory.IncrementExpression(pNode.Value.Accept<IdentifierSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(LengthQueryExpressionSyntax pNode)
        {
            return SyntaxFactory.LengthQueryExpression(pNode.Array.Accept<IdentifierSyntax>(this));
        }
        public virtual SyntaxNode Visit(LessThanEqualsExpressionSyntax pNode)
        {
            return SyntaxFactory.LessThanEqualsExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                          pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(LessThanExpressionSyntax pNode)
        {
            return SyntaxFactory.LessThanExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                    pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(LShiftExpressionSyntax pNode)
        {
            return SyntaxFactory.LShiftExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                    pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(MemberAccessSyntax pNode)
        {
            return SyntaxFactory.MemberAccess(pNode.Name.Accept<IdentifierSyntax>(this),
                                              pNode.Value.Accept<IdentifierSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(MethodSyntax pNode)
        {
            List<ParameterSyntax> parameters = new List<ParameterSyntax>(pNode.Parameters.Count);
            foreach (var p in pNode.Parameters)
            {
                parameters.Add(p.Accept<ParameterSyntax>(this));
            }

            List<ReturnValueSyntax> returnValues = new List<ReturnValueSyntax>(pNode.ReturnValues.Count);
            foreach (var r in pNode.ReturnValues)
            {
                returnValues.Add(r.Accept<ReturnValueSyntax>(this));
            }

            return SyntaxFactory.Method(pNode.Name, parameters, returnValues, pNode.Body.Accept<BlockSyntax>(this), pNode.External).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ModExpressionSyntax pNode)
        {
            return SyntaxFactory.ModExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                               pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(MulAssignmentExpressionSyntax pNode)
        {
            return SyntaxFactory.MulAssignmentExpression(pNode.Name.Accept<IdentifierSyntax>(this),
                                                         pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(MultiplicationExpressionSyntax pNode)
        {
            return SyntaxFactory.MultiplicationExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                          pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(NegateExpressionSyntax pNode)
        {
            return SyntaxFactory.NegateExpression(pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(NotEqualsExpressionSyntax pNode)
        {
            return SyntaxFactory.NotEqualsExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                     pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(NotExpressionSyntax pNode)
        {
            return SyntaxFactory.NotExpression(pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(NumericLiteralSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(OrExpressionSyntax pNode)
        {
            return SyntaxFactory.OrExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                              pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ParameterSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(ReturnSyntax pNode)
        {
            List<ExpressionSyntax> values = new List<ExpressionSyntax>();
            foreach (var r in pNode.Values)
            {
                values.Add(r.Accept<ExpressionSyntax>(this));
            }

            return SyntaxFactory.Return(values).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ReturnValueSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(RShiftExpressionSyntax pNode)
        {
            return SyntaxFactory.RShiftExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                    pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(StringLiteralSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(StructSyntax pNode)
        {
            List<FieldDeclarationSyntax> fields = new List<FieldDeclarationSyntax>(pNode.Fields.Count);
            foreach(var f in pNode.Fields)
            {
                fields.Add(f.Accept<FieldDeclarationSyntax>(this));
            }
            return SyntaxFactory.Struct(pNode.Name, pNode.Prefix, pNode.TypeArgs, fields, pNode.Initializer?.Accept<BlockSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(SubAssignmentExpressionSyntax pNode)
        {
            return SyntaxFactory.SubAssignmentExpression(pNode.Name.Accept<IdentifierSyntax>(this),
                                                         pNode.Value.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(SubtractionExpressionSyntax pNode)
        {
            return SyntaxFactory.SubtractionExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                       pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(TernaryExpressionSyntax pNode)
        {
            return SyntaxFactory.TernaryExpression(pNode.Left.Accept<ExpressionSyntax>(this),
                                                   pNode.Center.Accept<ExpressionSyntax>(this),
                                                   pNode.Right.Accept<ExpressionSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(ValueDiscardSyntax pNode)
        {
            return pNode;
        }
        public virtual SyntaxNode Visit(WhileSyntax pNode)
        {
            return SyntaxFactory.While(pNode.Condition.Accept<ExpressionSyntax>(this),
                                       pNode.Body.Accept<BlockSyntax>(this)).WithAttributes(pNode);
        }
        public virtual SyntaxNode Visit(WorkspaceSyntax pNode)
        {
            List<ImportSyntax> imports = new List<ImportSyntax>(pNode.Imports.Count);
            foreach (var i in pNode.Imports)
            {
                imports.Add(i.Accept<ImportSyntax>(this));
            }

            List<MethodSyntax> methods = new List<MethodSyntax>(pNode.Methods.Count);
            foreach (var m in pNode.Methods)
            {
                methods.Add(m.Accept<MethodSyntax>(this));
            }

            List<CastSyntax> casts = new List<CastSyntax>(pNode.Casts.Count);
            foreach (var c in pNode.Casts)
            {
                casts.Add(c.Accept<CastSyntax>(this));
            }

            List<StructSyntax> structs = new List<StructSyntax>(pNode.Structs.Count);
            foreach (var s in pNode.Structs)
            {
                structs.Add(s.Accept<StructSyntax>(this));
            }

            List<ConstSyntax> consts = new List<Syntax.ConstSyntax>(pNode.Consts.Count);
            foreach(var c in pNode.Consts)
            {
                consts.Add(c.Accept<ConstSyntax>(this));
            }
            return SyntaxFactory.Workspace(imports, methods, casts, structs, consts).WithAttributes(pNode);
        }
    }
}
