using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class NumberDefinition : TokenDefinition
    {
        public override Token Match()
        {
            TokenType type = TokenType.EndOfFile;

            StringBuilder value = new StringBuilder();
            while (!EOF && char.IsDigit(Current))
            {
                type = TokenType.I32;
                value.Append(Current);
                Eat();
            }

            if (Current == '.' && char.IsDigit(Peek(1)))
            {
                value.Append(Current);
                Eat();
                type = TokenType.Float;

                while (!EOF && char.IsDigit(Current))
                {
                    value.Append(Current);
                    Eat();
                }
            }

            if (type != TokenType.EndOfFile)
            {
                if (type == TokenType.Float)
                {
                    switch (Current)
                    {
                        case 'F':
                            type = TokenType.Float;
                            Eat();
                            break;
                        case 'D':
                            type = TokenType.Double;
                            Eat();
                            break;
                    }
                }
                else
                {
                    switch (Current)
                    {
                        case 'S':
                            type = TokenType.I16;
                            Eat();
                            break;
                        case 'L':
                            type = TokenType.I64;
                            Eat();
                            break;
                        case 'I':
                            type = TokenType.I32;
                            Eat();
                            break;
                        case 'D':
                            type = TokenType.Double;
                            Eat();
                            break;

                        case 'F':
                            type = TokenType.Float;
                            Eat();
                            break;
                    }
                }

                if (IsWordBreak(Current)) return CreateSymbol(type, value.ToString());
            }

            return null;
        }
    }
}
