using System;
using System.Linq;
using System.Collections.Generic;
using SmallLang.Syntax;
using System.Reflection;

namespace SmallLang.Parsing
{
    class DefinitionVisitor : SyntaxVisitor
    {
        bool _mainFound;
        readonly bool _mainRequired;
        readonly string _path;
        readonly HashSet<string> _importPath;
        public DefinitionVisitor(CompilationOptions pOptions)
        {
            _mainFound = false;
            _mainRequired = pOptions.OutputType == CompilationOutputType.Exe;
            _path = pOptions.OutputPath;
            _importPath = new HashSet<string>();
        }

        public override void Visit(WorkspaceSyntax pNode)
        {
            foreach (var i in pNode.Imports)
            {
                i.Accept(this);
            }

            //We need to register all types since structs could have fields of other types
            foreach (var s in pNode.Structs)
            {
                var st = SmallType.RegisterType("", s.Prefix, true, s.TypeArgs);
                if(s.Initializer != null)
                {
                    st.SetInitializer(new InitializerInfo(s.Initializer));
                }
            }
            foreach (var s in pNode.Structs)
            {
                s.Accept(this);
            }

            foreach (var m in pNode.Methods)
            {
                m.Accept(this);
            }

            foreach (var c in pNode.Casts)
            {
                c.Accept(this);
            }
            if (!_mainFound && _mainRequired) Compiler.ReportError(CompilerErrorType.NoRun, new TextSpan());
            MetadataCache.AddImplicitCasts();
        }

        public override void Visit(CastSyntax pNode)
        {
            //Infer parameter types now that structs have been defined
            pNode.Parameter.SetType(SmallType.FromString(pNode.Parameter.Namespace, pNode.Parameter.Value));
            pNode.ReturnValue.SetType(SmallType.FromString(pNode.ReturnValue.Namespace, pNode.ReturnValue.Value));

            var d = MetadataCache.AddCast(pNode.Parameter.Type, pNode.Parameter.Value, pNode.ReturnValue.Type);
            d.SetScope(Scope.Public);
            pNode.SetDefinition(d);
        }

