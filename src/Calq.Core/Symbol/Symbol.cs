using System;

namespace Calq.Core
{
    public abstract class Symbol : Term, IComparable<Symbol>
    {
        public enum SymbolType
        {
            Const,
            Real,
            Variable
        }

        public SymbolType SType;

        protected Symbol(SymbolType type, bool isAddInverse, bool isMultInverse) : base(TermType.Symbol, isAddInverse, isMultInverse)
        {
            SType = type;
        }

        public static Symbol FromString(string s)
        {
            const string numbers = "1234567890";
            if (numbers.Contains(s[0].ToString()))
                return new Real(double.Parse(s));

            Symbol ret = Constant.FromName(s);

            if (ret is null)
                return new Variable(s);

            return ret;
        }

        public static bool operator ==(Symbol a, Symbol b)
        {
            if (a is null || b is null)
                return a is null && b is null;

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
            return ToInfix();
        }
        public override string ToString()
        {
            return base.ToString();
        }

        public override Term MergeBranches()
        {
            return this;
        }

        public int CompareTo(Symbol other)
        {
            if (SType == other.SType)
            {
                switch (SType)
                {
                    case SymbolType.Real:
                        return ((Real)this).CompareTo((Real)other);

                    case SymbolType.Const:
                        return ((Constant)this).CompareTo((Constant)other);
 
                    case SymbolType.Variable:
                        return ((Variable)this).CompareTo((Variable)other);
                }
            }

            return SType.CompareTo(other.SType);
        }
    }
}
