using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using SmallLang.Lexing;
using SmallLang.Syntax;

namespace SmallLang.Parsing
{
    public class Parser
    {
        private Token Current { get { return _stream.Current; } }

        readonly Lexer _lexer;
        ITokenStream _stream;
        string _source;
        readonly Stack<ExpressionSyntax> _operands;
        readonly Stack<ExpressionSyntax> _operators;
        readonly Stack<int> _positions;

        public static Parser Create(ILanguageDefinition pDefinition)
        {
            var lexer = new Lexer(pDefinition);
            return new Parser(lexer);
        }

        public Parser(Lexer pLexer)
        {
            _lexer = pLexer;
            _operands = new Stack<ExpressionSyntax>();
            _operators = new Stack<ExpressionSyntax>();
            _positions = new Stack<int>();
        }

        public WorkspaceSyntax Parse(string pSource)
        {
            _source = pSource;
            _stream = _lexer.StartTokenStream(pSource);

            return ParseWorkspace();
        }

        private WorkspaceSyntax ParseWorkspace()
        {
            List<MethodSyntax> functions = new List<MethodSyntax>();
            List<CastSyntax> casts = new List<CastSyntax>();
            List<ImportSyntax> imports = new List<ImportSyntax>();
            List<StructSyntax> structs = new List<StructSyntax>();
            List<ConstSyntax> consts = new List<ConstSyntax>();

            StartSpan();

            IgnoreNewlines();

            while (Peek(TokenType.Import))
            {
                imports.Add(ParseImport());
            }

            IgnoreNewlines();

            while(!Peek(TokenType.EndOfFile))
            {
                switch(Current.Type)
                {
                    case TokenType.Cast:
                        casts.Add(ParseCast());
                        break;

                    case TokenType.Struct:
                        structs.Add(ParseStruct());
                        break;

                    case TokenType.Colon:
                        consts.Add(ParseConst());
                        break;

                    default:
                        functions.Add(ParseMethod());
                        break;
                }
                IgnoreNewlines();
            }

            return SyntaxFactory.Workspace(imports, functions, casts, structs, consts).SetSpan<WorkspaceSyntax>(EndSpan());
        }

        private ConstSyntax ParseConst()
        {
            StartSpan();
            Expect(TokenType.Colon);
            var id = ParseIdentifierWithFunctionCall(true);
            Expect(TokenType.Equals);
            var v = ParseLiteral();

            return SyntaxFactory.Const(id, v);
        }

        private StructSyntax ParseStruct()
        {
            StartSpan();

            Expect(TokenType.Struct);
            Expect(TokenType.Identifier, out string name);

            List<string> typeArgs = new List<string>();
            if (PeekAndExpect(TokenType.LessThan))
            {
                Expect(TokenType.Identifier, out string typeArg);
                typeArgs.Add(typeArg);
                while (!PeekAndExpect(TokenType.GreaterThan))
                {
                    typeArgs.Add(typeArg);
                }
            }

            string prefix = null;
            if(!PeekAndExpect(TokenType.DashGreater) || !PeekAndExpect(TokenType.Identifier, out prefix))
                Compiler.ReportError(CompilerErrorType.StructNoType, PeekSpan(), name);
            Ignore(TokenType.Newline);

            Expect(TokenType.LeftScope);
            IgnoreNewlines();

            BlockSyntax initializer = null;
            List<FieldDeclarationSyntax> fields = new List<FieldDeclarationSyntax>();
            while (!PeekAndExpect(TokenType.RightScope))
            {
                StartSpan();

                if (PeekAndExpect(TokenType.New))
                {
                    IgnoreNewlines();
                    initializer = ParseBlock();
                }
                else
                {
                    ParseType(out string space, out string fieldName, out IList<string> typeHints);
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        Compiler.ReportError("Expecting type declaration");
                        Expect(Current.Type);
                    }

                    var f = SyntaxFactory.FieldDeclaration(space, fieldName);
                    f.SetTypeHints(typeHints);
                    fields.Add(f);
                }

                IgnoreNewlines();
            }
            var s = SyntaxFactory.Struct(name, prefix, typeArgs, fields, initializer).SetSpan<StructSyntax>(EndSpan());
            AddAnnotations(s);
            return s;
        }

