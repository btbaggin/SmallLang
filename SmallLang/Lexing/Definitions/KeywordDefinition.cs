using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class KeywordDefinition : TokenDefinition
    {
        public bool AllowSubstring { get; set; }
        readonly string _word;
        readonly TokenType _symbol;
        public KeywordDefinition(string pWord, TokenType pSymbol)
        {
            _word = pWord;
            _symbol = pSymbol;
        }

        public override Token Match()
        {
            foreach (char c in _word)
            {
                if (char.ToUpperInvariant(Current) == char.ToUpperInvariant(c))
                {
                    Eat();
                }
                else
                {
                    return null;
                }
            }

            var found = AllowSubstring || IsWordBreak(Current);
            if (found) return CreateSymbol(_symbol);
            return null;
        }
    }
}
