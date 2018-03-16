using System;
using System.Collections.Generic;
using System.Text;
using SmallLang.Emitting;
using System.Reflection.Emit;

namespace SmallLang.Syntax
{
    public class DateLiteralSyntax : IdentifierSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.DateLiteral;
        public override SmallType Type => SmallType.Date;

        readonly int _day;
        readonly int _month;
        readonly int _year;
        public DateLiteralSyntax(string pValue) : base(pValue)
        {
            var parts = Value.Split(',');
            _day = int.Parse(parts[0]);
            _month = int.Parse(parts[1]);
            _year = int.Parse(parts[2]);
        }

        public override void Emit(ILRunner pRunner)
        {
            var constructor = typeof(DateTime).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int) });
            pRunner.EmitInt(_year);
            pRunner.EmitInt(_month);
            pRunner.EmitInt(_day);
            pRunner.Emitter.Emit(OpCodes.Call, constructor);
        }
    }
}
