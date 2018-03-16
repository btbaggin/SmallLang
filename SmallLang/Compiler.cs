using System;
using System.Diagnostics;
using System.Collections.Generic;
using SmallLang.Emitting;
using SmallLang.Parsing;

namespace SmallLang
{
    /* TODO
     * Move to stream text processing
     * Enums
     * 
     * validate constants
     * Optimizations
     * Some sort of template meta programming
     */
    public class Compiler
    {
        static List<SmallError> _errors;
        string _path;
        static Compiler()
        {
            _errors = new List<SmallError>();
        }

        public void Run(string pPath, CompilationOptions pOptions)
        {
            if(!System.IO.Directory.Exists(pOptions.OutputPath))
            {
                ReportError(CompilerErrorType.InvalidPath, new TextSpan(), pOptions.OutputPath);
                PrintAllErrors("");
                return;
            }

            var sw = new Stopwatch();
            sw.Start();
            _path = pPath;
            _errors.Clear();
            MetadataCache.Clear();

            string text = System.IO.File.ReadAllText(pPath);
            var p = Parser.Create(new Lexing.SmallLangDefinition());
            var ilRunner = new ILRunner(pOptions);
            var ws = p.Parse(text);

            if (PrintAllErrors(text)) return;

            ws = ws.Accept<Syntax.WorkspaceSyntax>(new GroupAssignmentSyntaxRewriter());
            ws.Accept(new DefinitionVisitor(pOptions));
            ws = ws.Accept<Syntax.WorkspaceSyntax>(new ReferenceBinderRewriter());

            if (PrintAllErrors(text)) return;

            ws.Accept(new TypeInferenceVisitor());
            ws.Accept(new Typer());
            ws.Accept(new ValidationVisitor());

            if (PrintAllErrors(text)) return;

            ws.Emit(ilRunner);

            sw.Stop();
            Console.WriteLine("Compiled " + _path + " in " + sw.ElapsedMilliseconds.ToString() + "ms");
        }

        private bool PrintAllErrors(string pText)
        {
            if(_errors.Count > 0)
            {
                Console.WriteLine("Errors compiling " + _path);
                foreach (var e in _errors)
                {
                    Console.WriteLine(e.Text + " at line: " + e.Span.Line + " column: " + e.Span.Column);
                    Console.WriteLine(pText.Substring(e.Span.Start, e.Span.Length));
                    Console.WriteLine();
                }
                return true;
            }

            return false;
        }

        internal static void ReportError(string pType)
        {
            _errors.Add(new SmallError(pType, new TextSpan()));
        }

        internal static void ReportError(CompilerErrorType pType, TextSpan pSpan, params string[] pArgs)
        {
            _errors.Add(new SmallError(GetMessage(pType, pArgs), pSpan));
        }

        internal static void ReportError(CompilerErrorType pType, Syntax.SyntaxNode pNode, params string[] pArgs)
        {
            _errors.Add(new SmallError(GetMessage(pType, pArgs), pNode.Span));
        }

