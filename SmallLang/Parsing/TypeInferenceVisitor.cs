using System;
using System.Linq;
using System.Collections.Generic;
using SmallLang.Syntax;
using SmallLang.Emitting;

namespace SmallLang.Parsing
{
    class TypeInferenceVisitor : SyntaxVisitor
    {
        public override void Visit(ImplicitCastExpressionSyntax pNode)
        {
            var t = InferImplicitCastType(pNode.Parent, pNode);
            pNode.SetType(t);

            base.Visit(pNode);
            if(pNode.Value.Type != SmallType.Undefined && pNode.Type != SmallType.Undefined)
            {
                if (!MetadataCache.CastExists(pNode.Value.Type, pNode.Type))
                    Compiler.ReportError(CompilerErrorType.CastNotDefined, pNode, pNode.Value.Type.ToString(), pNode.Type.ToString());
            }
        }

        public override void Visit(ExplicitCastExpressionSyntax pNode)
        {
            base.Visit(pNode);
            if(pNode.Value.Type != SmallType.Undefined && pNode.Type != SmallType.Undefined)
            {
                if (!MetadataCache.CastExists(pNode.Value.Type, pNode.Type))
                    Compiler.ReportError(CompilerErrorType.CastNotDefined, pNode, pNode.Value.Type.ToString(), pNode.Type.ToString());
            }
        }

        private SmallType InferImplicitCastType(SyntaxNode pNode, SyntaxNode pChild)
        {
            switch(pNode)
            {
                case FunctionInvocationSyntax f:
                    var def = MetadataCache.FindBestOverload(f, out bool e);
                    int i = 0;
                    foreach(var a in f.Arguments)
                    {
                        if (a.Value == pChild) return def.Parameters[i].Type;
                        i++;
                    }
                    return SmallType.Undefined;

                case ArgumentExpressionSyntax a:
                    return InferImplicitCastType(a.Parent, pChild);

                case ConcatenateExpressionSyntax _:
                    return SmallType.String;

                case ExponentExpressionSyntax _:
                    return SmallType.Double;

                case AndExpressionSyntax _:
                case OrExpressionSyntax _:
                    return SmallType.Boolean;

                case AssignmentStatementSyntax a:
                    return a.Identifier.Type;

                case DeclarationStatementSyntax d:
                    return d.Identifier.Type;

                case BinaryExpressionSyntax b:
                    if (b.Left == pChild) return b.Right.Type;
                    if (b.Right == pChild) return b.Left.Type;
                    Compiler.ReportError(CompilerErrorType.CannotInferType, pChild);
                    return SmallType.Undefined;

                case ArrayAccessExpressionSyntax _:
                    return InferImplicitCastType(pNode.Parent, pNode);

                case UnaryExpressionSyntax u:
                    return u.Type;

                case CatAssignmentExpressionSyntax c:
                    return SmallType.String;

                default:
                    throw new Exception();
            }
        }

        public override void Visit(FunctionInvocationSyntax pNode)
        {
            var d = MetadataCache.FindBestOverload(pNode, out bool e);
            if (d != null) pNode.SetType(d.GetReturnType());
            base.Visit(pNode);
        }

        public override void Visit(DeclarationStatementSyntax pNode)
        {
            base.Visit(pNode);
            //Tuple types will have been rewritten by GroupAssignmentSyntaxRewriter
            if (pNode.Value.Type.IsTupleType)
                pNode.Identifier.SetType(pNode.Value.Type);

            if (pNode.Value.GetType() == typeof(ArrayLiteralSyntax))
                ((ArrayLiteralSyntax)pNode.Value).SetType(pNode.Identifier.Type);
        }

        public override void Visit(AssignmentStatementSyntax pNode)
        {
            base.Visit(pNode);

            if (pNode.Value.GetType() == typeof(ArrayLiteralSyntax))
                ((ArrayLiteralSyntax)pNode.Value).SetType(pNode.Identifier.Type);
        }

        public override void Visit(MemberAccessSyntax pNode)
        {
            base.Visit(pNode);
            if (pNode.Name.Type.IsTupleType)
            {
                var v = pNode.Value;
                var t = pNode.Name.Type.ToSystemType();
                var f = t.GetField(v.Value);
                v.SetType(SmallType.FromSystemType(f.FieldType));
            }
        }

        public override void Visit(StructSyntax pNode)
        {
            base.Visit(pNode);
        }

