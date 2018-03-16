using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

using SmallLang.Emitting;

namespace SmallLang.Syntax
{
    public abstract class SyntaxNode
    {
        #region Properties
        internal bool LoadAddress { get; set; }

        public abstract SyntaxKind Kind { get; }

        public SyntaxNode Parent { get; internal set; }

        List<Annotation> _annotations = new List<Annotation>();
        public List<Annotation> Annotations { get { return _annotations; } }

        public TextSpan Span { get; private set; }
        #endregion

        public void AddAnnotation(Annotation pAnnotation)
        {
            Annotations.Add(pAnnotation);
        }

        public void Accept(SyntaxVisitor pVisitor)
        {
            if (Kind == SyntaxKind.AddAssignmentExpression)
                pVisitor.Visit((AddAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.AdditionExpression)
                pVisitor.Visit((AdditionExpressionSyntax)this);

            if (Kind == SyntaxKind.AndExpression)
                pVisitor.Visit((AndExpressionSyntax)this);

            if (Kind == SyntaxKind.ArgumentExpression)
                pVisitor.Visit((ArgumentExpressionSyntax)this);

            if (Kind == SyntaxKind.ArrayAccessExpression)
                pVisitor.Visit((ArrayAccessExpressionSyntax)this);

            if (Kind == SyntaxKind.ArrayLiteral)
                pVisitor.Visit((ArrayLiteralSyntax)this);

            if (Kind == SyntaxKind.AssignmentStatement)
                pVisitor.Visit((AssignmentStatementSyntax)this);

            if (Kind == SyntaxKind.BitwiseAndExpression)
                pVisitor.Visit((BitwiseAndExpressionSyntax)this);

            if (Kind == SyntaxKind.BitwiseOrExpression)
                pVisitor.Visit((BitwiseOrExpressionSyntax)this);

            if (Kind == SyntaxKind.Block)
                pVisitor.Visit((BlockSyntax)this);

            if (Kind == SyntaxKind.BooleanLiteral)
                pVisitor.Visit((BooleanLiteralSyntax)this);

            if (Kind == SyntaxKind.Cast)
                pVisitor.Visit((CastSyntax)this);

            if (Kind == SyntaxKind.CatAssignmentExpression)
                pVisitor.Visit((CatAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.ConcatenateExpression)
                pVisitor.Visit((ConcatenateExpressionSyntax)this);

            if (Kind == SyntaxKind.Const)
                pVisitor.Visit((ConstSyntax)this);

            if (Kind == SyntaxKind.DateLiteral)
                pVisitor.Visit((DateLiteralSyntax)this);

            if (Kind == SyntaxKind.DeclarationStatement)
                pVisitor.Visit((DeclarationStatementSyntax)this);

            if (Kind == SyntaxKind.DecrementExpression)
                pVisitor.Visit((DecrementExpressionSyntax)this);

            if (Kind == SyntaxKind.DefaultInitializerExpression)
                pVisitor.Visit((ObjectInitializerExpressionSyntax)this);

            if (Kind == SyntaxKind.DivAssignmentExpression)
                pVisitor.Visit((DivAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.DivisionExpression)
                pVisitor.Visit((DivisionExpressionSyntax)this);

            if (Kind == SyntaxKind.Else)
                pVisitor.Visit((ElseSyntax)this);

            if (Kind == SyntaxKind.EqualsExpression)
                pVisitor.Visit((EqualsExpressionSyntax)this);

            if (Kind == SyntaxKind.ExplicitCastExpression)
                pVisitor.Visit((ExplicitCastExpressionSyntax)this);

            if (Kind == SyntaxKind.ExponentExpression)
                pVisitor.Visit((ExponentExpressionSyntax)this);

            if (Kind == SyntaxKind.SentinelExpression)
                pVisitor.Visit((SentinelExpressionSyntax)this);

            if (Kind == SyntaxKind.FieldDeclaration)
                pVisitor.Visit((FieldDeclarationSyntax)this);

            if (Kind == SyntaxKind.For)
                pVisitor.Visit((ForSyntax)this);

            if (Kind == SyntaxKind.FunctionInvocation)
                pVisitor.Visit((FunctionInvocationSyntax)this);

            if (Kind == SyntaxKind.GreaterThanEqualsExpression)
                pVisitor.Visit((GreaterThanEqualsExpressionSyntax)this);

            if (Kind == SyntaxKind.GreaterThanExpression)
                pVisitor.Visit((GreaterThanExpressionSyntax)this);

            if (Kind == SyntaxKind.GroupAssignmentStatement)
                pVisitor.Visit((GroupAssignmentStatementSyntax)this);

            if (Kind == SyntaxKind.GroupDeclarationStatement)
                pVisitor.Visit((GroupDeclarationStatementSyntax)this);

            if (Kind == SyntaxKind.IdentifierGroup)
                pVisitor.Visit((IdentifierGroupSyntax)this);

            if (Kind == SyntaxKind.Identifier)
                pVisitor.Visit((IdentifierSyntax)this);

            if (Kind == SyntaxKind.If)
                pVisitor.Visit((IfSyntax)this);

            if (Kind == SyntaxKind.ImplicitCastExpression)
                pVisitor.Visit((ImplicitCastExpressionSyntax)this);

            if (Kind == SyntaxKind.Import)
                pVisitor.Visit((ImportSyntax)this);

            if (Kind == SyntaxKind.IncrementExpression)
                pVisitor.Visit((IncrementExpressionSyntax)this);

            if (Kind == SyntaxKind.LengthQuery)
                pVisitor.Visit((LengthQueryExpressionSyntax)this);

            if (Kind == SyntaxKind.LessThanEqualsExpression)
                pVisitor.Visit((LessThanEqualsExpressionSyntax)this);

            if (Kind == SyntaxKind.LessThanExpression)
                pVisitor.Visit((LessThanExpressionSyntax)this);

            if (Kind == SyntaxKind.LShiftExpression)
                pVisitor.Visit((LShiftExpressionSyntax)this);

            if (Kind == SyntaxKind.MemberAccess)
                pVisitor.Visit((MemberAccessSyntax)this);

            if (Kind == SyntaxKind.MemberIdentifier)
                pVisitor.Visit((MemberIdentifierSyntax)this);

            if (Kind == SyntaxKind.Method)
                pVisitor.Visit((MethodSyntax)this);

            if (Kind == SyntaxKind.ModExpression)
                pVisitor.Visit((ModExpressionSyntax)this);

            if (Kind == SyntaxKind.MulAssignmentExpression)
                pVisitor.Visit((MulAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.MultiplicationExpression)
                pVisitor.Visit((MultiplicationExpressionSyntax)this);

            if (Kind == SyntaxKind.NamespaceIdentifier)
                pVisitor.Visit((NamespaceIdentifierSyntax)this);

            if (Kind == SyntaxKind.NegateExpression)
                pVisitor.Visit((NegateExpressionSyntax)this);

            if (Kind == SyntaxKind.NotEqualsExpression)
                pVisitor.Visit((NotEqualsExpressionSyntax)this);

            if (Kind == SyntaxKind.NotExpression)
                pVisitor.Visit((NotExpressionSyntax)this);

            if (Kind == SyntaxKind.NumericLiteral)
                pVisitor.Visit((NumericLiteralSyntax)this);

            if (Kind == SyntaxKind.OrExpression)
                pVisitor.Visit((OrExpressionSyntax)this);

            if (Kind == SyntaxKind.Parameter)
                pVisitor.Visit((ParameterSyntax)this);

            if (Kind == SyntaxKind.Return)
                pVisitor.Visit((ReturnSyntax)this);

            if (Kind == SyntaxKind.ReturnValue)
                pVisitor.Visit((ReturnValueSyntax)this);

            if (Kind == SyntaxKind.RShiftExpression)
                pVisitor.Visit((RShiftExpressionSyntax)this);

            if (Kind == SyntaxKind.StringLiteral)
                pVisitor.Visit((StringLiteralSyntax)this);

            if (Kind == SyntaxKind.Struct)
                pVisitor.Visit((StructSyntax)this);

            if (Kind == SyntaxKind.SubAssignmentExpression)
                pVisitor.Visit((SubAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.SubtractionExpression)
                pVisitor.Visit((SubtractionExpressionSyntax)this);

            if (Kind == SyntaxKind.TernaryExpression)
                pVisitor.Visit((TernaryExpressionSyntax)this);

            if (Kind == SyntaxKind.ValueDiscard)
                pVisitor.Visit((ValueDiscardSyntax)this);

            if (Kind == SyntaxKind.While)
                pVisitor.Visit((WhileSyntax)this);

            if (Kind == SyntaxKind.Workspace)
                pVisitor.Visit((WorkspaceSyntax)this);
        }

        public T Accept<T>(SyntaxRewriter pVisitor) where T : SyntaxNode
        {
            if (Kind == SyntaxKind.AddAssignmentExpression)
                return (T)pVisitor.Visit((AddAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.AdditionExpression)
                return (T)pVisitor.Visit((AdditionExpressionSyntax)this);

            if (Kind == SyntaxKind.AndExpression)
                return (T)pVisitor.Visit((AndExpressionSyntax)this);

            if (Kind == SyntaxKind.ArgumentExpression)
                return (T)pVisitor.Visit((ArgumentExpressionSyntax)this);

            if (Kind == SyntaxKind.ArrayAccessExpression)
                return (T)pVisitor.Visit((ArrayAccessExpressionSyntax)this);

            if (Kind == SyntaxKind.ArrayLiteral)
                return (T)pVisitor.Visit((ArrayLiteralSyntax)this);

            if (Kind == SyntaxKind.AssignmentStatement)
                return (T)pVisitor.Visit((AssignmentStatementSyntax)this);

            if (Kind == SyntaxKind.BitwiseAndExpression)
                return (T)pVisitor.Visit((BitwiseAndExpressionSyntax)this);

            if (Kind == SyntaxKind.BitwiseOrExpression)
                return (T)pVisitor.Visit((BitwiseOrExpressionSyntax)this);

            if (Kind == SyntaxKind.Block)
                return (T)pVisitor.Visit((BlockSyntax)this);

            if (Kind == SyntaxKind.BooleanLiteral)
                return (T)pVisitor.Visit((BooleanLiteralSyntax)this);

            if (Kind == SyntaxKind.Cast)
                return (T)pVisitor.Visit((CastSyntax)this);

            if (Kind == SyntaxKind.CatAssignmentExpression)
                return (T)pVisitor.Visit((CatAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.ConcatenateExpression)
                return (T)pVisitor.Visit((ConcatenateExpressionSyntax)this);

            if (Kind == SyntaxKind.Const)
                return (T)pVisitor.Visit((ConstSyntax)this);

            if (Kind == SyntaxKind.DateLiteral)
                return (T)pVisitor.Visit((DateLiteralSyntax)this);

            if (Kind == SyntaxKind.DeclarationStatement)
                return (T)pVisitor.Visit((DeclarationStatementSyntax)this);

            if (Kind == SyntaxKind.DecrementExpression)
                return (T)pVisitor.Visit((DecrementExpressionSyntax)this);

            if (Kind == SyntaxKind.DefaultInitializerExpression)
                return (T)pVisitor.Visit((ObjectInitializerExpressionSyntax)this);

            if (Kind == SyntaxKind.DivAssignmentExpression)
                return (T)pVisitor.Visit((DivAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.DivisionExpression)
                return (T)pVisitor.Visit((DivisionExpressionSyntax)this);

            if (Kind == SyntaxKind.Else)
                return (T)pVisitor.Visit((ElseSyntax)this);

            if (Kind == SyntaxKind.EqualsExpression)
                return (T)pVisitor.Visit((EqualsExpressionSyntax)this);

            if (Kind == SyntaxKind.ExplicitCastExpression)
                return (T)pVisitor.Visit((ExplicitCastExpressionSyntax)this);

            if (Kind == SyntaxKind.ExponentExpression)
                return (T)pVisitor.Visit((ExponentExpressionSyntax)this);

            if (Kind == SyntaxKind.SentinelExpression)
                return (T)pVisitor.Visit((SentinelExpressionSyntax)this);

            if (Kind == SyntaxKind.FieldDeclaration)
                return (T)pVisitor.Visit((FieldDeclarationSyntax)this);

            if (Kind == SyntaxKind.For)
                return (T)pVisitor.Visit((ForSyntax)this);

            if (Kind == SyntaxKind.FunctionInvocation)
                return (T)pVisitor.Visit((FunctionInvocationSyntax)this);

            if (Kind == SyntaxKind.GreaterThanEqualsExpression)
                return (T)pVisitor.Visit((GreaterThanEqualsExpressionSyntax)this);

            if (Kind == SyntaxKind.GreaterThanExpression)
                return (T)pVisitor.Visit((GreaterThanExpressionSyntax)this);

            if (Kind == SyntaxKind.GroupAssignmentStatement)
                return (T)pVisitor.Visit((GroupAssignmentStatementSyntax)this);

            if (Kind == SyntaxKind.GroupDeclarationStatement)
                return (T)pVisitor.Visit((GroupDeclarationStatementSyntax)this);

            if (Kind == SyntaxKind.IdentifierGroup)
                return (T)pVisitor.Visit((IdentifierGroupSyntax)this);

            if (Kind == SyntaxKind.Identifier)
                return (T)pVisitor.Visit((IdentifierSyntax)this);

            if (Kind == SyntaxKind.If)
                return (T)pVisitor.Visit((IfSyntax)this);

            if (Kind == SyntaxKind.ImplicitCastExpression)
                return (T)pVisitor.Visit((ImplicitCastExpressionSyntax)this);

            if (Kind == SyntaxKind.Import)
                return (T)pVisitor.Visit((ImportSyntax)this);

            if (Kind == SyntaxKind.IncrementExpression)
                return (T)pVisitor.Visit((IncrementExpressionSyntax)this);

            if (Kind == SyntaxKind.LengthQuery)
                return (T)pVisitor.Visit((LengthQueryExpressionSyntax)this);

            if (Kind == SyntaxKind.LessThanEqualsExpression)
                return (T)pVisitor.Visit((LessThanEqualsExpressionSyntax)this);

            if (Kind == SyntaxKind.LessThanExpression)
                return (T)pVisitor.Visit((LessThanExpressionSyntax)this);

            if (Kind == SyntaxKind.LShiftExpression)
                return (T)pVisitor.Visit((LShiftExpressionSyntax)this);

            if (Kind == SyntaxKind.MemberAccess)
                return (T)pVisitor.Visit((MemberAccessSyntax)this);

            if (Kind == SyntaxKind.MemberIdentifier)
                return (T)pVisitor.Visit((MemberIdentifierSyntax)this);

            if (Kind == SyntaxKind.Method)
                return (T)pVisitor.Visit((MethodSyntax)this);

            if (Kind == SyntaxKind.ModExpression)
                return (T)pVisitor.Visit((ModExpressionSyntax)this);

            if (Kind == SyntaxKind.MulAssignmentExpression)
                return (T)pVisitor.Visit((MulAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.MultiplicationExpression)
                return (T)pVisitor.Visit((MultiplicationExpressionSyntax)this);

            if (Kind == SyntaxKind.NamespaceIdentifier)
                return (T)pVisitor.Visit((NamespaceIdentifierSyntax)this);

            if (Kind == SyntaxKind.NegateExpression)
                return (T)pVisitor.Visit((NegateExpressionSyntax)this);

            if (Kind == SyntaxKind.NotEqualsExpression)
                return (T)pVisitor.Visit((NotEqualsExpressionSyntax)this);

            if (Kind == SyntaxKind.NotExpression)
                return (T)pVisitor.Visit((NotExpressionSyntax)this);

            if (Kind == SyntaxKind.NumericLiteral)
                return (T)pVisitor.Visit((NumericLiteralSyntax)this);

            if (Kind == SyntaxKind.OrExpression)
                return (T)pVisitor.Visit((OrExpressionSyntax)this);

            if (Kind == SyntaxKind.Parameter)
                return (T)pVisitor.Visit((ParameterSyntax)this);

            if (Kind == SyntaxKind.Return)
                return (T)pVisitor.Visit((ReturnSyntax)this);

            if (Kind == SyntaxKind.ReturnValue)
                return (T)pVisitor.Visit((ReturnValueSyntax)this);

            if (Kind == SyntaxKind.RShiftExpression)
                return (T)pVisitor.Visit((RShiftExpressionSyntax)this);

            if (Kind == SyntaxKind.StringLiteral)
                return (T)pVisitor.Visit((StringLiteralSyntax)this);

            if (Kind == SyntaxKind.Struct)
                return (T)pVisitor.Visit((StructSyntax)this);

            if (Kind == SyntaxKind.SubAssignmentExpression)
                return (T)pVisitor.Visit((SubAssignmentExpressionSyntax)this);

            if (Kind == SyntaxKind.SubtractionExpression)
                return (T)pVisitor.Visit((SubtractionExpressionSyntax)this);

            if (Kind == SyntaxKind.TernaryExpression)
                return (T)pVisitor.Visit((TernaryExpressionSyntax)this);

            if (Kind == SyntaxKind.ValueDiscard)
                return (T)pVisitor.Visit((ValueDiscardSyntax)this);

            if (Kind == SyntaxKind.While)
                return (T)pVisitor.Visit((WhileSyntax)this);

            if (Kind == SyntaxKind.Workspace)
                return (T)pVisitor.Visit((WorkspaceSyntax)this);

            return null;
        }

        public virtual SyntaxNode WithAttributes(SyntaxNode pNode)
        {
            _annotations = pNode.Annotations;
            Span = pNode.Span;
            return this;
        }

        public T SetSpan<T>(TextSpan pSpan) where T: SyntaxNode
        {
            Span = pSpan;
            return (T)this;
        }

        public abstract SmallType Type { get; }
        public abstract void Emit(ILRunner pRunner);
    }
}
