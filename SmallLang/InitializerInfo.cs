using SmallLang.Emitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace SmallLang
{
    public class InitializerInfo
    {
        readonly Syntax.BlockSyntax _block;
        MethodBuilder _builder;
        public MethodInfo Info { get; private set; }
        public InitializerInfo(Syntax.BlockSyntax pBlock)
        {
            _block = pBlock;
        }

        public void SetBuilder(MethodBuilder pBuilder)
        {
            _builder = pBuilder;
            Info = pBuilder;
        }

        public void SetMethod(MethodInfo methodInfo)
        {
            Info = methodInfo;
        }

        public void Emit(ILRunner pRunner)
        {
            pRunner.OverrideGenerator = _builder.GetILGenerator();
            _block.Emit(pRunner);
            pRunner.Emitter.Emit(OpCodes.Ret);
            pRunner.OverrideGenerator = null;
        }
    }
}
