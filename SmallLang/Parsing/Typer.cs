using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallLang.Syntax;

namespace SmallLang.Parsing
{
    class Typer : SyntaxVisitor
    {
        public override void Visit(DeclarationStatementSyntax pNode)
        {
            if (!pNode.Identifier.Type.IsAssignableFrom(pNode.Value.Type))
                Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, pNode.Value.Type.ToString(), pNode.Identifier.Type.ToString());

            if (pNode.Identifier.Type.IsGenericDefinition && !pNode.Identifier.Type.IsGeneric)
                Compiler.ReportError(CompilerErrorType.GenericArgs, pNode.Identifier);

            base.Visit(pNode);
        }

        public override void Visit(AssignmentStatementSyntax pNode)
        {
            if (!pNode.Identifier.Type.IsAssignableFrom(pNode.Value.Type))
                Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, pNode.Value.Type.ToString(), pNode.Identifier.Type.ToString());
            
            base.Visit(pNode);
        }

        public override void Visit(ConcatenateExpressionSyntax pNode)
        {
            //Validate left
            if (pNode.Left.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.String.IsAssignableFrom(pNode.Left.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.String.ToString(), pNode.Left.Type.ToString());
            }

            //Validate right
            if (pNode.Right.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.String.IsAssignableFrom(pNode.Right.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.String.ToString(), pNode.Right.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(CatAssignmentExpressionSyntax pNode)
        {
            //Validate right
            if (pNode.Value.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.String.IsAssignableFrom(pNode.Value.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.String.ToString(), pNode.Value.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(AddAssignmentExpressionSyntax pNode)
        {
            //Validate right
            if (pNode.Name.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!pNode.Name.Type.IsAssignableFrom(pNode.Value.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, pNode.Name.Type.ToString(), pNode.Value.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(AndExpressionSyntax pNode)
        {
            //Validate left
            if (pNode.Left.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Boolean.IsAssignableFrom(pNode.Left.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Boolean.ToString(), pNode.Left.Type.ToString());
            }

            //Validate right
            if (pNode.Right.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Boolean.IsAssignableFrom(pNode.Right.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Boolean.ToString(), pNode.Right.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(OrExpressionSyntax pNode)
        {
            //Validate left
            if (pNode.Left.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Boolean.IsAssignableFrom(pNode.Left.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Boolean.ToString(), pNode.Left.Type.ToString());
            }

            //Validate right
            if (pNode.Right.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Boolean.IsAssignableFrom(pNode.Right.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Boolean.ToString(), pNode.Right.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(EqualsExpressionSyntax pNode)
        {
            if (pNode.Left.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode.Left);
            else if (pNode.Right.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode.Right);
            else
            {
                if (!pNode.Left.Type.IsAssignableFrom(pNode.Right.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, pNode.Right.Type.ToString(), pNode.Left.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(NotEqualsExpressionSyntax pNode)
        {
            if (pNode.Left.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode.Left);
            else if (pNode.Right.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode.Right);
            else
            {
                if (!pNode.Left.Type.IsAssignableFrom(pNode.Right.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, pNode.Right.Type.ToString(), pNode.Left.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(NotExpressionSyntax pNode)
        {
            if (pNode.Value.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Boolean.IsAssignableFrom(pNode.Value.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Boolean.ToString(), pNode.Value.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(WhileSyntax pNode)
        {
            if (pNode.Condition.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Boolean.IsAssignableFrom(pNode.Condition.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Boolean.ToString(), pNode.Condition.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(ArrayLiteralSyntax pNode)
        {
            if(pNode.Size.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.I32.IsAssignableFrom(pNode.Size.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.I32.ToString(), pNode.Size.Type.ToString());
            }
            base.Visit(pNode);
        }

        public override void Visit(ForSyntax pNode)
        {
            if (pNode.Condition.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Boolean.IsAssignableFrom(pNode.Condition.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Boolean.ToString(), pNode.Condition.Type.ToString());
            }
            base.Visit(pNode);
        }

        public override void Visit(IfSyntax pNode)
        {
            if (pNode.Condition.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Boolean.IsAssignableFrom(pNode.Condition.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Boolean.ToString(), pNode.Condition.Type.ToString());
            }
            
            base.Visit(pNode);
        }

        public override void Visit(ExponentExpressionSyntax pNode)
        {
            //Validate left
            if (pNode.Left.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Double.IsAssignableFrom(pNode.Left.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Double.ToString(), pNode.Left.Type.ToString());
            }

            //Validate right
            if (pNode.Right.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode);
            else
            {
                if (!SmallType.Double.IsAssignableFrom(pNode.Right.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, SmallType.Double.ToString(), pNode.Right.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(FunctionInvocationSyntax pNode)
        {
            var md = MetadataCache.FindBestOverload(pNode, out bool exact);
            for(int i = 0; i < md.Parameters.Length; i++)
            {
                if (pNode.Arguments[i].Type == SmallType.Undefined)
                    Compiler.ReportError(CompilerErrorType.UndefinedType, pNode.Arguments[i]);
                else
                {
                    if (!md.Parameters[i].Type.IsAssignableFrom(pNode.Arguments[i].Type))
                        Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode.Arguments[i], md.Parameters[i].Type.ToString(), pNode.Arguments[i].Type.ToString());
                }
            }

            if (!exact) //All functions should be able to be bound exactly at this point
                Compiler.ReportError(CompilerErrorType.NoMethodOverload, pNode, pNode.Value, pNode.Arguments.Count.ToString());

            base.Visit(pNode);
        }

        public override void Visit(AdditionExpressionSyntax pNode)
        {
            if (pNode.Left.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode.Left);
            else if (pNode.Right.Type == SmallType.Undefined)
                Compiler.ReportError(CompilerErrorType.UndefinedType, pNode.Right);
            else
            {
                if (!pNode.Left.Type.IsAssignableFrom(pNode.Right.Type))
                    Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, pNode.Right.Type.ToString(), pNode.Left.Type.ToString());
            }

            base.Visit(pNode);
        }

        public override void Visit(ArrayAccessExpressionSyntax pNode)
        {
            if (!pNode.Identifier.Type.IsArray && pNode.Identifier.Type != SmallType.String)
                Compiler.ReportError("Array access used on non array");
            base.Visit(pNode);
        }

        public override void Visit(LengthQueryExpressionSyntax pNode)
        {
            if (!pNode.Array.Type.IsArray)
                Compiler.ReportError("Length query used on non array");
            base.Visit(pNode);
        }

        IList<ReturnValueSyntax> _returnValues;
        public override void Visit(MethodSyntax pNode)
        {
            _returnValues = pNode.ReturnValues;
            base.Visit(pNode);
        }

        public override void Visit(CastSyntax pNode)
        {
            _returnValues = new List<ReturnValueSyntax>() { pNode.ReturnValue };
            base.Visit(pNode);
        }

        public override void Visit(ReturnSyntax pNode)
        {
            if (_returnValues != null && pNode.Values.Count != _returnValues.Count)
            {
                if (_returnValues.Count == 0) Compiler.ReportError(CompilerErrorType.NoReturnValues, pNode);
                else Compiler.ReportError(CompilerErrorType.IncorrectReturnCount, pNode, _returnValues.Count.ToString());
            }
            else
            {
                for(int i = 0; i < pNode.Values.Count; i++)
                {
                    if(!_returnValues[i].Type.IsAssignableFrom(pNode.Values[i].Type))
                    {
                        Compiler.ReportError(CompilerErrorType.TypeMismatch, pNode, _returnValues[i].Type.ToString(), pNode.Values[i].Type.ToString());
                    }
                }
            }
            base.Visit(pNode);
        }
    }
}
