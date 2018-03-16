using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang.Emitting
{
    public class TypePrefixAttribute : Attribute
    {
        public string Prefix { get; private set; }
        public TypePrefixAttribute(string pPrefix)
        {
            Prefix = pPrefix;
        }
    }
}
