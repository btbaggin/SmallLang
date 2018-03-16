using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang
{
    public struct SmallError
    {
        public string Text { get; private set; }
        public TextSpan Span { get; private set; }

        public SmallError(string pText, TextSpan pSpan)
        {
            Text = pText;
            Span = pSpan;
        }
    }
}
