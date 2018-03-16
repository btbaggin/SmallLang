using System;
using System.Linq;
using System.Collections.Generic;

namespace SmallLang
{
    class MetadataCache
    {
        public class LocalScope : IDisposable
        {
            public LocalScope()
            {
                StartLocalScope();
            }

            public void Dispose()
            {
                EndLocalScope();
            }
        }

        static Dictionary<string, List<MethodDefinition>> _methods;
        static Dictionary<string, Dictionary<string, List<MethodDefinition>>> _importedMethods;
        static HashSet<string> _casts;
        static List<string> _importedNamespaces;

        static MetadataCache()
        {
            _methods = new Dictionary<string, List<MethodDefinition>>(StringComparer.OrdinalIgnoreCase);
            _importedMethods = new Dictionary<string, Dictionary<string, List<MethodDefinition>>>(StringComparer.OrdinalIgnoreCase);
            _importedNamespaces = new List<string>();
            _casts = new HashSet<string>();
        }

        internal static void Clear()
        {
            _methods.Clear();
            _importedMethods.Clear();
            _importedNamespaces.Clear();
            _casts.Clear();
            SmallType.ClearTypes();
        }

        public static MethodDefinition AddCast(SmallType pType1, string pParameterName, SmallType pType2)
        {
            var name = CastFunction(pType1, pType2);

            var md = AddMethod(name,
                               new MethodDefinition.Parameter[] { new MethodDefinition.Parameter(pType1, pParameterName, false) },
                               new SmallType[] { pType2 },
                               new List<SmallType>()); //TODO?
            _casts.Add(name);
            return md;
        }
        
        public static MethodDefinition AddImportedCast(string pNamespace, SmallType pType1, string pParameterName, SmallType pType2)
        {
            var name = CastFunction(pType1, pType2);
            var md = AddImportedMethod(pNamespace,
                                       name,
                                       new MethodDefinition.Parameter[] { new MethodDefinition.Parameter(pType1, pParameterName, false) },
                                       new SmallType[] { pType2 },
                                       new List<SmallType>()); //TODO?
            _casts.Add(name);
            return md;
        }

        public static string CastFunction(SmallType pType1, SmallType pType2)
        {
            return pType1.ToString() + "___" + pType2.ToString();
        }

        public static bool CastExists(SmallType pType1, SmallType pType2)
        {
            if (pType1 == pType2) return true;
            return _casts.Contains(CastFunction(pType1, pType2));
        }

        internal static void AddImplicitCasts()
        {
            AddCast(SmallType.I16, "i16Num", SmallType.I32);
            AddCast(SmallType.I16, "i16Num", SmallType.I64);
            AddCast(SmallType.I16, "i16Num", SmallType.Double);
            AddCast(SmallType.I16, "i16Num", SmallType.Float);

            AddCast(SmallType.I32, "i32Num", SmallType.I16);
            AddCast(SmallType.I32, "i32Num", SmallType.I64);
            AddCast(SmallType.I32, "i32Num", SmallType.Double);
            AddCast(SmallType.I32, "i32Num", SmallType.Float);

            AddCast(SmallType.I64, "i32Num", SmallType.I16);
            AddCast(SmallType.I64, "i32Num", SmallType.I32);
            AddCast(SmallType.I64, "i32Num", SmallType.Double);
            AddCast(SmallType.I64, "i32Num", SmallType.Float);

            AddCast(SmallType.Double, "i32Num", SmallType.I16);
            AddCast(SmallType.Double, "i32Num", SmallType.I32);
            AddCast(SmallType.Double, "i32Num", SmallType.I64);
            AddCast(SmallType.Double, "i32Num", SmallType.Float);

            AddCast(SmallType.Float, "i32Num", SmallType.I16);
            AddCast(SmallType.Float, "i32Num", SmallType.I32);
            AddCast(SmallType.Float, "i32Num", SmallType.I64);
            AddCast(SmallType.Float, "i32Num", SmallType.Double);
        }

        public static MethodDefinition AddMethod(string pName, MethodDefinition.Parameter[] pParameters, SmallType[] pReturnTypes, IList<SmallType> pTypeHints)
        {
            var md = new MethodDefinition(pName, pParameters, pReturnTypes, pTypeHints);
            if (!_methods.ContainsKey(pName)) _methods.Add(pName, new List<MethodDefinition>());
            _methods[pName].Add(md);
            return md;
        }

        public static MethodDefinition AddImportedMethod(string pNamespace, string pName, MethodDefinition.Parameter[] pParameters, SmallType[] pReturnTypes, IList<SmallType> pTypeHints)
        {
            var md = new MethodDefinition(pName, pParameters, pReturnTypes, pTypeHints);
            if (!_importedMethods.ContainsKey(pNamespace)) _importedMethods.Add(pNamespace, new Dictionary<string, List<MethodDefinition>>());
            if (!_importedMethods[pNamespace].ContainsKey(pName)) _importedMethods[pNamespace].Add(pName, new List<MethodDefinition>());
            _importedMethods[pNamespace][pName].Add(md);
            return md;
        }

        public static IEnumerable<string> ImportedNamespaces()
        {
            return _importedNamespaces;
        }

        public static void AddNamespace(string pNamespace)
        {
            _importedNamespaces.Add(pNamespace);
        }

