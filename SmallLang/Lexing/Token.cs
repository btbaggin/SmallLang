using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang
{
    public class Token
    {
        private static Token end = new Token(TokenType.EndOfFile, 3);
        public static Token End { get { return end; } }

        public TokenType Type { get; private set; }

        public int Length { get; private set; }

        public string Value { get; private set; }

        public Token(TokenType pType, int pLength)
        {
            Type = pType;
            Length = pLength;
        }

        public Token(TokenType pType, int pLength, string pValue)
        {
            Type = pType;
            Length = pLength;
            Value = pValue;
        }
    }
}
