using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace SmallLang
{
    public enum Scope
    {
        Public,
        Private
    }

    public class MethodDefinition
    {
        public struct Parameter
        {
            public SmallType Type { get; set; }
            public string Name { get; set; }
            public bool IsRef { get; set; }

            public Parameter(SmallType pType, string pName, bool pIsRef)
            {
                Type = pType;
                Name = pName;
                IsRef = pIsRef;
            }

            public Parameter(Syntax.ParameterSyntax pParameter)
            {
                Type = pParameter.Type;
                Name = pParameter.Value;
                IsRef = pParameter.IsRef;
            }
        }

        //Emitting stuff
        private MethodBuilder _builder;
        public ILGenerator Emitter { get; private set; }
        public MethodInfo CallSite { get; private set; }
        private readonly Dictionary<string, (ParameterBuilder, short, Type)> _parameters;
        public bool IsMain { get; set; }
        public MethodInfo ExternMethod { get; private set; }

        public Scope Scope { get; private set; }
        public string Name { get; private set; }
        public Parameter[] Parameters { get; private set; }
        public SmallType[] ReturnTypes { get; private set; }
        public IList<SmallType> TypeHints { get; private set; }

        public MethodDefinition(string pName, 
                                Parameter[] pParameters, 
                                SmallType[] pReturnTypes,
                                IList<SmallType> pTypeHints)
        {
            _parameters = new Dictionary<string, (ParameterBuilder, short, Type)>();

            Name = pName;
            Parameters = pParameters;
            ReturnTypes = pReturnTypes;
            TypeHints = pTypeHints;
            Scope = Scope.Private;
        }

        public void SetExternInfo(string pAssembly, string pType, string pMethod)
        {
            MethodInfo m = null;
            TypeInfo t = null;
            try
            {
                var p = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                p = System.IO.Path.Combine(p, "Reference Assemblies", "Microsoft", "Framework", ".NETFramework", "v4.5.1");
                var a = Assembly.LoadFile(System.IO.Path.Combine(p, pAssembly));
                t = a.GetType(pType).GetTypeInfo();

                //Try to get just on name... 
                //This can get things that take enums we don't have access to
                m = t.GetMethod(pMethod);
            }
            catch (Exception)
            {
                //Ignore exception
            };

            if (t == null)
                Compiler.ReportError(CompilerErrorType.InvalidExternalMethod, new TextSpan(), pMethod);

            if(m == null)
            {
                Type[] types = new Type[Parameters.Length];
                for (int i = 0; i < Parameters.Length; i++)
                {
                        types[i] = Parameters[i].Type.ToSystemType();
                }
                m = t.GetMethod(pMethod, types);
            }


            ExternMethod = m;
        }

        public void SetExternMethod(MethodInfo pMethod)
        {
            ExternMethod = pMethod;
            CallSite = pMethod;
        }

        internal void SetBuilder(MethodBuilder pBuilder)
        {
            _builder = pBuilder;
            Emitter = _builder.GetILGenerator();
            CallSite = _builder;
        }

        internal void SetScope(Scope pScope)
        {
            Scope = pScope;
        }

        public SmallType GetReturnType()
        {
            if (ReturnTypes.Length == 0) return SmallType.Undefined;
            if (ReturnTypes.Length == 1) return ReturnTypes[0];
            return SmallType.CreateTupleOf(ReturnTypes);
        }

        #region Emitting functions
        public LocalBuilder CreateLocal(string pName, SmallType pType)
        {
            return Emitter.DeclareLocal(pType.ToSystemType());
        }

        public bool HasParameter(string pName)
        {
            return _parameters.ContainsKey(pName);
        }

        public ParameterBuilder GetParameter(string pName, out short pIndex)
        {
            if(_parameters.ContainsKey(pName))
            {
                var t = _parameters[pName];
                pIndex = t.Item2;
                return t.Item1;
            }
            pIndex = 0;
            return null;
        }

        public ParameterBuilder CreateParameter(string pName, short pIndex, SmallType pType, bool pIsRef)
        {
            var pb = _builder.DefineParameter(pIndex, ParameterAttributes.None, pName);
            var t = pType.ToSystemType();
            if (pIsRef)
                t = t.MakeByRefType();
            _parameters.Add(pName, (pb, pIndex, t));
            return pb;
        }
        #endregion
    }
}
