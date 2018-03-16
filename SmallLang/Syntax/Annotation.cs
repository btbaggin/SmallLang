using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang.Syntax
{
    public class Annotation
    {
        public string Value { get; private set; }
        public Annotation(string pValue)
        {
            Value = pValue;
        }
    }
}