        public override void Visit(MethodSyntax pNode)
        {
            //Create any generic type parameters to the function
            Dictionary<string, SmallType> typeArgs = new Dictionary<string, SmallType>();
            foreach(var t in pNode.TypeHints)
            {
                typeArgs.Add(t, SmallType.CreateGenericParameter(t));
            }

            //
            //Create types for method
            //

            //Infer parameter types now that structs have been defined
            foreach (var p in pNode.Parameters)
            {
                var st = SmallType.FromString(p.Namespace, p.Value);
                if (p.TypeParameters.Count > 0)
                {
                    SmallType[] types = new SmallType[p.TypeParameters.Count];
                    for (int i = 0; i < types.Length; i++)
                    {
                        //If the type parameter is one on the method definition, use that type
                        if(typeArgs.ContainsKey(p.TypeParameters[i]))
                        {
                            types[i] = typeArgs[p.TypeParameters[i]];
                        }
                        else
                        {
                            types[i] = SmallType.FromString("", p.TypeParameters[i]);
                        }
                    }

                    if (!p.Type.IsVariant)
                    {
                        st = st.MakeGenericType(types);
                    }
                    else
                    {
                        //vnt types are transformed to the actual type parameter
                        if (p.TypeParameters.Count > 1)
                        {
                            SmallType[] typeParameters = new SmallType[p.TypeParameters.Count];
                            for(int i = 0; i < typeParameters.Length; i++)
                            {
                                typeParameters[i] = SmallType.CreateGenericParameter(p.TypeParameters[i]);
                            }
                            st = SmallType.CreateTupleOf(typeParameters);
                        }
                        else
                        {
                            st = SmallType.CreateGenericParameter(p.TypeParameters[0]);
                        }
                    }
                    
                }
                p.SetType(st);
            }

            foreach (var r in pNode.ReturnValues)
            {
                //If the type parameter is one on the method definition, use that type
                if (typeArgs.ContainsKey(r.Value))
                {
                    r.SetType(SmallType.CreateGenericParameter(r.Value));
                }
                else
                {
                    r.SetType(SmallType.FromString(r.Namespace, r.Value));
                }
            }

            //
            //Create method definition
            //

            //Create parameters
            MethodDefinition.Parameter[] parameters = new MethodDefinition.Parameter[pNode.Parameters.Count];
            for (int i = 0; i < pNode.Parameters.Count; i++)
            {
                parameters[i] = new MethodDefinition.Parameter(pNode.Parameters[i]);
            }

            //Create return types
            SmallType[] returnTypes = new SmallType[pNode.ReturnValues.Count];
            for (int i = 0; i < pNode.ReturnValues.Count; i++)
            {
                returnTypes[i] = pNode.ReturnValues[i].Type;
            }

            var d = MetadataCache.AddMethod(pNode.Name, parameters, returnTypes, typeArgs.Values.ToList());
            if (pNode.Annotations.Count > 0)
            {
                //Process any standard annotations
                //run
                //export
                //external info
                foreach (var a in pNode.Annotations)
                {
                    if (a.Value.Equals("run", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_mainFound)
                        {
                            Compiler.ReportError(CompilerErrorType.DuplicateRun, pNode);
                        }
                        else
                        {
                            d.IsMain = true;
                        }

                        if (pNode.Parameters.Count > 0 || pNode.ReturnValues.Count > 0)
                        {
                            Compiler.ReportError(CompilerErrorType.InvalidRun, pNode);
                        }
                        _mainFound = true;
                    }
                    else if (a.Value.Equals("export", StringComparison.OrdinalIgnoreCase))
                    {
                        d.SetScope(Scope.Public);
                    }
                    else if (pNode.External)
                    {
                        var s = a.Value.Split(';');
                        if (s.Length != 3) Compiler.ReportError(CompilerErrorType.InvalidExternalAnnotation, pNode);

                        d.SetExternInfo(s[0], s[1], s[2]);
                    }
                }

                if (d.ExternMethod != null && d.Scope == Scope.Public)
                {
                    Compiler.ReportError(CompilerErrorType.ExportExternal, pNode, pNode.Name);
                }
            }
            pNode.SetDefinition(d);
        }

        public override void Visit(StructSyntax pNode)
        {
            var s = SmallType.FromString("", pNode.Prefix); 
            //Determine types of all fields
            foreach (var f in pNode.Fields)
            {
                if (s.FieldExists(f.Value))
                {
                    Compiler.ReportError(CompilerErrorType.StructDuplicateMember, pNode, f.Value, pNode.Name);
                }
                else
                {
                    var fld = SmallType.FromString(f.Namespace, f.Value);
                    s.AddField(f.Value, f.TypeParameters, fld);
                }
            }

            SmallType.RegisterType(s, null);

            //Process standard annotations
            //export
            foreach (var a in pNode.Annotations)
            {
                if (a.Value.Equals("export", StringComparison.OrdinalIgnoreCase))
                {
                    s.Exported = true;
                }
            }
            pNode.SetType(s);
        }

