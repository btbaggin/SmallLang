using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang
{
    class BufferedTokenStream : ITokenStream
    {

        public Token Current { get => _tokens[Index]; }

        public bool EOF { get => Index >= _tokens.Count; }

        public int SourceIndex { get => mSource.Index; }

        public int SourceLine { get; private set; }

        public int SourceColumn { get; private set; }

        public int Index { get; private set; }

        readonly ITokenSource mSource;
        readonly List<Token> _tokens;
        Stack<int> mstkMarkers;

        public BufferedTokenStream(ITokenSource pSource)
        {
            mSource = pSource;
            mstkMarkers = new Stack<int>();
            _tokens = new List<Token>();
            Index = 0;
            Fetch(1);
        }

        public bool MoveNext()
        {
            SourceLine = mSource.Line;
            SourceColumn = mSource.Column;
            Index++;
            Sync(Index);
            return _tokens.Count > Index;
        }

        public void InsertToken(Token pToken)
        {
            _tokens.Insert(Index, pToken);
        }

        public void Reset()
        {
            mstkMarkers = new Stack<int>();
            Index = 0;
        }

        public void BeginTransaction() => mstkMarkers.Push(Index);

        public void RollbackTransaction()
        {
            if (mstkMarkers.Count > 0) Index = mstkMarkers.Pop();
        }

        public bool InTransaction() => mstkMarkers.Count > 0;

        private void Sync(int plngNum)
        {
            int n = plngNum - _tokens.Count + 1;
            if (n > 0) Fetch(n);
        }

        private void Fetch(int plngNum)
        {
            for(int i = 0; i < plngNum; i++)
            {
                if(mSource.GetNextToken(out Token t))
                {
                    _tokens.Add(t);
                }
            }
        }

        public void CommitTransaction()
        {
            if (mstkMarkers.Count > 0) mstkMarkers.Pop();
        }

        public void Seek(int pIndex)
        {
            Sync(pIndex);
            Index = pIndex;
        }
    }
}
