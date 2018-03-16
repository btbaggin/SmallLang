using System;
using System.Collections.Generic;
using System.Text;

namespace SmallLang
{
    public interface ITokenStream
    {
        int SourceIndex { get; }
        int SourceLine { get; }
        int SourceColumn { get; }

        int Index { get; }
        Token Current { get; }
        bool EOF { get; }

        bool MoveNext();
        void InsertToken(Token pToken);
        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction();
        bool InTransaction();
        void Seek(int pIndex);
        void Reset();
    }
}