        private ImportSyntax ParseImport()
        {
            StartSpan();

            Expect(TokenType.Import);
            Expect(TokenType.String, out string path);
            string alias = "";
            if(PeekAndExpect(TokenType.As))
            {
                Expect(TokenType.Identifier, out alias);
            }
            Expect(TokenType.Newline);

            return SyntaxFactory.Import(path, alias).SetSpan<ImportSyntax>(EndSpan());
        }

        private CastSyntax ParseCast()
        {
            StartSpan();

            Expect(TokenType.Cast);
            Expect(TokenType.ColonColon);

            Expect(TokenType.Identifier, out string name);
            ParameterSyntax p = new ParameterSyntax(false, "", name);
            Expect(TokenType.DashGreater);

            ParseType(out string space, out string retName, out IList<string> typehints); //TODO do something with?
            ReturnValueSyntax r = new ReturnValueSyntax(space, retName);
            Ignore(TokenType.Newline);

            var body = ParseBlock();
            return SyntaxFactory.Cast(p, r, body).SetSpan<CastSyntax>(EndSpan());
        }

        private MethodSyntax ParseMethod()
        {
            StartSpan();

            bool isExtern = PeekAndExpect(TokenType.Extern);
            Expect(TokenType.Identifier, out string name);

            List<string> typeHints = new List<string>();
            if (PeekAndExpect(TokenType.LessThan))
            {
                do
                {
                    Expect(TokenType.Identifier, out string typeHint);
                    typeHints.Add(typeHint);
                } while (PeekAndExpect(TokenType.Comma));
                Expect(TokenType.GreaterThan);
            }

            Expect(TokenType.ColonColon);
            Expect(TokenType.LeftParen);

            List<ParameterSyntax> parameters = new List<ParameterSyntax>();
            List<ReturnValueSyntax> returnValues = new List<ReturnValueSyntax>();

            if (!Peek(TokenType.RightParen))
            {
                do
                {
                    bool isRef = PeekAndExpect(TokenType.Ref);
                    ParseType(out string space, out string parameterName, out IList<string> parameterTypeHints);

                    var p = SyntaxFactory.Parameter(isRef, space, parameterName);
                    AddAnnotations(p);
                    p.SetTypeHints(parameterTypeHints);
                    parameters.Add(p);
                } while (PeekAndExpect(TokenType.Comma));
            }

            Expect(TokenType.RightParen);

            if (PeekAndExpect(TokenType.DashGreater))
            {
                do
                {
                    ParseType(out string space, out string t, out IList<string> retTypeHints);//TODO
                    var r = SyntaxFactory.ReturnValue(space, t);
                    returnValues.Add(r);
                } while (PeekAndExpect(TokenType.Comma));
            }
            Ignore(TokenType.Newline);

            var body = ParseBlock();

            var method = SyntaxFactory.Method(name, parameters, returnValues, body, isExtern);
            AddAnnotations(method);
            method.SetTypeHints(typeHints);
            return method.SetSpan<MethodSyntax>(EndSpan());
        }

        private void AddAnnotations(SyntaxNode pNode)
        {
            while(PeekAndExpect(TokenType.Annotation, out string a))
            {
                pNode.AddAnnotation(new Annotation(a));
            }
        }

        private BlockSyntax ParseBlock()
        {
            StartSpan();

            List<SyntaxNode> statements = new List<SyntaxNode>();

            Expect(TokenType.LeftScope);
            IgnoreNewlines();

            while(!Peek(TokenType.RightScope))
            {
                var s = ParseStatement();
                if(s != null) statements.Add(s);
                IgnoreNewlines();
            }
            Expect(TokenType.RightScope);

            return SyntaxFactory.Block(statements).SetSpan<BlockSyntax>(EndSpan());
        }

