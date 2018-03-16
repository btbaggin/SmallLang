using System;
using System.Collections.Generic;

namespace SmallLang
{
    public partial class SmallType
    {
        private class TypeDictionary
        {
            readonly Dictionary<string, Tuple<SmallType, Type>> _types;
            public TypeDictionary()
            {
                _types = new Dictionary<string, Tuple<SmallType, Type>>();
            }

            private static string GetKey(string pNamespace, string pName)
            {
                return pNamespace + "___" + pName;
            }

            public void AddType(SmallType pType)
            {
                var key = GetKey(pType.Namespace, pType.Name);
                if (!_types.ContainsKey(key)) _types.Add(key, Tuple.Create<SmallType, Type>(pType, null));
            }

            public void SetSystemType(SmallType pType, Type pSystemType)
            {
                var key = GetKey(pType.Namespace, pType.Name);
                _types[key] = Tuple.Create(pType, pSystemType);
            }

            public bool HasType(string pNamespace, string pName)
            {
                var key = GetKey(pNamespace, pName);
                return _types.ContainsKey(key);
            }

            public SmallType RetrieveType(string pNamespace, string pName)
            {
                var key = GetKey(pNamespace, pName);
                return _types[key].Item1;
            }

            public Type GetSystemType(string pNamespace, string pName)
            {
                var key = GetKey(pNamespace, pName);
                if (_types.ContainsKey(key))
                {
                    return _types[key].Item2;
                }
                return null;
            }

            public SmallType FromSystemType(Type pType)
            {
                foreach(var kv in _types)
                {
                    if(kv.Value.Item2 != null)
                    {
                        if (kv.Value.Item2 == pType)
                            return kv.Value.Item1;
                        else if (kv.Value.Item2.IsGenericType && 
                                 pType.IsGenericType &&
                                 kv.Value.Item2.GetGenericTypeDefinition() == pType.GetGenericTypeDefinition())
                        {
                            return kv.Value.Item1;
                        }
                    }
                }
                return null;
            }

            public SmallType FromString(string pNamespace, string pName)
            {
                foreach (var kv in _types)
                {
                    var type = kv.Value.Item1.Name;
                    if (pName.Length >= type.Length && type.Equals(pName.Substring(0, type.Length), StringComparison.OrdinalIgnoreCase))
                    {
                        return kv.Value.Item1;
                    }
                }
                return Undefined;
            }
        }
    }
}
