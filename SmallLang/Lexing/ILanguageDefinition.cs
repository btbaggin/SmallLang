using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing
{
    public interface ILanguageDefinition
    {
        void InitializeGrammar(Lexer pLexer);
        void InitializeOperatorPrecedence(Lexer pLexer);
    }
}