        private SyntaxNode ParseStatement()
        {
            SyntaxNode s = null;
            switch(Current.Type)
            {
                case TokenType.If:
                    s = ParseIf();
                    break;

                case TokenType.While:
                    s = ParseWhile();
                    break;

                case TokenType.For:
                    s = ParseFor();
                    break;

                case TokenType.FunctionInvocation:
                    s = ParseFunctionInvocation();
                    break;

                case TokenType.Return:
                    s = ParseReturn();
                    break;

                case TokenType.LeftScope:
                    s = ParseBlock();
                    break;

                case TokenType.Colon:
                    s = ParseDeclaration(true);
                    break;

                default:
                    s = ParseAssignment();
                    if (!Peek(TokenType.Newline) && !Peek(TokenType.RightScope))
                        Compiler.ReportError(CompilerErrorType.ExpectingToken, PeekSpan(), TokenType.Newline.ToString());
                    break;
            }

            return s;
        }

        private DeclarationStatementSyntax ParseDeclaration(bool pAllowMultiple)
        {
            StartSpan();
            Expect(TokenType.Colon);
            List<IdentifierSyntax> id = new List<IdentifierSyntax>();
            do
            {
                id.Add(ParseIdentifierWithFunctionCall(true));
            }
            while (PeekAndExpect(TokenType.Comma) && pAllowMultiple);

            Expect(TokenType.Equals);

            ExpressionSyntax left = null;
            ExpressionSyntax right = null;
            if (id.Count == 1) left = id[0];
            else left = SyntaxFactory.IdentifierGroup(id);

            right = ParseNew(left);
            if(right == null) right = GetExpression();
            if (right == null)
                Compiler.ReportError(CompilerErrorType.ExpressionExpected, PeekSpan());

            if (id.Count == 1) return SyntaxFactory.DeclarationStatement(id[0], right).SetSpan<DeclarationStatementSyntax>(EndSpan());
            else return SyntaxFactory.GroupDeclarationStatement((IdentifierGroupSyntax)left, right).SetSpan<GroupDeclarationStatementSyntax>(EndSpan());
        }

        private ObjectInitializerExpressionSyntax ParseNew(ExpressionSyntax pValue)
        {
            if (!PeekAndExpect(TokenType.New)) return null;
            return SyntaxFactory.ObjectInitialzer(pValue);
        }

        private SyntaxNode ParseAssignment()
        {
            StartSpan();

            List<IdentifierSyntax> id = new List<IdentifierSyntax>();
            do
            {
                id.Add(ParseIdentifierWithFunctionCall(false));
            }
            while (PeekAndExpect(TokenType.Comma));
            ExpressionSyntax right = null;
            SyntaxNode node = null;

            switch(Current.Type)
            {
                case TokenType.Equals:
                    Expect(TokenType.Equals);
                    right = GetExpression();

                    if (id.Count == 1) node = SyntaxFactory.AssignmentStatement(id[0], right);
                    else node = SyntaxFactory.GroupAssignmentStatement(SyntaxFactory.IdentifierGroup(id), right);
                    break;

                case TokenType.PlusEquals:
                case TokenType.MinusEquals:
                case TokenType.StarEquals:
                case TokenType.SlashEquals:
                case TokenType.PeriodPeriodEquals:
                    if (id.Count > 1) throw new Exception();
                    var type = Current.Type;
                    Expect(type);
                    var exp = SyntaxFactory.AssignmentExpression(type, id[0], GetExpression());
                    node = exp;
                    break;

                case TokenType.PlusPlus:
                case TokenType.MinusMinus:
                    if (id.Count > 1) throw new Exception();
                    _operands.Push(id[0]);
                    PushUnaryOperator();
                    PopOperator();
                    node = _operands.Pop();
                    break;

                case TokenType.Newline:
                case TokenType.RightScope:
                    node = id[0];
                    break;

                default:
                    Compiler.ReportError(CompilerErrorType.UnexpectedToken, PeekSpan(), Current.Type.ToString());
                    Expect(Current.Type);
                    return null;
            }

            return node.SetSpan<SyntaxNode>(EndSpan());
        }

