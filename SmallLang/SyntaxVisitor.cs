using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang.Syntax
{
    public abstract class SyntaxVisitor : IVistorContext
    {
        #region IVisitorContext
        readonly Dictionary<string, object> _context = new Dictionary<string, object>();
        public T GetValue<T>(string pKey, T pDefault)
        {
            if (!_context.ContainsKey(pKey)) return pDefault;
            return (T)_context[pKey];
        }

        public void AddValue(string pKey, object pValue)
        {
            if(!_context.ContainsKey(pKey)) _context.Add(pKey, pValue);
        }

        public void SetValue(string pKey, object pValue)
        {
            if (_context.ContainsKey(pKey)) _context[pKey] = pValue;
        }

        public void RemoveValue(string pKey)
        {
            if(_context.ContainsKey(pKey)) _context.Remove(pKey);
        }
        #endregion

        public virtual void Visit(SyntaxNode pNode) { throw new NotImplementedException(); }
        public virtual void Visit(BinaryExpressionSyntax pNode) { throw new NotImplementedException(); }
        public virtual void Visit(ExpressionSyntax pNode) { throw new NotImplementedException(); }
        public virtual void Visit(UnaryExpressionSyntax pNode) { throw new NotImplementedException(); }

        public virtual void Visit(AddAssignmentExpressionSyntax pNode)
        {
            pNode.Name.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(AdditionExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(AndExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(ArgumentExpressionSyntax pNode)
        {
            pNode.Value.Accept(this);
        }
        public virtual void Visit(ArrayAccessExpressionSyntax pNode)
        {
            pNode.Identifier.Accept(this);
            pNode.Index?.Accept(this);
        }
        public virtual void Visit(ArrayLiteralSyntax pNode)
        {
            pNode.Size.Accept(this);
        }
        public virtual void Visit(AssignmentStatementSyntax pNode)
        {
            pNode.Identifier.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(BitwiseAndExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(BitwiseOrExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(BlockSyntax pNode)
        {
            foreach (var s in pNode.Statements)
            {
                s.Accept(this);
            }
        }
        public virtual void Visit(BooleanLiteralSyntax pNode) { }
        public virtual void Visit(CastSyntax pNode)
        {
            pNode.Parameter.Accept(this);
            pNode.ReturnValue.Accept(this);
            pNode.Body.Accept(this);
        }
        public virtual void Visit(CatAssignmentExpressionSyntax pNode)
        {
            pNode.Name.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(ConcatenateExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(ConstSyntax pNode)
        {
            pNode.Identifier.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(DateLiteralSyntax pNode) { }
        public virtual void Visit(DeclarationStatementSyntax pNode)
        {
            pNode.Identifier.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(DecrementExpressionSyntax pNode)
        {
            pNode.Value.Accept(this);
        }
        public virtual void Visit(ObjectInitializerExpressionSyntax pNode) { }
        public virtual void Visit(DivAssignmentExpressionSyntax pNode)
        {
            pNode.Name.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(DivisionExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(ElseSyntax pNode)
        {
            pNode.If?.Accept(this);
            pNode.Body?.Accept(this);
        }
        public virtual void Visit(EqualsExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(ExplicitCastExpressionSyntax pNode)
        {
            pNode.Value.Accept(this);
        }
        public virtual void Visit(ExponentExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(FieldDeclarationSyntax pNode) { }
        public virtual void Visit(ForSyntax pNode)
        {
            pNode.Declaration.Accept(this);
            pNode.Condition.Accept(this);
            pNode.PostLoop.Accept(this);
            pNode.Body.Accept(this);
        }
        public virtual void Visit(FunctionInvocationSyntax pNode)
        {
            foreach(var a in pNode.Arguments)
            {
                a.Accept(this);
            }
        }
        public virtual void Visit(GreaterThanEqualsExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(GreaterThanExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(GroupAssignmentStatementSyntax pNode)
        {
            pNode.Identifier.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(GroupDeclarationStatementSyntax pNode)
        {
            pNode.Identifier.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(IdentifierGroupSyntax pNode)
        {
            foreach(var i in pNode.Identifiers)
            {
                i.Accept(this);
            }
        }
        public virtual void Visit(IdentifierSyntax pNode) { }
        public virtual void Visit(IfSyntax pNode)
        {
            pNode.Condition.Accept(this);
            pNode.Body.Accept(this);
            pNode.Else?.Accept(this);
        }
        public virtual void Visit(ImplicitCastExpressionSyntax pNode)
        {
            pNode.Value.Accept(this);
        }
        public virtual void Visit(ImportSyntax pNode) { }
        public virtual void Visit(IncrementExpressionSyntax pNode)
        {
            pNode.Value.Accept(this);
        }
        public virtual void Visit(LengthQueryExpressionSyntax pNode)
        {
            pNode.Array.Accept(this);
        }
        public virtual void Visit(LessThanEqualsExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(LessThanExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(LShiftExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(MemberAccessSyntax pNode)
        {
            pNode.Name.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(MemberIdentifierSyntax pNode) { }
        public virtual void Visit(MethodSyntax pNode)
        {
            foreach(var p in pNode.Parameters)
            {
                p.Accept(this);
            }
            foreach(var r in pNode.ReturnValues)
            {
                r.Accept(this);
            }
            pNode.Body.Accept(this);
        }
        public virtual void Visit(ModExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(MulAssignmentExpressionSyntax pNode)
        {
            pNode.Name.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(MultiplicationExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(NamespaceIdentifierSyntax pNode) { }
        public virtual void Visit(NegateExpressionSyntax pNode)
        {
            pNode.Value.Accept(this);
        }
        public virtual void Visit(NotEqualsExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(NotExpressionSyntax pNode)
        {
            pNode.Value.Accept(this);
        }
        public virtual void Visit(NumericLiteralSyntax pNode) { }
        public virtual void Visit(OrExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(ParameterSyntax pNode) { }
        public virtual void Visit(ReturnSyntax pNode)
        {
            foreach(var r in pNode.Values)
            {
                r.Accept(this);
            }
        }
        public virtual void Visit(ReturnValueSyntax pNode) { }
        public virtual void Visit(RShiftExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(StringLiteralSyntax pNode) { }
        public virtual void Visit(StructSyntax pNode)
        {
            foreach(var f in pNode.Fields)
            {
                f.Accept(this);
            }
            pNode.Initializer?.Accept(this);
        }
        public virtual void Visit(SubAssignmentExpressionSyntax pNode)
        {
            pNode.Name.Accept(this);
            pNode.Value.Accept(this);
        }
        public virtual void Visit(SubtractionExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(TernaryExpressionSyntax pNode)
        {
            pNode.Left.Accept(this);
            pNode.Center.Accept(this);
            pNode.Right.Accept(this);
        }
        public virtual void Visit(ValueDiscardSyntax pNode) { }
        public virtual void Visit(WhileSyntax pNode)
        {
            pNode.Condition.Accept(this);
            pNode.Body.Accept(this);
        }
        public virtual void Visit(WorkspaceSyntax pNode)
        {
            foreach (var i in pNode.Imports)
            {
                i.Accept(this);
            }
            foreach (var s in pNode.Structs)
            {
                s.Accept(this);
            }
            foreach(var m in pNode.Methods)
            {
                m.Accept(this);
            }
            foreach(var c in pNode.Casts)
            {
                c.Accept(this);
            }
        }
    }
}
