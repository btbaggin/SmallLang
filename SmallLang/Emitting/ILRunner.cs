using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace SmallLang.Emitting
{
    public class ILRunner
    {
        readonly string _exeName;
        readonly AssemblyBuilder _assembly;
        readonly ModuleBuilder _builder;

        readonly TypeBuilder _currentType;
        public MethodDefinition CurrentMethod { get; private set; }

        internal ILGenerator OverrideGenerator { get; set; }
        public ILGenerator Emitter
        {
            get
            {
                if (OverrideGenerator != null) return OverrideGenerator;
                return CurrentMethod.Emitter;
            }
        }

        public ILRunner(CompilationOptions pOptions)
        {
            _exeName = pOptions.ExeName;

            var an = new AssemblyName(pOptions.AssemblyName);
            _assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Save, pOptions.OutputPath);
            _builder = _assembly.DefineDynamicModule(pOptions.ModuleName, _exeName);
            _currentType = _builder.DefineType(pOptions.TypeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit, null);
        }

        public void Finish()
        {
            _currentType.CreateType();
            System.IO.File.Delete(_exeName);
            _assembly.Save(_exeName);
        }

        #region Emitting functions
        public void EmitInt(int pValue)
        {
            switch (pValue)
            {
                case -1:
                    Emitter.Emit(OpCodes.Ldc_I4_M1);
                    break;

                case 0:
                    Emitter.Emit(OpCodes.Ldc_I4_0);
                    break;

                case 1:
                    Emitter.Emit(OpCodes.Ldc_I4_1);
                    break;

                case 2:
                    Emitter.Emit(OpCodes.Ldc_I4_2);
                    break;

                case 3:
                    Emitter.Emit(OpCodes.Ldc_I4_3);
                    break;

                case 4:
                    Emitter.Emit(OpCodes.Ldc_I4_4);
                    break;

                case 5:
                    Emitter.Emit(OpCodes.Ldc_I4_5);
                    break;

                case 6:
                    Emitter.Emit(OpCodes.Ldc_I4_6);
                    break;

                case 7:
                    Emitter.Emit(OpCodes.Ldc_I4_7);
                    break;

                case 8:
                    Emitter.Emit(OpCodes.Ldc_I4_8);
                    break;

                default:
                    Emitter.Emit(OpCodes.Ldc_I4, pValue);
                    break;
            }
        }

        public void EmitBool(bool pValue)
        {
            if (pValue) Emitter.Emit(OpCodes.Ldc_I4_1);
            else Emitter.Emit(OpCodes.Ldc_I4_0);
        }

        public LocalBuilder CreateLocal(string pValue, SmallType pType)
        {
            return CurrentMethod.CreateLocal(pValue, pType);
        }

        public MethodInfo EmitFunction(MethodDefinition pDefinition)
        {
            if (pDefinition.ExternMethod != null)
            {
                pDefinition.SetExternMethod(pDefinition.ExternMethod);
                if (pDefinition.ExternMethod.ContainsGenericParameters)
                {
                    for (int i = 0; i < pDefinition.TypeHints.Count; i++)
                    {
                        var args = pDefinition.ExternMethod.GetGenericArguments();
                        for (int j = 0; j < args.Length; j++)
                        {
                            if (pDefinition.TypeHints[i].Name == args[j].Name) pDefinition.TypeHints[i].SetSystemType(args[j]);
                        }
                    }
                }
                return pDefinition.CallSite;
            }

            var ma = MethodAttributes.HideBySig | MethodAttributes.Static;
            switch (pDefinition.Scope)
            {
                case Scope.Private:
                    ma |= MethodAttributes.Private;
                    break;

                case Scope.Public:
                    ma |= MethodAttributes.Public;
                    break;
            }

            CurrentMethod = pDefinition;
            var mb = _currentType.DefineMethod(pDefinition.Name, ma, null, null);
            CurrentMethod.SetBuilder(mb);

            Type[] types = new Type[pDefinition.Parameters.Length];

            Type[] typeParms = null;
            if (pDefinition.TypeHints.Count > 0)
            {
                string[] typeNames = new string[pDefinition.TypeHints.Count];
                for (int i = 0; i < typeNames.Length; i++)
                {
                    typeNames[i] = pDefinition.TypeHints[i].Name;
                }
                typeParms = mb.DefineGenericParameters(typeNames);
            }

            for (int i = 0; i < pDefinition.Parameters.Length; i++)
            {
                var st = pDefinition.Parameters[i].Type;
                Type t = null;
                if (typeParms != null)
                {
                    for (int j = 0; j < typeParms.Length; j++)
                    {
                        for (int k = 0; k < st.GenericTypeArgs.Length; k++)
                        {
                            var stt = st.GenericTypeArgs[k];
                            if (stt.Name == typeParms[j].Name)
                            {
                                t = typeParms[j];
                                stt.SetSystemType(t);
                            }
                        }
                    }

                    if (st.IsGenericTypeParameter)
                    {
                        t = typeParms[0];
                        st.SetSystemType(t);
                    }
                }

                types[i] = st.ToSystemType();
                if (pDefinition.Parameters[i].IsRef)
                    types[i] = types[i].MakeByRefType();

            }

            SmallType srt = pDefinition.GetReturnType();
            if (typeParms != null)
            {
                for (int j = 0; j < typeParms.Length; j++)
                {
                    if (srt.Name == typeParms[j].Name)
                        srt.SetSystemType(typeParms[j]);
                }
            }

            mb.SetParameters(types);
            mb.SetReturnType(srt.ToSystemType());

            if (pDefinition.IsMain)
            {
                _assembly.SetEntryPoint(CurrentMethod.CallSite, PEFileKinds.ConsoleApplication);
                var cb = new CustomAttributeBuilder(typeof(STAThreadAttribute).GetConstructor(new Type[] { }), new object[] { });
                mb.SetCustomAttribute(cb);
            }

            for (short i = 0; i < pDefinition.Parameters.Length; i++)
            {
                CreateParameter(i, pDefinition.Parameters[i].Name, pDefinition.Parameters[i].Type, pDefinition.Parameters[i].IsRef);
            }
            return CurrentMethod.CallSite;
        }

        public Type EmitStruct(SmallType pDefinition)
        {
            Type t = null;
            var ta = TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout;
            if (pDefinition.Exported) ta |= TypeAttributes.Public;

            var tb = _builder.DefineType(pDefinition.Name, ta, typeof(ValueType));
            GenericTypeParameterBuilder[] typeParms = null;
            if (pDefinition.IsGenericDefinition)
            {
                typeParms = tb.DefineGenericParameters(pDefinition.GenericTypeParameters.Select((pSt) => pSt.Name).ToArray()); //eww
            }

            if (pDefinition.Exported)
            {
                var c = typeof(TypePrefixAttribute).GetConstructor(new Type[] { typeof(string) });
                var cb = new CustomAttributeBuilder(c, new object[] { pDefinition.Name });
                tb.SetCustomAttribute(cb);
            }

            foreach (var f in pDefinition.GetFields())
            {
                var found = false;
                if(typeParms != null)
                {
                    foreach (var tp in typeParms)
                    {
                        foreach(var a in f.Type.GenericTypeParameters)
                        {
                            if(tp.Name == a.Name)
                            {
                                Type tt = tp;
                                if (f.Type.IsArray) tt = tt.MakeArrayType();
                                f.Info = tb.DefineField(f.Name, tt, FieldAttributes.Public);
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if(!found)
                    f.Info = tb.DefineField(f.Name, f.Type.ToSystemType(), FieldAttributes.Public);
            }

            if(pDefinition.HasInitializer)
            {
                var mb = tb.DefineMethod("Initialize", MethodAttributes.HideBySig | MethodAttributes.Public, null, null);
                pDefinition.Initializer.SetBuilder(mb);
            }

            //t = tb.CreateType();

            pDefinition.SetSystemType(tb);
            return t;
        }

        public void SetCurrentMethod(MethodDefinition pDefinition)
        {
            CurrentMethod = pDefinition;
        }

        private void CreateParameter(short pPosition, string pName, SmallType pType, bool pIsRef)
        {
            CurrentMethod.CreateParameter(pName, pPosition, pType, pIsRef);
        }

        public ParameterBuilder GetParameter(string pName, out short pIndex)
        {
            return CurrentMethod.GetParameter(pName, out pIndex);
        }
        #endregion
    }
}
