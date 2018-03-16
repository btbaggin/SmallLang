using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Syntax
{
    public static class SyntaxFactory
    {
        internal static ExpressionSyntax AssignmentExpression(TokenType pToken, IdentifierSyntax pName, ExpressionSyntax pValue)
        {
            switch (pToken)
            {
                case TokenType.PlusEquals:
                    return new AddAssignmentExpressionSyntax(0, pName, pValue);

                case TokenType.PeriodPeriodEquals:
                    return new CatAssignmentExpressionSyntax(0, pName, pValue);

                case TokenType.SlashEquals:
                    return new DivAssignmentExpressionSyntax(0, pName, pValue);

                case TokenType.StarEquals:
                    return new MulAssignmentExpressionSyntax(0, pName, pValue);

                case TokenType.MinusEquals:
                    return new SubAssignmentExpressionSyntax(0, pName, pValue);

                default:
                    throw new Exception("Unknown assignment " + pToken.ToString());
            }
        }
        internal static BinaryExpressionSyntax BinaryExpression(int pPrecedence, TokenType pToken)
        {
            switch (pToken)
            {
                case TokenType.Addition:
                    return new AdditionExpressionSyntax(pPrecedence, null, null);

                case TokenType.Subtraction:
                    return new SubtractionExpressionSyntax(pPrecedence, null, null);

                case TokenType.Multiplication:
                    return new MultiplicationExpressionSyntax(pPrecedence, null, null);

                case TokenType.Division:
                    return new DivisionExpressionSyntax(pPrecedence, null, null);

                case TokenType.Exponent:
                    return new ExponentExpressionSyntax(pPrecedence, null, null);

                case TokenType.Mod:
                    return new ModExpressionSyntax(pPrecedence, null, null);

                case TokenType.Concatenate:
                    return new ConcatenateExpressionSyntax(pPrecedence, null, null);

                case TokenType.LessThan:
                    return new LessThanExpressionSyntax(pPrecedence, null, null);

                case TokenType.GreaterThan:
                    return new GreaterThanExpressionSyntax(pPrecedence, null, null);

                case TokenType.Equals:
                    return new EqualsExpressionSyntax(pPrecedence, null, null);

                case TokenType.NotEquals:
                    return new NotEqualsExpressionSyntax(pPrecedence, null, null);

                case TokenType.LessThanEquals:
                    return new LessThanEqualsExpressionSyntax(pPrecedence, null, null);

                case TokenType.GreaterThanEquals:
                    return new GreaterThanEqualsExpressionSyntax(pPrecedence, null, null);

                case TokenType.Or:
                    return new OrExpressionSyntax(pPrecedence, null, null);

                case TokenType.And:
                    return new AndExpressionSyntax(pPrecedence, null, null);

                case TokenType.BitwiseAnd:
                    return new BitwiseAndExpressionSyntax(pPrecedence, null, null);

                case TokenType.BitwiseOr:
                    return new BitwiseOrExpressionSyntax(pPrecedence, null, null);

                case TokenType.LShift:
                    return new LShiftExpressionSyntax(pPrecedence, null, null);

                case TokenType.RShift:
                    return new RShiftExpressionSyntax(pPrecedence, null, null);

                default:
                    throw new Exception("Unknown Expression Type " + pToken.ToString());
            }
        }

        internal static UnaryExpressionSyntax UnaryExpression(int pPrecedence, TokenType pToken)
        {
            switch (pToken)
            {
                case TokenType.Subtraction:
                    return new NegateExpressionSyntax(pPrecedence, null);

                case TokenType.Not:
                    return new NotExpressionSyntax(pPrecedence, null);

                case TokenType.ImplicitCast:
                    return new ImplicitCastExpressionSyntax(pPrecedence, null);

                case TokenType.Cast:
                    return new ExplicitCastExpressionSyntax(pPrecedence, null);

                case TokenType.PlusPlus:
                    return new IncrementExpressionSyntax(pPrecedence, null);

                case TokenType.MinusMinus:
                    return new DecrementExpressionSyntax(pPrecedence, null);

                default:
                    throw new Exception("Unknown Expression Type " + pToken.ToString());
            }
        }

        internal static TernaryExpressionSyntax TernaryExpression(int pPrecedence, TokenType pToken)
        {
            switch (pToken)
            {
                case TokenType.Ternary:
                    return new TernaryExpressionSyntax(pPrecedence, null, null, null);

                default:
                    throw new Exception("Unknown Expression Type " + pToken.ToString());
            }
        }

        public static AddAssignmentExpressionSyntax AddAssignmentExpression(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            return new AddAssignmentExpressionSyntax(0, pLeft, pRight);
        }

        public static AdditionExpressionSyntax AdditionExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var a = new AdditionExpressionSyntax(0, null, null);
            a.SetLeft(pLeft);
            a.SetRight(pRight);
            return a;
        }

        public static AndExpressionSyntax AndExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var a = new AndExpressionSyntax(0, null, null);
            a.SetLeft(pLeft);
            a.SetRight(pRight);
            return a;
        }

        public static BitwiseAndExpressionSyntax BitwiseAndExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var a = new BitwiseAndExpressionSyntax(0, null, null);
            a.SetLeft(pLeft);
            a.SetRight(pRight);
            return a;
        }

        public static BitwiseOrExpressionSyntax BitwiseOrExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var a = new BitwiseOrExpressionSyntax(0, null, null);
            a.SetLeft(pLeft);
            a.SetRight(pRight);
            return a;
        }

        public static CatAssignmentExpressionSyntax CatAssignmentExpression(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            return new CatAssignmentExpressionSyntax(0, pLeft, pRight);
        }

        public static ConcatenateExpressionSyntax ConcatenateExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var c = new ConcatenateExpressionSyntax(0, null, null);
            c.SetLeft(pLeft);
            c.SetRight(pRight);
            return c;
        }

        public static DecrementExpressionSyntax DecrementExpression(ExpressionSyntax pValue)
        {
            var d = new DecrementExpressionSyntax(0, null);
            d.SetValue(pValue);
            return d;
        }

        public static DivAssignmentExpressionSyntax DivAssignmentExpression(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            return new DivAssignmentExpressionSyntax(0, pLeft, pRight);
        }

        public static DivisionExpressionSyntax DivisionExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var d = new DivisionExpressionSyntax(0, null, null);
            d.SetLeft(pLeft);
            d.SetRight(pRight);
            return d;
        }

        public static EqualsExpressionSyntax EqualsExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var e = new EqualsExpressionSyntax(0, null, null);
            e.SetLeft(pLeft);
            e.SetRight(pRight);
            return e;
        }

        public static ExplicitCastExpressionSyntax ExplicitCastExpression(ExpressionSyntax pValue, SmallType pTargetType)
        {
            var e = new ExplicitCastExpressionSyntax(0, null);
            e.SetValue(pValue);
            e.TypeNamspace = pTargetType.Namespace;
            e.TypeName = pTargetType.Name;
            return e;
        }

        public static ExponentExpressionSyntax ExponentExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var e = new ExponentExpressionSyntax(0, null, null);
            e.SetLeft(pLeft);
            e.SetRight(pRight);
            return e;
        }

        public static GreaterThanEqualsExpressionSyntax GreaterThanEqualsExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var g = new GreaterThanEqualsExpressionSyntax(0, null, null);
            g.SetLeft(pLeft);
            g.SetRight(pRight);
            return g;
        }

        public static GreaterThanExpressionSyntax GreaterThanExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var g = new GreaterThanExpressionSyntax(0, null, null);
            g.SetLeft(pLeft);
            g.SetRight(pRight);
            return g;
        }

        public static ImplicitCastExpressionSyntax ImplicitCast(ExpressionSyntax pValue, SmallType pType)
        {
            var i = new ImplicitCastExpressionSyntax(0, null);
            i.SetValue(pValue);
            i.SetType(pType);
            return i;
        }

        public static IncrementExpressionSyntax IncrementExpression(ExpressionSyntax pValue)
        {
            var i = new IncrementExpressionSyntax(0, null);
            i.SetValue(pValue);
            return i;
        }

        public static LessThanEqualsExpressionSyntax LessThanEqualsExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var l = new LessThanEqualsExpressionSyntax(0, null, null);
            l.SetLeft(pLeft);
            l.SetRight(pRight);
            return l;
        }

        public static LessThanExpressionSyntax LessThanExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var l = new LessThanExpressionSyntax(0, null, null);
            l.SetLeft(pLeft);
            l.SetRight(pRight);
            return l;
        }

        public static LShiftExpressionSyntax LShiftExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var l = new LShiftExpressionSyntax(0, null, null);
            l.SetLeft(pLeft);
            l.SetRight(pRight);
            return l;
        }

        public static ModExpressionSyntax ModExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var m = new ModExpressionSyntax(0, null, null);
            m.SetLeft(pLeft);
            m.SetRight(pRight);
            return m;
        }

        public static MulAssignmentExpressionSyntax MulAssignmentExpression(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            return new MulAssignmentExpressionSyntax(0, pLeft, pRight);
        }

        public static MultiplicationExpressionSyntax MultiplicationExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var m = new MultiplicationExpressionSyntax(0, null, null);
            m.SetLeft(pLeft);
            m.SetRight(pRight);
            return m;
        }

        public static NegateExpressionSyntax NegateExpression(ExpressionSyntax pValue)
        {
            var n = new NegateExpressionSyntax(0, null);
            n.SetValue(pValue);
            return n;
        }

        public static NotEqualsExpressionSyntax NotEqualsExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var n = new NotEqualsExpressionSyntax(0, null, null);
            n.SetLeft(pLeft);
            n.SetRight(pRight);
            return n;
        }

        public static NotExpressionSyntax NotExpression(ExpressionSyntax pValue)
        {
            var n = new NotExpressionSyntax(0, null);
            n.SetValue(pValue);
            return n;
        }

        public static OrExpressionSyntax OrExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var o = new OrExpressionSyntax(0, null, null);
            o.SetLeft(pLeft);
            o.SetRight(pRight);
            return o;
        }

        public static RShiftExpressionSyntax RShiftExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var r = new RShiftExpressionSyntax(0, null, null);
            r.SetLeft(pLeft);
            r.SetRight(pRight);
            return r;
        }

        public static SubAssignmentExpressionSyntax SubAssignmentExpression(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            return new SubAssignmentExpressionSyntax(0, pLeft, pRight);
        }

        public static SubtractionExpressionSyntax SubtractionExpression(ExpressionSyntax pLeft, ExpressionSyntax pRight)
        {
            var s = new SubtractionExpressionSyntax(0, null, null);
            s.SetLeft(pLeft);
            s.SetRight(pRight);
            return s;
        }

        public static TernaryExpressionSyntax TernaryExpression(ExpressionSyntax pLeft, ExpressionSyntax pCenter, ExpressionSyntax pRight)
        {
            var t = new TernaryExpressionSyntax(0, null, null, null);
            t.SetLeft(pLeft);
            t.SetCenter(pCenter);
            t.SetRight(pRight);
            return t;
        }

        public static WorkspaceSyntax Workspace(IList<ImportSyntax> pImports,
                                                IList<MethodSyntax> pMethods,
                                                IList<CastSyntax> pCasts,
                                                IList<StructSyntax> pStructs,
                                                IList<ConstSyntax> pConsts)
        {
            return new WorkspaceSyntax(pImports, pMethods, pCasts, pStructs, pConsts);
        }

        public static ParameterSyntax Parameter(bool pIsRef, string pNamespace, string pName)
        {
            return new ParameterSyntax(pIsRef, pNamespace, pName);
        }

        public static ReturnValueSyntax ReturnValue(string pNamespace, string pValue)
        {
            return new ReturnValueSyntax(pNamespace, pValue);
        }

        public static BlockSyntax Block(IList<SyntaxNode> pStatements)
        {
            return new BlockSyntax(pStatements);
        }

        public static MethodSyntax Method(string pName, IList<ParameterSyntax> pParameters, IList<ReturnValueSyntax> pReturnValues, BlockSyntax pBody, bool pIsExtern)
        {
            return new MethodSyntax(pName, pParameters, pReturnValues, pBody, pIsExtern);
        }

        public static CastSyntax Cast(ParameterSyntax pParameter, ReturnValueSyntax pReturnValue, BlockSyntax pBody)
        {
            return new CastSyntax(pParameter, pReturnValue, pBody);
        }

        public static IfSyntax If(ExpressionSyntax pCondition, BlockSyntax pBlock, ElseSyntax pElse)
        {
            return new IfSyntax(pCondition, pBlock, pElse);
        }

        public static ElseSyntax Else(IfSyntax pIf, BlockSyntax pBody)
        {
            return new ElseSyntax(pIf, pBody);
        }

        public static WhileSyntax While(ExpressionSyntax pCondition, BlockSyntax pBody)
        {
            return new WhileSyntax(pCondition, pBody);
        }

        public static NumericLiteralSyntax NumericLiteral(string pValue, NumberType pType)
        {
            return new NumericLiteralSyntax(pValue, pType);
        }

        public static BooleanLiteralSyntax BooleanLiteral(string pValue)
        {
            return new BooleanLiteralSyntax(pValue);
        }

        public static StringLiteralSyntax StringLiteral(string pValue)
        {
            return new StringLiteralSyntax(pValue);
        }

        public static DateLiteralSyntax DateLiteral(string pValue)
        {
            return new DateLiteralSyntax(pValue);
        }

        public static ArrayLiteralSyntax ArrayLiteral(ExpressionSyntax pValue)
        {
            return new ArrayLiteralSyntax(pValue);
        }

        public static ArrayAccessExpressionSyntax ArrayAccessExpression(IdentifierSyntax pValue, ExpressionSyntax pIndex)
        {
            return new ArrayAccessExpressionSyntax(pValue, pIndex);
        }

        public static IdentifierSyntax Identifier(string pValue)
        {
            return new IdentifierSyntax(pValue);
        }

        public static ReturnSyntax Return(IList<ExpressionSyntax> pValues)
        {
            return new ReturnSyntax(pValues);
        }

        public static FunctionInvocationSyntax FunctionInvocation(string pValue, IList<ArgumentExpressionSyntax> pArguments)
        {
            return new FunctionInvocationSyntax(pValue, pArguments);
        }

        public static DeclarationStatementSyntax DeclarationStatement(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            return new DeclarationStatementSyntax(pLeft, pRight);
        }

        internal static GroupDeclarationStatementSyntax GroupDeclarationStatement(IdentifierGroupSyntax pLeft, ExpressionSyntax pValue)
        {
            return new GroupDeclarationStatementSyntax(pLeft, pValue);
        }

        public static AssignmentStatementSyntax AssignmentStatement(IdentifierSyntax pLeft, ExpressionSyntax pRight)
        {
            return new AssignmentStatementSyntax(pLeft, pRight);
        }

        internal static GroupAssignmentStatementSyntax GroupAssignmentStatement(IdentifierGroupSyntax pLeft, ExpressionSyntax pValue)
        {
            return new GroupAssignmentStatementSyntax(pLeft, pValue);
        }

        public static ValueDiscardSyntax ValueDiscard()
        {
            return new ValueDiscardSyntax();
        }

        public static IdentifierGroupSyntax IdentifierGroup(IList<IdentifierSyntax> pIdentifiers)
        {
            return new IdentifierGroupSyntax(pIdentifiers);
        }

        public static ObjectInitializerExpressionSyntax ObjectInitialzer(ExpressionSyntax pLeft)
        {
            return new ObjectInitializerExpressionSyntax(pLeft);
        }

        public static MemberAccessSyntax MemberAccess(IdentifierSyntax pName, IdentifierSyntax pValue)
        {
            return new MemberAccessSyntax(pName, pValue);
        }

        public static ImportSyntax Import(string pPath, string pAlias)
        {
            return new ImportSyntax(pPath, pAlias);
        }

        public static StructSyntax Struct(string pName, string pPrefix, IList<string> pTypeArgs, IList<FieldDeclarationSyntax> pFields, BlockSyntax pInitializer)
        {
            return new StructSyntax(pName, pPrefix, pTypeArgs, pFields, pInitializer);
        }

        public static NamespaceIdentifierSyntax NamespaceIdentifier(string pValue)
        {
            return new NamespaceIdentifierSyntax(pValue);
        }

        public static MemberIdentifierSyntax MemberIdentifier(LocalDefinition pDefinition, string pValue)
        {
            return new MemberIdentifierSyntax(pDefinition, pValue);
        }

        public static ForSyntax For(DeclarationStatementSyntax pDeclaration, ExpressionSyntax pCondition, SyntaxNode pPostLoop, BlockSyntax pBody)
        {
            return new ForSyntax(pDeclaration, pCondition, pPostLoop, pBody);
        }

        public static ConstSyntax Const(IdentifierSyntax pIdentifier, IdentifierSyntax pValue)
        {
            return new ConstSyntax(pIdentifier, pValue);
        }

        public static ArgumentExpressionSyntax ArgumentExpression(bool pIsRef, ExpressionSyntax pValue)
        {
            return new ArgumentExpressionSyntax(pIsRef, pValue);
        }

        public static ArgumentExpressionSyntax ArgumentExpression(ExpressionSyntax pValue)
        {
            return new ArgumentExpressionSyntax(false, pValue);
        }

        public static LengthQueryExpressionSyntax LengthQueryExpression(IdentifierSyntax pValue)
        {
            return new LengthQueryExpressionSyntax(pValue);
        }

        public static FieldDeclarationSyntax FieldDeclaration(string pNamespace, string pValue)
        {
            return new FieldDeclarationSyntax(pNamespace, pValue);
        }
    }
}
