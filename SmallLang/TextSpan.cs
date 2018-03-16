using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang
{
    public struct TextSpan
    {
        public int Start { get; private set; }
        public int Length { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }
        public int End
        {
            get { return Start + Length; }
        }

        public TextSpan(int pStart, int pEnd, int pLine, int pColumn)
        {
            Start = pStart;
            Length = pEnd - pStart;
            Line = pLine;
            Column = pColumn;
        }
    }
}
