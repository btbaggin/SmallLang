using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Lexing.Definitions
{
    class CommentDefinition : TokenDefinition
    {
        public CommentDefinition()
        {
            IncludeSymbol = false;
        }

        public override Token Match()
        {
            if (Current == '#')
            {
                Eat();
                while (!EOF && Current != '\r' && Current != '\n')
                {
                    Eat();
                }

                return CreateSymbol(TokenType.Comment);
            }

            return null;
        }
    }

    class BlockCommentDefinition : TokenDefinition
    {
        public BlockCommentDefinition()
        {
            IncludeSymbol = false;
        }

        public override Token Match()
        {
            if (Current == '#' && Peek(1) == '-')
            {
                Eat();
                Eat();
                bool isComment = false;
                while (!EOF && !isComment)
                {
                    Eat();
                    isComment = (Current == '-' && Peek(1) == '#');
                }

                if (isComment)
                {
                    Eat();
                    Eat();
                    return CreateSymbol(TokenType.Comment);
                }
            }

            return null;
        }
    }
}
