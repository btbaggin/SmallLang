using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class WhitespaceDefinition : TokenDefinition
    {
        public WhitespaceDefinition()
        {
            IncludeSymbol = false;
        }
        public override Token Match()
        {
            bool whiteSpace = false;
            while (!EOF && (Current == ' ' || Current == '\t'))
            {
                whiteSpace = true;
                Eat();
            }

            if (whiteSpace) return CreateSymbol(TokenType.Whitespace);
            return null;
        }
    }
}