        private IdentifierSyntax ParseIdentifier()
        {
            StartSpan();

            if (Peek(TokenType.Identifier))
            {
                Expect(TokenType.Identifier, out string i);
                var id = SyntaxFactory.Identifier(i);
                if (PeekAndExpect(TokenType.LeftBracket))
                {
                    _operators.Push(ExpressionSyntax.Sentinel);
                    ExpressionSyntax e = GetExpression();

                    id = SyntaxFactory.ArrayAccessExpression(id, e);
                    Expect(TokenType.RightBracket);
                    _operators.Pop();
                }

                return id.SetSpan<IdentifierSyntax>(EndSpan());
            }
            else
            {
                DestroySpan();
                return null;
            }
        }

        private IdentifierSyntax ParseIdentifierComplete()
        {
            StartSpan();

            if (Peek(TokenType.Identifier))
            {
                Expect(TokenType.Identifier, out string i);
                var id = SyntaxFactory.Identifier(i);
                if (PeekAndExpect(TokenType.LeftBracket))
                {
                    _operators.Push(ExpressionSyntax.Sentinel);
                    ExpressionSyntax e = GetExpression();

                    id = SyntaxFactory.ArrayAccessExpression(id, e);
                    Expect(TokenType.RightBracket);
                    _operators.Pop();
                }
                else if (PeekAndExpect(TokenType.LessThan))
                {
                    do
                    {
                        Expect(TokenType.Identifier, out string typeHint);
                        id.SetTypeHints(new List<string>() { typeHint });
                    } while (PeekAndExpect(TokenType.Comma));

                    Expect(TokenType.GreaterThan);
                }
                return id.SetSpan<IdentifierSyntax>(EndSpan());
            }
            else if (PeekAndExpect(TokenType.Underscore)) return SyntaxFactory.ValueDiscard().SetSpan<ValueDiscardSyntax>(EndSpan());
            else
            {
                DestroySpan();
                return null;
            }
        }

        private IdentifierSyntax ParseIdentifierWithFunctionCall(bool pIdentifierOnly)
        {
            StartSpan();

            var id = ParseIdentifierComplete();
            
            if (id == null && !pIdentifierOnly) id = ParseFunctionInvocation();
            if (id == null)
            {
                DestroySpan();
                return null;
            }

            if (PeekAndExpect(TokenType.Period))
            {
                return SyntaxFactory.MemberAccess(id, ParseIdentifierWithFunctionCall(pIdentifierOnly)).SetSpan<MemberAccessSyntax>(EndSpan());
            }
            return id.SetSpan<IdentifierSyntax>(EndSpan());
        }

        private IfSyntax ParseIf()
        {
            StartSpan();

            Expect(TokenType.If);
            Expect(TokenType.LeftParen);
            var condition = GetExpression();
            Expect(TokenType.RightParen);

            Ignore(TokenType.Newline);
            BlockSyntax body = null;
            if (Peek(TokenType.LeftScope)) body = ParseBlock();
            else body = SyntaxFactory.Block(new List<SyntaxNode> { ParseStatement() });
            Ignore(TokenType.Newline);

            ElseSyntax e = ParseElse();

            return SyntaxFactory.If(condition, body, e).SetSpan<IfSyntax>(EndSpan());
        }

        private ElseSyntax ParseElse()
        {
            if (!PeekAndExpect(TokenType.Else)) return null;
            StartSpan();

            IfSyntax i = null;
            BlockSyntax body = null;
            if (Peek(TokenType.If)) i = ParseIf();
            else
            {
                Ignore(TokenType.Newline);
                if (Peek(TokenType.LeftScope)) body = ParseBlock();
                else body = SyntaxFactory.Block(new List<SyntaxNode> { ParseStatement() });
            }

            return SyntaxFactory.Else(i, body).SetSpan<ElseSyntax>(EndSpan());
        }

        private WhileSyntax ParseWhile()
        {
            StartSpan();

            Expect(TokenType.While);
            Expect(TokenType.LeftParen);
            var condition = GetExpression();
            Expect(TokenType.RightParen);

            PeekAndExpect(TokenType.Newline);
            BlockSyntax body = null;
            if (Peek(TokenType.LeftScope)) body = ParseBlock();
            else body = SyntaxFactory.Block(new List<SyntaxNode> { ParseStatement() });

            return SyntaxFactory.While(condition, body).SetSpan<WhileSyntax>(EndSpan());
        }

