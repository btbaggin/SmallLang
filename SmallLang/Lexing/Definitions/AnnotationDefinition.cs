using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class AnnotationDefinition : TokenDefinition
    {
        public override Token Match()
        {
            if (Current == '@')
            {
                Eat();
                StringBuilder value = new StringBuilder();
                while (!EOF && Current != ' ' && Current != '\r' && Current != '\n')
                {
                    value.Append(Current);
                    Eat();
                }

                return CreateSymbol(TokenType.Annotation, value.ToString());
            }

            return null;
        }
    }
}
