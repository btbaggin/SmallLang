using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SmallLang
{
    public abstract class TokenDefinition
    {
        static char[] Breaks = { ' ', ',', '(', '[', '{', ')', ']', '}', '+', '-', '*', '/', '^', '&', '<', '!', '>', '=', '\r', '\n', '\t', '.', '#'};
        protected Tokenizer mTokenizer;
        int mlngLength;

        protected char Current
        {
            get { return mTokenizer.Current; }
        }

        protected bool EOF
        {
            get { return mTokenizer.EOF; }
        }

        public bool IncludeSymbol { get; set; }

        public bool CaseSensative { get; set; }

        protected TokenDefinition()
        {
            IncludeSymbol = true;
        }

        public Token IsMatch(Tokenizer pTokenizer)
        {
            mTokenizer = pTokenizer;
            if(mTokenizer.EOF) return Token.End;

            mTokenizer.BeginTransaction();
            mlngLength = 0;

            Token sym = Match();

            if (sym == null) mTokenizer.RollbackTransaction();
            else mTokenizer.CommitTransaction();

            return sym;
        }

        protected char Peek(int pPeek)
        {
            return mTokenizer.Peek(pPeek);
        }

        protected void Eat()
        {
            mTokenizer.Eat();
            mlngLength++;
        }

        protected static bool IsWordBreak(char pChar)
        {
            return Breaks.Contains(pChar);
        }

        protected Token CreateSymbol(TokenType penmType)
        {
            return new Token(penmType, mlngLength);
        }

        protected Token CreateSymbol(TokenType penmType, string pstrValue)
        {
            return new Token(penmType, mlngLength, pstrValue);
        }

        public abstract Token Match();
    }
}
