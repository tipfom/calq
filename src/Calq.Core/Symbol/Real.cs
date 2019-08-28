using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Real : Symbol
    {
        public readonly double Value;

        public Real(double value) : base(SymbolType.Real, false, false)
        {
            Value = value;
        }
        public Real(double value, bool isAddInverse, bool isMultInverse) : base(SymbolType.Real, isAddInverse, isMultInverse)
        {
            Value = value;
        }

        public static bool operator ==(Real a, Real b)
        {
            return a.Value == b.Value;
        }
        public static bool operator !=(Real a, Real b)
        {
            return !(a == b);
        }

        public static explicit operator Real(double v)
        {
            return new Real(v);
        }
        public override Term Clone()
        {
            return new Real(Value, IsAddInverse, IsMulInverse);
        }
        public override Term GetDerivative(string argument)
        {
            return new Real(0);
        }

        public override Term Evaluate()
        {
            return this;
        }
        public override Term Approximate()
        {
            return this;
        }

        public override HashSet<string> GetVariableNames()
        {
            return new HashSet<string>();
        }

        public override string ToLaTeX()
        {
            return GetSign() + Value.ToString();
        }
        public override string ToInfix()
        {
            return GetSign() + Value.ToString();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Term CheckAddReduce(Term t)
        {
            if (t.GetType() == typeof(Real))
            {
                return new Real((IsAddInverse ? -1 : 1) * Value + (t.IsAddInverse ? -1 : 1) * ((Real)t).Value);
            }
            return null;
        }
    }
}
