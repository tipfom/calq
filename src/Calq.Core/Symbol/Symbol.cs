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
            if (a.SType != b.SType) return false;

            switch (a.SType)
            {
                case SymbolType.Const: return (Constant)a == (Constant)b;
                case SymbolType.Real: return (Real)a == (Real)b;
                case SymbolType.Variable: return (Variable)a == (Variable)b;
            }

            return false;
        }
        public static bool operator !=(Symbol a, Symbol b)
        {
            return !(a == b);
        }


        public override string ToPrefix()
        {
            return ToString();
        }
    }
}
