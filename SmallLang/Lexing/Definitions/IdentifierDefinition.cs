using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class IdentifierDefinition : TokenDefinition
    {
        public override Token Match()
        {
            var id = false;
            while (!EOF && (char.IsLetterOrDigit(Current) || Current == '_'))
            {
                id = true;
                Eat();
            }

            if (id) return CreateSymbol(TokenType.Identifier);
            return null;
        }
    }
}
