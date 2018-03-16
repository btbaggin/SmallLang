using System;
using System.Collections.Generic;
using System.Text;

using SmallLang.Lexing.Definitions;

namespace SmallLang.Lexing
{
    public class SmallLangDefinition : ILanguageDefinition
    {
        public void InitializeGrammar(Lexer pLexer)
        {
            //Constructs
            pLexer.AddDefinition(new KeywordDefinition(":=", TokenType.ColonEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("::", TokenType.ColonColon) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("->", TokenType.DashGreater) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("{", TokenType.LeftScope) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("}", TokenType.RightScope) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("(", TokenType.LeftParen) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition(")", TokenType.RightParen) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("[", TokenType.LeftBracket) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("]", TokenType.RightBracket) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition(",", TokenType.Comma) { AllowSubstring = true });

            //Operators
            pLexer.AddDefinition(new KeywordDefinition("**", TokenType.ImplicitCast) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("!=", TokenType.NotEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("<=", TokenType.LessThanEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition(">=", TokenType.GreaterThanEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("<", TokenType.LessThan) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition(">", TokenType.GreaterThan) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("=", TokenType.Equals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("+=", TokenType.PlusEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("-=", TokenType.MinusEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("*=", TokenType.StarEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("/=", TokenType.SlashEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("..=", TokenType.PeriodPeriodEquals) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("++", TokenType.PlusPlus));
            pLexer.AddDefinition(new KeywordDefinition("--", TokenType.MinusMinus));
            pLexer.AddDefinition(new KeywordDefinition("*", TokenType.Multiplication) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("/", TokenType.Division) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("+", TokenType.Addition) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("-", TokenType.Subtraction) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("%", TokenType.Mod) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("^", TokenType.Exponent) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("...", TokenType.Concatenate) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("&", TokenType.And));
            pLexer.AddDefinition(new KeywordDefinition("|", TokenType.Or));
            pLexer.AddDefinition(new KeywordDefinition("!", TokenType.Not) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("or", TokenType.BitwiseOr));
            pLexer.AddDefinition(new KeywordDefinition("and", TokenType.BitwiseAnd));
            pLexer.AddDefinition(new KeywordDefinition("lshift", TokenType.LShift));
            pLexer.AddDefinition(new KeywordDefinition("rshift", TokenType.RShift));

            pLexer.AddDefinition(new KeywordDefinition(":", TokenType.Colon) { AllowSubstring = true });

            //Keywords
            pLexer.AddDefinition(new KeywordDefinition("&", TokenType.Ref) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("Struct", TokenType.Struct));
            pLexer.AddDefinition(new KeywordDefinition("Cast", TokenType.Cast));
            pLexer.AddDefinition(new KeywordDefinition("Extern", TokenType.Extern));
            pLexer.AddDefinition(new KeywordDefinition("Import", TokenType.Import));
            pLexer.AddDefinition(new KeywordDefinition("As", TokenType.As));
            pLexer.AddDefinition(new KeywordDefinition("Return", TokenType.Return));
            pLexer.AddDefinition(new KeywordDefinition("If", TokenType.If));
            pLexer.AddDefinition(new KeywordDefinition("Else", TokenType.Else));
            pLexer.AddDefinition(new KeywordDefinition("While", TokenType.While));
            pLexer.AddDefinition(new KeywordDefinition("For", TokenType.For));
            pLexer.AddDefinition(new KeywordDefinition("New", TokenType.New));
            pLexer.AddDefinition(new KeywordDefinition(".", TokenType.Period) { AllowSubstring = true });
            pLexer.AddDefinition(new KeywordDefinition("_", TokenType.Underscore));

            pLexer.AddDefinition(new KeywordDefinition("len", TokenType.Length) { AllowSubstring = true });

            //Literals
            pLexer.AddDefinition(new FunctionDefinition());
            pLexer.AddDefinition(new KeywordDefinition("False", TokenType.False));
            pLexer.AddDefinition(new KeywordDefinition("True", TokenType.True));
            pLexer.AddDefinition(new NumberDefinition());
            pLexer.AddDefinition(new StringDefinition());
            pLexer.AddDefinition(new DateDefinition());

            pLexer.AddDefinition(new IdentifierDefinition());

            //Trivia
            pLexer.AddDefinition(new WhitespaceDefinition());
            pLexer.AddDefinition(new BlockCommentDefinition());
            pLexer.AddDefinition(new CommentDefinition());
            pLexer.AddDefinition(new NewlineDefinition());
            pLexer.AddDefinition(new AnnotationDefinition());
        }

        public void InitializeOperatorPrecedence(Lexer pLexer)
        {
            pLexer.AddPrecedence(TokenType.And, 7);
            pLexer.AddPrecedence(TokenType.Or, 7);

            pLexer.AddPrecedence(TokenType.LessThan, 6);
            pLexer.AddPrecedence(TokenType.LessThanEquals, 6);
            pLexer.AddPrecedence(TokenType.GreaterThan, 6);
            pLexer.AddPrecedence(TokenType.GreaterThanEquals, 6);
            pLexer.AddPrecedence(TokenType.Equals, 6);
            pLexer.AddPrecedence(TokenType.NotEquals, 6);

            pLexer.AddPrecedence(TokenType.Exponent, 5);
            pLexer.AddPrecedence(TokenType.Mod, 5);

            pLexer.AddPrecedence(TokenType.Multiplication, 4);
            pLexer.AddPrecedence(TokenType.Division, 4);
            pLexer.AddPrecedence(TokenType.StarEquals, 4);
            pLexer.AddPrecedence(TokenType.SlashEquals, 4);

            pLexer.AddPrecedence(TokenType.Addition, 3);
            pLexer.AddPrecedence(TokenType.Subtraction, 3);
            pLexer.AddPrecedence(TokenType.Concatenate, 3);
            pLexer.AddPrecedence(TokenType.PlusPlus, 3);
            pLexer.AddPrecedence(TokenType.MinusMinus, 3);
            pLexer.AddPrecedence(TokenType.PlusEquals, 3);
            pLexer.AddPrecedence(TokenType.MinusEquals, 3);
            pLexer.AddPrecedence(TokenType.PeriodPeriodEquals, 3);
            pLexer.AddPrecedence(TokenType.BitwiseAnd, 3);
            pLexer.AddPrecedence(TokenType.BitwiseOr, 3);
            pLexer.AddPrecedence(TokenType.LShift, 3);
            pLexer.AddPrecedence(TokenType.RShift, 3);

            pLexer.AddPrecedence(TokenType.Not, 2);
            pLexer.AddPrecedence(TokenType.Ref, 2);

            pLexer.AddPrecedence(TokenType.ImplicitCast, 1);
            pLexer.AddPrecedence(TokenType.Cast, 1);
        }
    }
}