        public static MethodDefinition FindBestOverload(Syntax.FunctionInvocationSyntax pFunc, out bool pExact)
        {
            List<MethodDefinition> methods = null;
            if (string.IsNullOrEmpty(pFunc.Namespace))
            {
                methods = new List<MethodDefinition>();
                if (_methods.ContainsKey(pFunc.Value)) methods.AddRange(_methods[pFunc.Value]);
                if (_importedMethods.ContainsKey("") && _importedMethods[""].ContainsKey(pFunc.Value)) methods.AddRange(_importedMethods[""][pFunc.Value]);
            }
            else methods = _importedMethods[pFunc.Namespace][pFunc.Value];

            SmallType[] parameters = new SmallType[pFunc.Arguments.Count];
            for (int i = 0; i < pFunc.Arguments.Count; i++)
            {
                parameters[i] = pFunc.Arguments[i].Type;
            }

            pExact = false;
            MethodDefinition method = null;
            foreach (var md in methods)
            {
                if (method == null) method = md;

                if (md.Parameters.Length == parameters.Length)
                {
                    pExact = true;
                    method = md;
                    for (int i = 0; i < md.Parameters.Length; i++)
                    {
                        pExact = CompareParameter(parameters[i], md.Parameters[i].Type, pFunc.TypeParameters);
                        if (!pExact) break;
                    }
                }
            }

            return method;
        }
        
        public static MethodDefinition GetMethod(Syntax.FunctionInvocationSyntax pFunc)
        {
            List<MethodDefinition> methods = null;
            if (string.IsNullOrEmpty(pFunc.Namespace))
            {
                methods = new List<MethodDefinition>();
                if (_methods.ContainsKey(pFunc.Value)) methods.AddRange(_methods[pFunc.Value]);
                if (_importedMethods.ContainsKey("") && _importedMethods[""].ContainsKey(pFunc.Value)) methods.AddRange(_importedMethods[""][pFunc.Value]);
            }
            else methods = _importedMethods[pFunc.Namespace][pFunc.Value];

            SmallType[] parameters = new SmallType[pFunc.Arguments.Count];
            for (int i = 0; i < pFunc.Arguments.Count; i++)
            {
                parameters[i] = pFunc.Arguments[i].Type;
            }

            foreach (var md in methods)
            {
                if (md.Parameters.Length == parameters.Length)
                {
                    bool found = true;
                    for (int i = 0; i < md.Parameters.Length; i++)
                    {
                        found = CompareParameter(parameters[i], md.Parameters[i].Type, pFunc.TypeParameters);
                        if (!found) break;
                    }

                    if (found) return md;
                }
            }

            throw new Exception("Unable to find matching function");
        }

        private static bool CompareParameter(SmallType pGiven, SmallType pDeclared, IList<string> pTypeHints)
        {
            if (pGiven == pDeclared) return true;

            //Type parameters do not match, see if they are generics
            if (pGiven.IsGenericDefinition && pDeclared.IsGenericDefinition && pTypeHints.Count > 0)
            {
                if(pGiven.IsGeneric)
                {
                    for (int j = 0; j < pTypeHints.Count; j++)
                    {
                        if (pGiven.GenericTypeArgs[j] != SmallType.CreateGenericArgument(pTypeHints[j])) return false;
                    }
                }

                return true;

            }
            else if (pDeclared.IsGenericTypeParameter) return true;

            return false;
        }

        public static MethodDefinition GetCast(SmallType pType, SmallType pTypeTo)
        {
            var name = CastFunction(pType, pTypeTo);
            List<MethodDefinition> methods = null;
            if (_methods.ContainsKey(name))
            {
                methods = _methods[name];
                if (methods.Count == 1) return methods[0];
            }
            else
            {
                foreach (var kv in _importedMethods)
                {
                    if (_importedMethods[kv.Key].ContainsKey(name))
                    {
                        methods = _importedMethods[kv.Key][name];
                        if (methods.Count == 1) return methods[0];
                        break;
                    }
                }
            }

            throw new Exception("Unable to find matching function");
        }

        public static bool MethodExists(string pNamespace, string pName)
        {
            if (_methods.ContainsKey(pName)) return true;
            else if (_importedMethods.ContainsKey(pNamespace)) return _importedMethods[pNamespace].ContainsKey(pName);
            return false;
        }

        static List<Dictionary<string, LocalDefinition>> _locals = new List<Dictionary<string, LocalDefinition>>();
        public static LocalDefinition DefineLocal(Syntax.IdentifierSyntax pNode, SmallType pType)
        {
            var ld = LocalDefinition.Create(pNode, pNode.Value, pType);
            _locals[_locals.Count - 1].Add(pNode.Value, ld);
            return ld;
        }

        public static LocalDefinition DefineParameter(Syntax.ParameterSyntax pNode, SmallType pType)
        {
            var ld = LocalDefinition.CreateAsParameter(pNode, pNode.IsRef, pNode.Value, pType);
            _locals[_locals.Count - 1].Add(pNode.Value, ld);
            return ld;
        }

        public static LocalDefinition DefineField(Syntax.IdentifierSyntax pNode, SmallType pType, SmallType pStructType)
        {
            return LocalDefinition.CreateAsField(pNode, pNode.Value, pType, pStructType);
        }

        public static LocalDefinition GetLocal(string pName)
        {
            for(int i = _locals.Count - 1; i >= 0; i--)
            {
                if (_locals[i].ContainsKey(pName)) return _locals[i][pName];
            }

            return null;
        }

        public static bool LocalExists(string pName)
        {
            foreach (var d in _locals)
            {
                if (d.ContainsKey(pName)) return true;
            }

            return false;
        }

        public static bool LocalExistsInThisScope(string pName)
        {
            return _locals[_locals.Count - 1].ContainsKey(pName);
        }

        public static void StartLocalScope()
        {
            _locals.Add(new Dictionary<string, LocalDefinition>(StringComparer.OrdinalIgnoreCase));
        }

        public static void EndLocalScope()
        {
            _locals.RemoveAt(_locals.Count - 1);
        }
    }
}
