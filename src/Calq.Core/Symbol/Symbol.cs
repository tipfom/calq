using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public abstract class Symbol : Term
    {
        public enum VarType
        {
            Const,
            Real,
            Variable
        }

        public VarType Type;

        protected Symbol(VarType type) : base(TermType.Symbol)
        {
            Type = type;
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