        private ForSyntax ParseFor()
        {
            StartSpan();

            Expect(TokenType.For);
            Expect(TokenType.LeftParen);
            var decl = ParseDeclaration(false);

            Expect(TokenType.Comma);
            var condition = GetExpression();

            Expect(TokenType.Comma);
            var postLoop = GetExpression();
            Expect(TokenType.RightParen);

            PeekAndExpect(TokenType.Newline);
            BlockSyntax body = null;
            if (Peek(TokenType.LeftScope)) body = ParseBlock();
            else body = SyntaxFactory.Block(new List<SyntaxNode> { ParseStatement() });

            return SyntaxFactory.For(decl, condition, postLoop, body).SetSpan<ForSyntax>(EndSpan());
        }

        private ReturnSyntax ParseReturn()
        {
            StartSpan();

            Expect(TokenType.Return);
            List<ExpressionSyntax> values = new List<ExpressionSyntax>();
            if(!Peek(TokenType.Newline))
            {
                do
                {
                    Ignore(TokenType.Newline);
                    values.Add(GetExpression());
                } while (PeekAndExpect(TokenType.Comma));
            }

            return SyntaxFactory.Return(values).SetSpan<ReturnSyntax>(EndSpan());
        }

        private ExpressionSyntax GetExpression()
        {
            StartSpan();

            ParseAndOr();
            while (_operators.Count > 0 && _operators.Peek() != ExpressionSyntax.Sentinel)
            {
                PopOperator();
            }

            if (_operands.Count == 0)
            {
                DestroySpan();
                return null;
            }
            return _operands.Pop().SetSpan<ExpressionSyntax>(EndSpan());
        }

        private void ParseAndOr()
        {
            ParseParen();

            while(Peek(TokenType.And) || Peek(TokenType.Or))
            {
                PushBinaryOperator();
                IgnoreNewlines();
                ParseAndOr();
            }
        }

        private void ParseParen()
        {
            if(PeekAndExpect(TokenType.LeftParen))
            {
                _operators.Push(ExpressionSyntax.Sentinel);
                IgnoreNewlines();
                ParseAndOr();
                Expect(TokenType.RightParen);
                _operators.Pop();
            }

            ParseCompare();

            while(_operators.Count > 0 && _operators.Peek() != ExpressionSyntax.Sentinel)
            {
                PopOperator();
            }
        }

        private void ParseCompare()
        {
            ParseAddition();

            while (Peek(TokenType.Equals) ||
                  Peek(TokenType.NotEquals) ||
                  Peek(TokenType.LessThan) ||
                  Peek(TokenType.LessThanEquals) ||
                  Peek(TokenType.GreaterThan) ||
                  Peek(TokenType.GreaterThanEquals))
            {
                PushBinaryOperator();
                IgnoreNewlines();
                ParseAndOr();
            }
        }

        private void ParseAddition()
        {
            ParseTerm();

            while (Peek(TokenType.Addition) ||
                  Peek(TokenType.Subtraction) ||
                  Peek(TokenType.Concatenate))
            {
                PushBinaryOperator();
                IgnoreNewlines();
                ParseAndOr();
            }
        }

        private void ParseTerm()
        {
            ParseUnary();

            while (Peek(TokenType.Multiplication) ||
                  Peek(TokenType.Division) ||
                  Peek(TokenType.Exponent) ||
                  Peek(TokenType.Mod) ||
                  Peek(TokenType.BitwiseAnd) ||
                  Peek(TokenType.BitwiseOr) ||
                  Peek(TokenType.LShift) ||
                  Peek(TokenType.RShift))
            {
                PushBinaryOperator();
                IgnoreNewlines();
                ParseAndOr();
            }
        }

        private void ParseUnary()
        {
            ParseCastCall();

            if(Peek(TokenType.PlusPlus) || Peek(TokenType.MinusMinus))
            {
                PushUnaryOperator();
                IgnoreNewlines();
                ParseAndOr();
            }
        }

