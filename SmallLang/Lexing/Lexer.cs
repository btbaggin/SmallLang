using System;
using System.Collections.Generic;

namespace SmallLang.Lexing
{
    public class Lexer : ITokenSource
    {

        readonly List<TokenDefinition> mlstDefinitions;
        readonly Dictionary<TokenType, int> mdctPrecedence;
        Tokenizer mTokenizer;
        bool mblnAtEnd;

        public int Index
        {
            get { return mTokenizer.Index; }
        }

        public int Line
        {
            get { return mTokenizer.Line; }
        }

        public int Column
        {
            get { return mTokenizer.Column; }
        }

        public Lexer(ILanguageDefinition pLanguageDefinition)
        {
            mlstDefinitions = new List<TokenDefinition>();
            mdctPrecedence = new Dictionary<TokenType, int>();

            pLanguageDefinition.InitializeGrammar(this);
            pLanguageDefinition.InitializeOperatorPrecedence(this);
        }

        public void AddDefinition(TokenDefinition pDefinition)
        {
            mlstDefinitions.Add(pDefinition);
        }

        public void AddPrecedence(TokenType pType, int pPrecedence)
        {
            mdctPrecedence.Add(pType, pPrecedence);
        }

        public ITokenStream StartTokenStream(string pstrSource)
        {
            mTokenizer = new Tokenizer(pstrSource);
            mblnAtEnd = false;
            return new BufferedTokenStream(this);
        }

        public Token PeekSymbol(int pCount)
        {
            Token s = null;
            bool blnAtEnd = mblnAtEnd;
            int i = 0;

            mTokenizer.BeginTransaction();
            while (i < pCount && GetNextToken(out s))
            {
                i++;
            }
        
            mTokenizer.RollbackTransaction();

            mblnAtEnd = blnAtEnd;
            return s;
        }

        public bool GetNextToken(out Token pToken)
        {
            Token current = NextSymbol(mTokenizer);
            if(current != null && current.Type != TokenType.EndOfFile)
            {
                pToken = current;
                return true;
            }

            if(!mblnAtEnd)
            {
                pToken = Token.End;
                mblnAtEnd = true;
                return true;
            }
            
            pToken = null;
            return false;
        }

        public void GetSourceInfo(out int pLine, out int pColumn)
        {
            pLine = mTokenizer.Line;
            pColumn = mTokenizer.Column;
        }

        private Token NextSymbol(Tokenizer pTokenizer)
        {
            if (pTokenizer.EOF) return Token.End;

            foreach(var d in mlstDefinitions)
            {
                Token sym = d.IsMatch(pTokenizer);
                if(sym != null)
                {
                    if (d.IncludeSymbol) return sym;
                    //We aren't including this symbol, return the next one
                    return NextSymbol(pTokenizer);
                }
            }

            return null;
        }

        public int GetPrecedence(TokenType pType)
        {
            if(mdctPrecedence.ContainsKey(pType))
            {
                return mdctPrecedence[pType];
            }

            throw new Exception("Unknown precedence for operator " + pType.ToString());
        }
    }
}
