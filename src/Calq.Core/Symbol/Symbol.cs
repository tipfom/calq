using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public abstract class Symbol : Term
    {
        public enum SymbolType
        {
            Const,
            Real,
            Variable
        }

        public SymbolType SType;

        protected Symbol(SymbolType type) : base(TermType.Symbol)
        {
            SType = type;
        }

        public static bool operator ==(Symbol a, Symbol b)
        {
            if ((object)a == null || (object)a == null)
                return (object)a == null && (object)a == null;

            if (a.SType != b.SType) return false;

            return a.ToString() == b.ToString();
        }

        public static bool operator !=(Symbol a, Symbol b)
        {
            return !(a == b);
        }

        public override Term Reduce()
        {
            return this;
        }


        public override string ToPrefix()
        {
            return ToString();
        }
    }
}
