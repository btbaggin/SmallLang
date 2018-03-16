using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang
{
    interface ITokenSource
    {
        int Index { get; }
        int Line { get; }
        int Column { get; }

        bool GetNextToken(out Token pToken);
    }
}
