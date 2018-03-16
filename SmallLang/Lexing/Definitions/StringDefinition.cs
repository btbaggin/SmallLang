using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class StringDefinition : TokenDefinition
    {
        const char QUOTE = '"';
        const char ESCAPE = '\\';

        public override Token Match()
        {
            if (Current == QUOTE)
            {
                Eat();
                StringBuilder value = new StringBuilder();

                bool isEscape = false;
                bool inString = true;
                while (inString)
                {
                    while (!EOF && (isEscape || Current != QUOTE))
                    {
                        if (!isEscape && Current == ESCAPE)
                        {
                            Eat();
                            isEscape = true;
                        }
                        else if (isEscape)
                        {
                            isEscape = false;
                            switch (Current)
                            {
                                case 'n':
                                    Eat();
                                    value.Append('\n');
                                    break;

                                case 't':
                                    Eat();
                                    value.Append('\t');
                                    break;

                                default:
                                    value.Append(Current);
                                    Eat();
                                    break;
                            }
                        }
                        else
                        {
                            value.Append(Current);
                            Eat();
                        }
                    }

                    if (Current == QUOTE)
                    {
                        Eat();
                        inString = false;
                    }
                    else
                    {
                        throw new Exception("No matching end quote");
                    }
                }

                return CreateSymbol(TokenType.String, value.ToString());
            }

            return null;
        }
    }
}