        private static string GetMessage(CompilerErrorType pType, params string[] pArgs)
        {
            switch(pType)
            {
                case CompilerErrorType.EndOfStatementExpected:
                    Debug.Assert(pArgs.Length == 0);
                    return "Comma, ')', or valid expression continuation expected.";

                case CompilerErrorType.TypeMismatch:
                    Debug.Assert(pArgs.Length == 2);
                    return "Unable to convert from " + pArgs[0] + " to " + pArgs[1];

                case CompilerErrorType.UndefinedType:
                    Debug.Assert(pArgs.Length == 0);
                    return "Expression does not produce a value";

                case CompilerErrorType.NoReturnValues:
                    Debug.Assert(pArgs.Length == 0);
                    return "Method does not return any values";

                case CompilerErrorType.IncorrectReturnCount:
                    Debug.Assert(pArgs.Length == 1);
                    return "Incorrect number of return values. Expecting " + pArgs[0];

                case CompilerErrorType.CannotInferType:
                    Debug.Assert(pArgs.Length == 0);
                    return "Cannot infer type of cast expression";

                case CompilerErrorType.PathNoReturnValue:
                    Debug.Assert(pArgs.Length == 0);
                    return "Not all paths return a value";

                case CompilerErrorType.InvalidSyntax:
                    Debug.Assert(pArgs.Length == 2);
                    return "Expecting " + pArgs[0] + " token, but encountered " + pArgs[1];

                case CompilerErrorType.NoRun:
                    Debug.Assert(pArgs.Length == 0);
                    return "No function has been marked with @run";

                case CompilerErrorType.DuplicateRun:
                    Debug.Assert(pArgs.Length == 0);
                    return "Only one function can be marked with @run";

                case CompilerErrorType.InvalidRun:
                    Debug.Assert(pArgs.Length == 0);
                    return "@run functions must have 0 parameters and 0 return values";

                case CompilerErrorType.InvalidExternalAnnotation:
                    Debug.Assert(pArgs.Length == 0);
                    return "Incorrectly formatted external annotation. Correct format is Assembly;Class;Function";

                case CompilerErrorType.ExportExternal:
                    Debug.Assert(pArgs.Length == 1);
                    return "External functions cannot be exported " + pArgs[0];

                case CompilerErrorType.InvalidImport:
                    Debug.Assert(pArgs.Length == 1);
                    return "Unable to import " + pArgs[0] + " ensure the path is correct.";

                case CompilerErrorType.DuplicateImportAlias:
                    Debug.Assert(pArgs.Length == 1);
                    return "Duplicate import alias " + pArgs[0];

                case CompilerErrorType.MethodNotDefined:
                    Debug.Assert(pArgs.Length == 2);
                    if (string.IsNullOrEmpty(pArgs[1])) return "Method " + pArgs[0] + " has not been defined in base namespace";
                    else return "Method " + pArgs[0] + " has not been defined in namespace " + pArgs[1];

                case CompilerErrorType.MethodNotExported:
                    Debug.Assert(pArgs.Length == 1);
                    return "Method " + pArgs[0] + " not marked with @export";

                case CompilerErrorType.NoMethodOverload:
                    Debug.Assert(pArgs.Length == 2);
                    return "Found no matching overload for " + pArgs[0] + " takes " + pArgs[1] + " arguments";
                    
                case CompilerErrorType.CastNotDefined:
                    Debug.Assert(pArgs.Length == 2);
                    return "No cast defined for types " + pArgs[0] + " and " + pArgs[1];

                case CompilerErrorType.LocalNotDefined:
                    Debug.Assert(pArgs.Length == 1);
                    return "The name '" + pArgs[0] + "' does not exist in the current context";

                case CompilerErrorType.DuplicateLocal:
                    Debug.Assert(pArgs.Length == 1);
                    return "A variable named '" + pArgs[0] + "' has already been defined in this scope";

                case CompilerErrorType.StructNotDefined:
                    Debug.Assert(pArgs.Length == 2);
                    if (string.IsNullOrEmpty(pArgs[1])) return "Struct " + pArgs[0] + " has not been defined in base namespace";
                    else return "Struct " + pArgs[0] + " has not been defined in namespace " + pArgs[1];

                case CompilerErrorType.StructInvalidMember:
                    Debug.Assert(pArgs.Length == 2);
                    return "Struct " + pArgs[0] + " does not contain member " + pArgs[1];

                case CompilerErrorType.StructDuplicateMember:
                    Debug.Assert(pArgs.Length == 2);
                    return "Struct " + pArgs[0] + " contains duplicate field definitions " + pArgs[1];

                case CompilerErrorType.StructNoType:
                    Debug.Assert(pArgs.Length == 1);
                    return "Struct " + pArgs[0] + " does not define a type prefix";
 
                case CompilerErrorType.UnknownType:
                    Debug.Assert(pArgs.Length == 0);
                    return "Unknown type";

                case CompilerErrorType.ExpressionExpected:
                    Debug.Assert(pArgs.Length == 0);
                    return "Expression expected";

                case CompilerErrorType.NoSpecifiedType:
                    Debug.Assert(pArgs.Length == 0);
                    return "Type must be specified";

                case CompilerErrorType.InvalidDefaultInitialize:
                    Debug.Assert(pArgs.Length == 0);
                    return "Cannot default initiaze type " + pArgs[0];

                case CompilerErrorType.InvalidArraySpecifier:
                    Debug.Assert(pArgs.Length == 0);
                    return "Cannot have array bound specifiers in declaration statements";

                case CompilerErrorType.ArrayLiteralBracket:
                    Debug.Assert(pArgs.Length == 0);
                    return "Array literal missing closing bracket";

                case CompilerErrorType.UnexpectedToken:
                    Debug.Assert(pArgs.Length == 1);
                    return "Unexpected token " + pArgs[0];

                case CompilerErrorType.ExpectingToken:
                    Debug.Assert(pArgs.Length == 1);
                    return "Expecting token " + pArgs[0];

                case CompilerErrorType.GenericArgs:
                    Debug.Assert(pArgs.Length == 0);
                    return "All generic types must specify type arguments";

                case CompilerErrorType.InvalidPath:
                    Debug.Assert(pArgs.Length == 1);
                    return "Path " + pArgs[0] + " does not exist";

                case CompilerErrorType.InvalidExternalMethod:
                    Debug.Assert(pArgs.Length == 1);
                    return "Unable to find external method " + pArgs[0];

                default:
                    throw new Exception("Unknown error type " + pType.ToString());
            }
        }
    }
}