        public override void Visit(ImportSyntax pNode)
        {
            if (!_importPath.Contains(pNode.Path))
            {
                _importPath.Add(pNode.Path);
                var path = System.IO.Path.GetFullPath(System.IO.Path.Combine(_path, pNode.Path + ".dll"));

                Assembly asm = null;
                try
                {
                    asm = Assembly.LoadFile(path);
                }
                catch (Exception)
                {
                    Compiler.ReportError(CompilerErrorType.InvalidImport, pNode, pNode.Path);
                    return;
                }

                try
                {
                    System.IO.File.Copy(path, System.IO.Path.Combine(_path, System.IO.Path.GetFileName(path)), true);
                }
                catch (Exception)
                {
                    Compiler.ReportError("Unable to copy import to local directory");
                }

                //Import all types from the import
                //TODO move this to struct definition thing?
                foreach (var t in asm.GetTypes())
                {
                    if (t.IsValueType && t.IsPublic)
                    {
                        string prefix = "";
                        var attr = t.GetCustomAttributes<Emitting.TypePrefixAttribute>().SingleOrDefault();

                        //All exported types must have a TypePrefixAttribute in order to be imported
                        if (attr != null)
                        {
                            //
                            //Create type
                            //
                            prefix = attr.Prefix;

                            //Get any generic type parameters of the type
                            List<string> typeParameters = new List<string>();
                            if (t.IsGenericType)
                            {
                                var ga = t.GetGenericArguments();
                                for (int i = 0; i < ga.Length; i++)
                                {
                                    typeParameters.Add(ga[i].Name);
                                }
                            }
                            var st = SmallType.RegisterType(pNode.Alias, prefix, true, typeParameters);
                            st.SetSystemType(t);

                            foreach (var f in t.GetFields())
                            {
                                SmallType fieldType = null;
                                if (f.FieldType.IsGenericParameter)
                                {
                                    //If it's a generic type parameter, use the generic type
                                    foreach (var s in st.GenericTypeParameters)
                                    {
                                        if (s.Name.Equals(f.FieldType.Name, StringComparison.OrdinalIgnoreCase))
                                        {
                                            fieldType = s;
                                        }
                                    }
                                }
                                else
                                {
                                    fieldType = SmallType.FromSystemType(f.FieldType);
                                }

                                typeParameters.Clear();

                                //Get field type parameters
                                if (f.FieldType.ContainsGenericParameters)
                                {
                                    var typeArguments = t.GetGenericArguments();
                                    foreach (var tp in typeArguments)
                                    {
                                        if (tp.IsGenericParameter)
                                        {
                                            typeParameters.Add(tp.Name);
                                        }
                                    }
                                }

                                st.AddField(f.Name, typeParameters, fieldType);
                            }

                            //Check if we have an initializer function
                            var m = t.GetMethod("Initialize");
                            if (m != null)
                            {
                                var ii = new InitializerInfo(null);
                                ii.SetMethod(m);
                                st.SetInitializer(ii);
                            }
                        }
                    }
                }

                foreach (var t in asm.GetTypes())
                {
                    foreach (var m in t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                    {
                        var p = m.GetParameters();
                        MethodDefinition.Parameter[] parameters = new MethodDefinition.Parameter[p.Length];
                        for (int i = 0; i < p.Length; i++)
                        {
                            parameters[i] = new MethodDefinition.Parameter(SmallType.FromSystemType(p[i].ParameterType), p[i].Name, p[i].ParameterType.IsByRef);
                        }

                        SmallType[] returnType;
                        if (m.ReturnType.IsConstructedGenericType)
                        {
                            var types = m.ReturnType.GetGenericArguments();
                            returnType = new SmallType[types.Length];
                            for (int i = 0; i < types.Length; i++)
                            {
                                returnType[i] = SmallType.FromSystemType(types[i]);
                            }
                        }
                        else if (m.ReturnType.IsGenericParameter)
                        {
                            returnType = new SmallType[] { SmallType.CreateGenericParameter(m.ReturnType.Name) };
                        }
                        else if (m.ReturnType != typeof(void)) returnType = new SmallType[] { SmallType.FromSystemType(m.ReturnType) };
                        else returnType = new SmallType[0];

                        List<SmallType> typeHints = new List<SmallType>();
                        if (m.IsGenericMethodDefinition)
                        {
                            var typeParameters = m.GetGenericArguments();
                            foreach (var tp in typeParameters)
                            {
                                if (tp.IsGenericParameter) typeHints.Add(SmallType.CreateGenericParameter(tp.Name));
                            }
                        }

                        MethodDefinition md = null;
                        if (returnType.Length == 1 && parameters.Length == 1 &&
                           m.Name == MetadataCache.CastFunction(parameters[0].Type, returnType[0]))
                        {
                            md = MetadataCache.AddImportedCast(pNode.Alias, parameters[0].Type, parameters[0].Name, returnType[0]);
                        }
                        else
                        {
                            md = MetadataCache.AddImportedMethod(pNode.Alias, m.Name, parameters, returnType, typeHints);
                        }
                        md.SetExternMethod(m);
                    }
                }
            }

            if (MetadataCache.ImportedNamespaces().Contains(pNode.Alias)) Compiler.ReportError(CompilerErrorType.DuplicateImportAlias, pNode, pNode.Alias);
            else MetadataCache.AddNamespace(pNode.Alias);
        }
    }
}
