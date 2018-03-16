using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang
{
    public class FieldDefinition
    {
        public string Name { get; private set; }
        public SmallType Type { get; private set; }
        public System.Reflection.FieldInfo Info { get; internal set; }

        public FieldDefinition(string pName, SmallType pType)
        {
            Name = pName;
            Type = pType;
        }
    }
}