        private void ParseCastCall()
        {
            switch (Current.Type)
            {
                case TokenType.Not:
                case TokenType.Subtraction:
                case TokenType.ImplicitCast:
                    PushUnaryOperator();
                    IgnoreNewlines();
                    ParseAndOr();
                    break;

                case TokenType.Cast:
                    var u = (ExplicitCastExpressionSyntax)PushUnaryOperator();
                    if (!Expect(TokenType.LeftParen))
                        Compiler.ReportError(CompilerErrorType.NoSpecifiedType, PeekSpan());
                    else
                    {
                        ParseType(out string space, out string name, out IList<string> typeHints); //TODO do something with?
                        u.TypeNamspace = space;
                        u.TypeName = name;
                        Expect(TokenType.RightParen);
                    }
                    IgnoreNewlines();
                    ParseAndOr();
                    break;
            }

            var o = ParseOperand();
            if (o != null) _operands.Push(o);
        }

        private IdentifierSyntax ParseOperand()
        {
            StartSpan();
            var e = ParseLiteral();
            if (e == null) e = ParseIdentifier();
            if (e == null) e = ParseFunctionInvocation();
            if (e == null) e = ParseQuery();
            if (e != null)
            {
                while (PeekAndExpect(TokenType.Period))
                {
                    e = SyntaxFactory.MemberAccess(e, ParseOperand());
                }
            }
            if (e == null)
            {
                DestroySpan();
                return null;
            }

            return e.SetSpan<IdentifierSyntax>(EndSpan());
        }

        private IdentifierSyntax ParseQuery()
        {
            switch(Current.Type)
            {
                case TokenType.Length:
                    Expect(TokenType.Length);
                    Expect(TokenType.LeftParen);

                    IdentifierSyntax i = ParseIdentifierWithFunctionCall(true);

                    Expect(TokenType.RightParen);
                    return SyntaxFactory.LengthQueryExpression(i);

                default:
                    return null;
            }
        }

        private IdentifierSyntax ParseFunctionInvocation()
        {
            if (!PeekAndExpect(TokenType.FunctionInvocation, out string value)) return null;

            StartSpan();
            Expect(TokenType.LeftParen);

            _operators.Push(ExpressionSyntax.Sentinel);
            List<ArgumentExpressionSyntax> arguments = new List<ArgumentExpressionSyntax>();
            if(!Peek(TokenType.RightParen))
            {
                StartSpan();
                ArgumentExpressionSyntax ae = SyntaxFactory.ArgumentExpression(GetExpression());
                arguments.Add(ae.SetSpan<ArgumentExpressionSyntax>(EndSpan()));

                while (!Peek(TokenType.RightParen))
                {
                    if (!PeekAndExpect(TokenType.Comma))
                    {
                        Compiler.ReportError(CompilerErrorType.EndOfStatementExpected, PeekSpan());
                        while(!Peek(TokenType.Comma) &&
                              !Peek(TokenType.LeftParen) &&
                              !Peek(TokenType.Identifier) &&
                              !Peek(TokenType.FunctionInvocation) &&
                              !Peek(TokenType.Newline))
                        {
                            Ignore(Current.Type);
                        }
                        _stream.InsertToken(new Token(TokenType.Comma, 1));
                    }
                    else
                    {
                        StartSpan();
                        ae = SyntaxFactory.ArgumentExpression(GetExpression());
                        arguments.Add(ae.SetSpan<ArgumentExpressionSyntax>(EndSpan()));
                    }
                }
            }
            _operators.Pop();

            Expect(TokenType.RightParen);

            List<string> types = new List<string>();
            if(PeekAndExpect(TokenType.Colon))
            {
                ParseType(out string space, out string name, out IList<string> typeHints);
                types.Add(name);
            }

            var f = SyntaxFactory.FunctionInvocation(value, arguments).SetSpan<IdentifierSyntax>(EndSpan());
            f.SetTypeHints(types);
            return f;
        }

