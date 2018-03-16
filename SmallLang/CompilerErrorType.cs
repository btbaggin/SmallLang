using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang
{
    internal enum CompilerErrorType
    {
        TypeMismatch,
        UndefinedType,
        NoReturnValues,
        IncorrectReturnCount,
        CannotInferType,
        PathNoReturnValue,
        InvalidSyntax,
        EndOfStatementExpected,
        NoRun,
        DuplicateRun,
        InvalidRun,
        InvalidExternalAnnotation,
        ExportExternal,
        InvalidImport,
        DuplicateImportAlias,
        MethodNotDefined,
        MethodNotExported,
        NoMethodOverload,
        CastNotDefined,
        LocalNotDefined,
        DuplicateLocal,
        StructNotDefined,
        StructInvalidMember,
        StructDuplicateMember,
        StructNoType,
        UnknownType,
        ExpressionExpected,
        NoSpecifiedType,
        InvalidDefaultInitialize,
        InvalidArraySpecifier,
        ArrayLiteralBracket,
        UnexpectedToken,
        ExpectingToken,
        GenericArgs,
        InvalidPath,
        InvalidExternalMethod
    }
}
