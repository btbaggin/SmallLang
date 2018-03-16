using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class FunctionDefinition : TokenDefinition
    {
        public override Token Match()
        {
            var id = false;
            while (!EOF && (char.IsLetterOrDigit(Current) || Current == '_'))
            {
                id = true;
                Eat();
            }

            if (id && Current == '(') return CreateSymbol(TokenType.FunctionInvocation);
            return null;
        }
    }
}
