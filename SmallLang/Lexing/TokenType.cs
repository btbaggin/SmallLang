using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang
{
    public enum TokenType
    {
        Return,
        If,
        Else,
        While,
        For,
        Extern,
        Import,
        FunctionInvocation,
        Identifier,
        Newline,
        ColonEquals,
        ColonColon,
        LeftScope,
        RightScope,
        LeftParen,
        RightParen,
        LeftBracket,
        RightBracket,
        Comma,
        DashGreater,
        Underscore,
        Cast,
        Period,
        As,
        Colon,
        Question,
        Struct,
        Ref,
        New,

        Length,

        //Operators
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Equals,
        NotEquals,
        LessThanEquals,
        GreaterThanEquals,
        LessThan,
        GreaterThan,
        And,
        Or,
        BitwiseOr,
        BitwiseAnd,
        LShift,
        RShift,

        Exponent,
        Not,
        Mod,
        Concatenate,
        PlusPlus,
        MinusMinus,
        PlusEquals,
        MinusEquals,
        SlashEquals,
        StarEquals,
        PeriodPeriodEquals,
        ImplicitCast,
        ExplicitCast,
        Ternary,

        //Literals
        String,
        Date,
        I64,
        I32,
        I16,
        Float,
        Double,
        True,
        False,

        //Trivia
        Whitespace,
        Comment,
        Annotation,

        //Special
        EndOfFile
    }
}
