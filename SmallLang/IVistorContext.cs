using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang
{
    public interface IVistorContext
    {
        void AddValue(string pKey, object pValue);
        T GetValue<T>(string pKey, T pDefault);
        void SetValue(string pKey, object pValue);
        void RemoveValue(string pKey);
    }

    public class ContextValue : IDisposable
    {
        readonly IVistorContext _visitor;
        readonly string _key;
        public ContextValue(IVistorContext pVisitor, string pKey, object pValue)
        {
            _visitor = pVisitor;
            _key = pKey;
            _visitor.AddValue(_key, pValue);
        }

        public void Dispose()
        {
            _visitor.RemoveValue(_key);
        }
    }
}
