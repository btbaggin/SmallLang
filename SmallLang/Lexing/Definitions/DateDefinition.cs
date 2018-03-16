using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class DateDefinition : TokenDefinition
    {
        const char QUOTE = '\'';
        public override Token Match()
        {
            if (Current != QUOTE) return null;
            Eat();

            StringBuilder value = new StringBuilder();

            //Day
            if (!char.IsNumber(Current)) return null;
            value.Append(Current);
            Eat();

            if (!char.IsNumber(Current)) return null;
            value.Append(Current);
            Eat();

            if (Current != ',') return null;
            value.Append(Current);
            Eat();

            //Month
            if (!char.IsNumber(Current)) return null;
            value.Append(Current);
            Eat();

            if (!char.IsNumber(Current)) return null;
            value.Append(Current);
            Eat();

            if (Current != ',') return null;
            value.Append(Current);
            Eat();

            //Year
            if (!char.IsNumber(Current)) return null;
            value.Append(Current);
            Eat();

            if (!char.IsNumber(Current)) return null;
            value.Append(Current);
            Eat();

            if (!char.IsNumber(Current)) return null;
            value.Append(Current);
            Eat();

            if (!char.IsNumber(Current)) return null;
            value.Append(Current);
            Eat();

            if (Current != QUOTE) return null;
            Eat();

            return CreateSymbol(TokenType.Date, value.ToString());
        }
    }
}
