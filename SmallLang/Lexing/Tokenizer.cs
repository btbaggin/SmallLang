using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang
{
    public class Tokenizer
    {
        readonly string mstrString;
        readonly Stack<Tuple<int, int, int>> mstkSnapshot;

        public char Current
        {
            get
            {
                if (EOF) return (char)0;
                return mstrString[Index];
            }
        }

        public int Line { get; private set; }

        public int Column { get; private set; }

        public int Index { get; private set; }

        public bool EOF
        {
            get { return Index >= mstrString.Length; }
        }

        public Tokenizer(string pstrString)
        {
            Line = 1;
            Column = 1;
            Index = 0;
            mstrString = pstrString;
            mstkSnapshot = new Stack<Tuple<Int32, Int32, Int32>>();
        }

        public void Eat()
        {
            if (Current == '\n')
            {
                Line++;
                Column = 1;
            }
            else Column++;

            Index++;
        }

        public char Peek(int plngLookahead)
        {
            if (Index + plngLookahead >= mstrString.Length) return (char)0;
            return mstrString[Index + plngLookahead];
        }

        public void BeginTransaction()
        {
            mstkSnapshot.Push(Tuple.Create(Index, Line, Column));
        }

        public void RollbackTransaction()
        {
            Tuple<int, int, int> t = mstkSnapshot.Pop();
            Index = t.Item1;
            Line = t.Item2;
            Column = t.Item3;
        }

        public void CommitTransaction()
        {
            mstkSnapshot.Pop();
        }
    }
}
