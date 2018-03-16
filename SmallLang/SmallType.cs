using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang
{
    public partial class SmallType
    {
        public static SmallType I16 => new SmallType("I16");
        public static SmallType I32 => new SmallType("I32");
        public static SmallType I64 => new SmallType("I64");
        public static SmallType Float => new SmallType("Flt");
        public static SmallType Double => new SmallType("Dbl");
        public static SmallType String => new SmallType("Str");
        public static SmallType Boolean => new SmallType("Bln");
        public static SmallType Date => new SmallType("Dtm", true);
        public static SmallType Object => new SmallType("Obj");

        public static SmallType Undefined => new SmallType("");

        static TypeDictionary _types;
        private SmallType _elementType;
        private Dictionary<string, FieldDefinition> _fields;

        #region Properties
        public bool Exported { get; internal set; }

        public string Namespace { get; private set; }

        public string Name { get; private set; }

        public bool IsVariant
        {
            get
            {
                if (Name.Length < 3) return false;
                if (IsArray) return Name.Substring(1, 3).Equals("vnt", StringComparison.OrdinalIgnoreCase);
                else return Name.Substring(0, 3).Equals("vnt", StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool IsArray
        {
            get { return _elementType != null; }
        }

        public bool IsGeneric
        {
            get { return GenericTypeArgs.Length > 0; }
        }

        public bool IsGenericDefinition
        {
            get { return GenericTypeParameters.Count > 0; }
        }

        public bool ContainsGenericTypeParameters
        {
            get { return GenericTypeParameters.Count > 0; }
        }

        public SmallType[] GenericTypeArgs { get; private set; }

        public IList<SmallType> GenericTypeParameters { get; private set; }

        public bool IsGenericTypeParameter { get; private set; }

        public bool IsValueType { get; private set; }

        public bool IsTupleType
        {
            get { return Name.Equals("Tuple", StringComparison.OrdinalIgnoreCase); }
        }

        public InitializerInfo Initializer { get; private set; }

        public bool HasInitializer { get; private set; }
        #endregion

        static SmallType()
        {
            ClearTypes();
        }
        
        private SmallType(string pName) : this(pName, false) { }
        private SmallType(string pName, bool pIsValueType) : this("", pName, pIsValueType) { }
        public SmallType(string pNamespace, string pName, bool pIsValueType)
        {
            Namespace = pNamespace;
            Name = pName;
            IsValueType = pIsValueType;
            GenericTypeArgs = new SmallType[0];
            GenericTypeParameters = new List<SmallType>();
            _fields = new Dictionary<string, FieldDefinition>();
        }

        public static SmallType Create(string pNamespace, string pName)
        {
            return _types.RetrieveType(pNamespace, pName);
        }

        public static SmallType CreateGenericParameter(string pName)
        {
            SmallType ret = new SmallType(pName)
            {
                IsGenericTypeParameter = true
            };
            return ret;
        }

        public static SmallType CreateGenericArgument(string pName)
        {
            return new SmallType(pName);
        }

        #region Static methods
        internal static void ClearTypes()
        {
            _types = new TypeDictionary();
            RegisterType(I16, typeof(short));
            RegisterType(I32, typeof(int));
            RegisterType(I64, typeof(long));
            RegisterType(Float, typeof(float));
            RegisterType(Double, typeof(double));
            RegisterType(String, typeof(string));
            RegisterType(Boolean, typeof(bool));
            RegisterType(Date, typeof(DateTime));
            RegisterType(Object, typeof(object));
            RegisterType(new SmallType("vnt"), typeof(object));
        }

        public static SmallType RegisterType(string pNamespace, string pName, bool pIsValueType, IList<string> pTypeHints)
        {
            List<SmallType> types = new List<SmallType>();
            foreach(var t in pTypeHints)
            {
                types.Add(CreateGenericParameter(t));
            }

            var st = new SmallType(pNamespace, pName, pIsValueType)
            {
                GenericTypeParameters = types
            };
            _types.AddType(st);
            return st;
        }

        public static void RegisterType(SmallType pSmallType, Type pType)
        {
            _types.AddType(pSmallType);
            if (pType != null) _types.SetSystemType(pSmallType, pType);
        }

        public static SmallType FromString(string pNamespace, string pName)
        {
            SmallType type = _types.FromString(pNamespace, pName);

            if (type == Undefined && pName.Length > 0 && Char.ToUpperInvariant(pName[0]) == 'A')
            {
                pName = pName.Substring(1);
                type = _types.FromString(pNamespace, pName);
                type = type.MakeArrayType();
            }

            return type;
        }

        public static SmallType CreateTupleOf(params SmallType[] pTypes)
        {
            SmallType t = new SmallType("Tuple", true) { GenericTypeArgs = pTypes };
            return t;
        }
        #endregion

        public void SetInitializer(InitializerInfo pInitializer)
        {
            Initializer = pInitializer;
            HasInitializer = true;
        }

        Type _systemType;
        internal void SetSystemType(Type pType)
        {
            if (IsGenericTypeParameter) _systemType = pType;
            else _types.SetSystemType(this, pType);
        }

        public void AddGenericTypeParameter(string pHint)
        {
            var t = CreateGenericParameter(pHint);
            GenericTypeParameters.Add(t);   
        }

        public void AddField(string pName, IEnumerable<string> pTypeHint, SmallType pType)
        {
            foreach(var t in pTypeHint)
            {
                pType.AddGenericTypeParameter(t);
            }
            _fields.Add(pName, new FieldDefinition(pName, pType));
        }

        public bool FieldExists(string pName)
        {
            return _fields.ContainsKey(pName);
        }

        public FieldDefinition GetField(string pName)
        {
            if (!_fields.ContainsKey(pName)) return null;

            var fd =_fields[pName];
            if(IsGeneric && fd.Type.IsGenericDefinition)
            {
                SmallType t = null;
                if (GenericTypeArgs.Length > 1) t = CreateTupleOf(GenericTypeArgs);
                else t = GenericTypeArgs[0];

                if (fd.Type.IsArray) t = t.MakeArrayType();
                fd = new FieldDefinition(fd.Name, t);
            }
            return fd;
        }

        public FieldDefinition[] GetFields()
        {
            FieldDefinition[] fields = new FieldDefinition[_fields.Count];
            int i = 0;
            foreach(var fd in _fields.Values)
            {
                fields[i] = fd;
                i++;
            }
            return fields;
        }

        public SmallType MakeArrayType()
        {
            var st = Copy();
            st.Name = "A" + st.Name;
            st._elementType = this;
            return st;
        }

        public SmallType MakeGenericType(SmallType[] pType)
        {
            SmallType ret = Copy();
            ret.GenericTypeArgs = pType;
            return ret;
        }

        public SmallType GetElementType()
        {
            if (IsArray) return _elementType;
            return Undefined;
        }

        public bool IsAssignableFrom(SmallType pType)
        {
            if (this == pType || IsGenericTypeParameter) return true;

            if (IsGenericDefinition && pType.IsGenericDefinition && GenericTypeParameters == pType.GenericTypeParameters)
            {
                bool match = true;
                if (IsGeneric)
                {
                    for (int i = 0; i < GenericTypeParameters.Count; i++)
                    {
                        if (!GenericTypeArgs[i].IsGenericTypeParameter) match = false;
                    }
                }

                return match;
            }
            else if (IsVariant && pType.IsGenericTypeParameter) return true;

            return false;
        }

        public static SmallType FromSystemType(Type pType)
        {
            if (pType.IsByRef) pType = pType.GetElementType();
            var t = _types.FromSystemType(pType);
            if (t != null) return t;

            if(pType.IsArray)
            {
                t = FromSystemType(pType.GetElementType());
                t = t.MakeArrayType();
                return t;
            }
            else if (pType.IsConstructedGenericType)
            {
                var ts = pType.GetGenericArguments();
                SmallType[] types = new SmallType[ts.Length];
                for(int i = 0; i < ts.Length; i++)
                {
                    types[i] = FromSystemType(ts[i]);
                }
                return CreateTupleOf(types);
            }

            if(pType.IsGenericParameter)
            {
                return CreateGenericParameter(pType.Name);
            }
            return null;
        }

        public Type ToSystemType()
        {
            Type t = null;
            if (Name == "Tuple")
            {
                Type[] typeArgs = new Type[Math.Min(GenericTypeArgs.Length, 8)];
                for (int i = 0; i < Math.Min(GenericTypeArgs.Length, 8); i++)
                {
                    typeArgs[i] = GenericTypeArgs[i].ToSystemType();
                }

                switch (GenericTypeArgs.Length)
                {
                    case 1:
                        t = typeof(ValueTuple<>);
                        break;
                    case 2:
                        t = typeof(ValueTuple<,>);
                        break;
                    case 3:
                        t = typeof(ValueTuple<,,>);
                        break;
                    case 4:
                        t = typeof(ValueTuple<,,,>);
                        break;
                    case 5:
                        t = typeof(ValueTuple<,,,,>);
                        break;
                    case 6:
                        t = typeof(ValueTuple<,,,,,>);
                        break;
                    case 7:
                        t = typeof(ValueTuple<,,,,,,>);
                        break;
                    case 8:
                        t = typeof(ValueTuple<,,,,,,,>);
                        break;
                    default:
                        throw new Exception();
                }
                return t.MakeGenericType(typeArgs);
            }
            else if (string.IsNullOrEmpty(Name)) return null;
            else if (_types.HasType(Namespace, Name))
            {
                t = _types.GetSystemType(Namespace, Name);
            }
            else if (IsArray)
            {
                t = _types.GetSystemType(Namespace, _elementType.Name);
                t = t.MakeArrayType();
            }
            else if (IsGenericTypeParameter)
            {
                t = _systemType;
            }
            else
            {
                throw new Exception("Unknown type " + Name);
            }

            if (GenericTypeArgs.Length != 0)
            {
                Type[] ta = new Type[GenericTypeArgs.Length];
                for(int i= 0; i < ta.Length; i++)
                {
                    ta[i] = GenericTypeArgs[i].ToSystemType();
                }
                t = t.MakeGenericType(ta);
            }
            return t;
        }

        internal void Create()
        {
            if(_types.HasType(Namespace, Name))
            {
                var t = _types.GetSystemType(Namespace, Name);
                if(t.GetType() == typeof(System.Reflection.Emit.TypeBuilder))
                {
                    _types.SetSystemType(this, ((System.Reflection.Emit.TypeBuilder)t).CreateType());
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            if(GenericTypeArgs.Length > 0)
            {
                sb.Append("<");
                foreach (var a in GenericTypeArgs)
                {
                    sb.Append(a.Name);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(">");
            }
            return sb.ToString();
        }

        private SmallType Copy()
        {
            var st = new SmallType(Namespace, Name, IsValueType)
            {
                _fields = _fields,
                GenericTypeArgs = GenericTypeArgs,
                GenericTypeParameters = GenericTypeParameters,
                _elementType = _elementType,
                _systemType = _systemType,
                Exported = Exported
            };
            return st;
        }

        #region Operators
        public static bool operator ==(SmallType pT1, SmallType pT2)
        {
            if ((object)pT1 == null && (object)pT2 != null) return false;
            if ((object)pT2 == null && (object)pT1 != null) return false;
            if ((object)pT2 == null && (object)pT1 == null) return true;
            if (pT1.Name != pT2.Name) return false;
            if (pT1.GenericTypeArgs.Length != pT2.GenericTypeArgs.Length) return false;

            for (int i = 0; i < pT1.GenericTypeArgs.Length; i++)
            {
                if (pT1.GenericTypeArgs[i] != pT2.GenericTypeArgs[i]) return false;
            }
            return true;
        }

        public static bool operator !=(SmallType pT1, SmallType pT2)
        {
            if ((object)pT1 == null && (object)pT2 != null) return true;
            if ((object)pT2 == null && (object)pT1 != null) return true;
            if ((object)pT2 == null && (object)pT1 == null) return false;
            if (pT1.Name != pT2.Name) return true;
            if (pT1.GenericTypeArgs.Length != pT2.GenericTypeArgs.Length) return true;

            for (int i = 0; i < pT1.GenericTypeArgs.Length; i++)
            {
                if (pT1.GenericTypeArgs[i] != pT2.GenericTypeArgs[i]) return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SmallType)) return false;
            return (SmallType)obj == this;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion
    }
}
