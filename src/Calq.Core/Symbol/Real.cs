using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Real : Symbol
    {
        public readonly double Value;

        public Real(double value) : base(VarType.Real)
        {
            Value = value;
        }

        public static explicit operator Real(double v)
        {
            return new Real(v);
        }

        public override Term Differentiate(string argument)
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
            return Value.ToString();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