        private IdentifierSyntax ParseLiteral()
        {
            StartSpan();
            IdentifierSyntax e = null;
            if (PeekAndExpect(TokenType.I32, out string v)) e = SyntaxFactory.NumericLiteral(v, NumberType.I32);
            else if (PeekAndExpect(TokenType.I16, out v)) e = SyntaxFactory.NumericLiteral(v, NumberType.I16);
            else if (PeekAndExpect(TokenType.I64, out v)) e = SyntaxFactory.NumericLiteral(v, NumberType.I64);
            else if (PeekAndExpect(TokenType.Double, out v)) e = SyntaxFactory.NumericLiteral(v, NumberType.Double);
            else if (PeekAndExpect(TokenType.Float, out v)) e = SyntaxFactory.NumericLiteral(v, NumberType.Float);
            else if (PeekAndExpect(TokenType.String, out v)) e = SyntaxFactory.StringLiteral(v);
            else if (PeekAndExpect(TokenType.Date, out v)) e = SyntaxFactory.DateLiteral(v);
            else if (PeekAndExpect(TokenType.True, out v)) e = SyntaxFactory.BooleanLiteral(v);
            else if (PeekAndExpect(TokenType.False, out v)) e = SyntaxFactory.BooleanLiteral(v);
            else if (PeekAndExpect(TokenType.LeftBracket))
            {
                ExpressionSyntax size = GetExpression();
                if (size == null)
                {
                    Compiler.ReportError(CompilerErrorType.ExpressionExpected, PeekSpan());
                    size = SyntaxFactory.NumericLiteral("0", NumberType.I32);
                }

                if (!PeekAndExpect(TokenType.RightBracket)) Compiler.ReportError(CompilerErrorType.ArrayLiteralBracket, PeekSpan());
                e = SyntaxFactory.ArrayLiteral(size);
            }
            else
            {
                DestroySpan();
                return null;
            }

            return e.SetSpan<IdentifierSyntax>(EndSpan());
        }

        private void ParseType(out string pNamespace, out string pName, out IList<string> pTypeHints)
        {
            List<string> typeHints = new List<string>();
            if (!PeekAndExpect(TokenType.Identifier, out string first))
            {
                pNamespace = "";
                pName = "";
                pTypeHints = typeHints;
                return;
            }

            if(PeekAndExpect(TokenType.Period))
            {
                Expect(TokenType.Identifier, out string second);
                pNamespace = first;
                pName = second;
            }
            else
            {
                pNamespace = "";
                pName = first;
            }
            
            if(PeekAndExpect(TokenType.LessThan))
            {
                do
                {
                    Expect(TokenType.Identifier, out string typeHint);
                    typeHints.Add(typeHint);
                } while (PeekAndExpect(TokenType.Comma));

                Expect(TokenType.GreaterThan);
            }

            pTypeHints = typeHints;
        }

        #region Helper functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Expect(TokenType pSymbol)
        {
            if (!_stream.EOF && Current.Type == pSymbol)
            {
                _stream.MoveNext();
                return true;
            }
            else
            {
                Compiler.ReportError(CompilerErrorType.InvalidSyntax, PeekSpan(), pSymbol.ToString(), Current.Type.ToString());
                return false;
            }
        }
        private bool Expect(TokenType pSymbol, out string s)
        {
            if (!_stream.EOF && Current.Type == pSymbol)
            {
                string value = Current.Value ?? _source.Substring(_stream.SourceIndex - Current.Length, Current.Length);
                _stream.MoveNext();
                s = value;
                return true;
            }
            else
            {
                s = null;
                Compiler.ReportError(CompilerErrorType.InvalidSyntax, PeekSpan(), pSymbol.ToString(), Current.Type.ToString());
                return false;
            }
        }

