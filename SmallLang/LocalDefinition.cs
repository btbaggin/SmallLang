using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using SmallLang.Emitting;
using SmallLang.Syntax;

namespace SmallLang
{
    public class LocalDefinition
    {
        public bool Parameter { get; private set; }
        public bool Field { get; private set; }
        public bool IsAddress { get; private set; }
        public SmallType Type { get; internal set; }
        public string Value { get; private set; }
        bool _created;
        LocalBuilder _lb = null;
        SmallType _structType;

        private LocalDefinition(bool pParm, bool pIsAddress, bool pField, string pValue, SmallType pType, SmallType pStructType)
        {
            Parameter = pParm;
            IsAddress = pIsAddress;
            Field = pField;
            Value = pValue;
            Type = pType;
            _structType = pStructType;
         }

        public static LocalDefinition Create(SyntaxNode pNode, string pValue, SmallType pType)
        {
            return new LocalDefinition(false, false, false, pValue, pType, null);
        }

        public static LocalDefinition CreateAsParameter(SyntaxNode pNode, bool pIsAddress, string pValue, SmallType pType)
        {
            return new LocalDefinition(true, pIsAddress, false, pValue, pType, null);
        }

        public static LocalDefinition CreateAsField(SyntaxNode pNode, string pValue, SmallType pType, SmallType pStructType)
        {
            return new LocalDefinition(false, false, true, pValue, pType, pStructType);
        }

        public void Emit(ILRunner pRunner, IdentifierSyntax pNode)
        {
            if (Parameter)
            {
                var pb = pRunner.GetParameter(Value, out short i);
                if (pb == null) throw new InvalidOperationException("Invalid var");

                if (pNode.LoadAddress && !IsAddress) pRunner.Emitter.Emit(OpCodes.Ldarga_S, i);
                else pRunner.Emitter.Emit(OpCodes.Ldarg, i);
            }
            else if(Field)
            {
                var fd = Type.GetField(Value);
                if (fd == null) throw new InvalidOperationException("Invalid var");

                pRunner.Emitter.Emit(OpCodes.Ldarg_0);
                pRunner.Emitter.Emit(OpCodes.Ldfld, fd.Type.ToSystemType());
            }
            else
            {
                if (!_created)
                {
                    _lb = pRunner.CreateLocal(Value, Type);
                    _created = true;
                }
                if (_lb == null) throw new InvalidOperationException("Invalid var");

                if (pNode.LoadAddress) pRunner.Emitter.Emit(OpCodes.Ldloca_S, _lb);
                else pRunner.Emitter.Emit(OpCodes.Ldloc, _lb);
            }
        }

        public LocalBuilder GetLocal(ILRunner pRunner)
        {
            if (!_created)
            {
                _lb = pRunner.CreateLocal(Value, Type);
                _created = true;
            }
            return _lb;
        }

        public short GetParameter(ILRunner pRunner)
        {
            pRunner.GetParameter(Value, out short i);
            return i;
        }

        public System.Reflection.FieldInfo GetField()
        {
            return _structType.GetField(Value).Info;
        }
    }
}
