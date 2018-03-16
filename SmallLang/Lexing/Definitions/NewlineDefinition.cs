using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class NewlineDefinition : TokenDefinition
    {
        public override Token Match()
        {
            if (Current == '\r')
            {
                Eat();
            }
            if (Current == '\n')
            {
                Eat();
                return CreateSymbol(TokenType.Newline);
            }

            return null;
        }
    }
}