        private bool Ignore(TokenType pSymbol)
        {
            if (!_stream.EOF && Current.Type == pSymbol)
            {
                _stream.MoveNext();
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Peek(TokenType pSymbol)
        {
            return Current.Type == pSymbol;
        }

        private bool PeekAndExpect(TokenType pSymbol, out string pValue)
        {
            if (Peek(pSymbol))
            {
                Expect(pSymbol, out pValue);
                return true;
            }
            pValue = "";
            return false;
        }

        private bool PeekAndExpect(TokenType pSymbol)
        {
            if (Peek(pSymbol))
            {
                Expect(pSymbol);
                return true;
            }
            return false;
        }

        private TernaryExpressionSyntax PushTernaryOperator()
        {
            StartSpan();
            var precedence = _lexer.GetPrecedence(Current.Type);

            while (_operands.Count > 0 && _operands.Peek()?.Precedence < precedence)
            {
                PopOperator();
            }
            var exp = SyntaxFactory.TernaryExpression(precedence, Current.Type);
            Expect(Current.Type);
            _operators.Push(exp);
            return exp.SetSpan<TernaryExpressionSyntax>(EndSpan());
        }

        private BinaryExpressionSyntax PushBinaryOperator()
        {
            StartSpan();
            var precedence = _lexer.GetPrecedence(Current.Type);

            while (_operators.Count > 0 && _operators.Peek()?.Precedence < precedence)
            {
                PopOperator();
            }
            var exp = SyntaxFactory.BinaryExpression(precedence, Current.Type);
            Expect(Current.Type);
            _operators.Push(exp);
            return exp.SetSpan<BinaryExpressionSyntax>(EndSpan());
        }

        private UnaryExpressionSyntax PushUnaryOperator()
        {
            StartSpan();
            var precedence = _lexer.GetPrecedence(Current.Type);

            while (_operators.Count > 0 && _operators.Peek()?.Precedence < precedence)
            {
                PopOperator();
            }
            var exp = SyntaxFactory.UnaryExpression(precedence, Current.Type);
            Expect(Current.Type);
            _operators.Push(exp);
            return exp.SetSpan<UnaryExpressionSyntax>(EndSpan());
        }

        private void PopOperator()
        {
            ExpressionSyntax op = null;
            ExpressionSyntax o1 = null;
            ExpressionSyntax o2 = null;
            ExpressionSyntax o3 = null;
            op = _operators.Pop();

            switch (op)
            {
                case TernaryExpressionSyntax t:
                    if (_operands.Count < 3)
                    {
                        Compiler.ReportError(CompilerErrorType.ExpressionExpected, PeekSpan());
                        if (_operands.Count > 0) o1 = _operands.Pop(); 
                        if(_operands.Count > 1) o2 = _operands.Pop();
                        o3 = SyntaxFactory.Identifier("");
                    }
                    else
                    {
                        o1 = _operands.Pop();
                        o2 = _operands.Pop();
                        o3 = _operands.Pop();
                    }

                    t.SetCenter(o3);
                    t.SetLeft(o2);
                    t.SetRight(o1);
                    break;

                case BinaryExpressionSyntax b:
                    if (_operands.Count < 2)
                    {
                        Compiler.ReportError(CompilerErrorType.ExpressionExpected, PeekSpan());
                        if (_operands.Count == 1) o1 = _operands.Pop();
                        o2 = SyntaxFactory.Identifier("");
                    }
                    else
                    {
                        o1 = _operands.Pop();
                        o2 = _operands.Pop();
                    }

                    b.SetLeft(o2);
                    b.SetRight(o1);
                    break;

                case UnaryExpressionSyntax u:
                    if (_operands.Count < 1)
                    {
                        Compiler.ReportError(CompilerErrorType.ExpressionExpected, PeekSpan());
                        o1 = SyntaxFactory.Identifier("");
                    }
                    else o1 = _operands.Pop();
                    u.SetValue(o1);
                    break;

                default:
                    throw new Exception("Unknown expression type");
            }

            _operands.Push(op);
        }

        private void IgnoreNewlines()
        {
            while (Ignore(TokenType.Newline)) ;
        }

        private void StartSpan()
        {
            _positions.Push(_stream.SourceIndex - Current.Length);
        }

        private void DestroySpan()
        {
            _positions.Pop();
        }

        private TextSpan EndSpan()
        {
            return new TextSpan(_positions.Pop(), _stream.SourceIndex - Current.Length, _stream.SourceLine, _stream.SourceColumn); 
        }

        private TextSpan PeekSpan()
        {
            return new TextSpan(_positions.Peek(), _stream.SourceIndex - Current.Length, _stream.SourceLine, _stream.SourceColumn);
        }
        #endregion  
    }
}