        public override void Visit(NumericLiteralSyntax pNode)
        {
            InferNumericType(pNode.Parent, pNode);
            base.Visit(pNode);
        }

        private void InferNumericType(SyntaxNode pNode, NumericLiteralSyntax pChild)
        {
            switch (pNode)
            {
                case FunctionInvocationSyntax f:
                    var def = MetadataCache.FindBestOverload(f, out bool e);
                    for(int i = 0; i < f.Arguments.Count; i++)
                    {
                        if (f.Arguments[i].Value == pChild)
                        {
                            if (IsNumericType(def.Parameters[i].Type)) ChangeNumber(pChild, def.Parameters[i].Type);
                        }
                    }
                    break;

                case ArgumentExpressionSyntax a:
                    InferNumericType(a.Parent, pChild);
                    break;

                case ExponentExpressionSyntax _:
                    pChild.NumberType = NumberType.Double;
                    break;

                case AssignmentStatementSyntax a:
                    if (IsNumericType(a.Identifier.Type)) ChangeNumber(pChild, a.Identifier.Type);
                    break;

                case DeclarationStatementSyntax d:
                    if (IsNumericType(d.Identifier.Type)) ChangeNumber(pChild, d.Identifier.Type);
                    break;

                case BinaryExpressionSyntax b:
                    if (b.Left == pChild && IsNumericType(b.Right.Type)) ChangeNumber(pChild, b.Right.Type);
                    if (b.Right == pChild && IsNumericType(b.Left.Type)) ChangeNumber(pChild, b.Left.Type);
                    break;

                case AddAssignmentExpressionSyntax a:
                    if (a.Name == pChild && IsNumericType(a.Value.Type)) ChangeNumber(pChild, a.Value.Type);
                    if (a.Value == pChild && IsNumericType(a.Name.Type)) ChangeNumber(pChild, a.Name.Type);
                    break;

                case CatAssignmentExpressionSyntax c:
                    if (c.Name == pChild && IsNumericType(c.Value.Type)) ChangeNumber(pChild, c.Value.Type);
                    if (c.Value == pChild && IsNumericType(c.Name.Type)) ChangeNumber(pChild, c.Name.Type);
                    break;

                case DivAssignmentExpressionSyntax d:
                    if (d.Name == pChild && IsNumericType(d.Value.Type)) ChangeNumber(pChild, d.Value.Type);
                    if (d.Value == pChild && IsNumericType(d.Name.Type)) ChangeNumber(pChild, d.Name.Type);
                    break;

                case MulAssignmentExpressionSyntax m:
                    if (m.Name == pChild && IsNumericType(m.Value.Type)) ChangeNumber(pChild, m.Value.Type);
                    if (m.Value == pChild && IsNumericType(m.Name.Type)) ChangeNumber(pChild, m.Name.Type);
                    break;

                case SubAssignmentExpressionSyntax s:
                    if (s.Name == pChild && IsNumericType(s.Value.Type)) ChangeNumber(pChild, s.Value.Type);
                    if (s.Value == pChild && IsNumericType(s.Name.Type)) ChangeNumber(pChild, s.Name.Type);
                    break;

                case ConstSyntax c:
                    ChangeNumber(pChild, c.Value.Type);
                    break;

                case UnaryExpressionSyntax u:
                    break;

                case ReturnSyntax r:
                    break;

                case ArrayAccessExpressionSyntax _:
                    ChangeNumber(pChild, SmallType.I32);
                    break;

                case ArrayLiteralSyntax _:
                    ChangeNumber(pChild, SmallType.I32);
                    break;

                default:
                    throw new Exception();
            }
        }

        private bool IsNumericType(SmallType pType)
        {
            return pType == SmallType.Double || 
                   pType == SmallType.Float || 
                   pType == SmallType.I16 || 
                   pType == SmallType.I32 || 
                   pType == SmallType.I64;
        }

        private void ChangeNumber(NumericLiteralSyntax pNode, SmallType pType)
        {
            if (pType == SmallType.Double) pNode.NumberType = NumberType.Double;
            if (pType == SmallType.Float) pNode.NumberType = NumberType.Float;
            if (pType == SmallType.I16) pNode.NumberType = NumberType.I16;
            if (pType == SmallType.I32) pNode.NumberType = NumberType.I32;
            if (pType == SmallType.I64) pNode.NumberType = NumberType.I64;
        }
    }
}
