using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Logarithm : Function
    {
        public Logarithm(params Term[] p) : base(FuncType.Log, p)
        {
        }

        public override Term Approximate()
        {
            throw new NotImplementedException();
        }

        public override Term Differentiate(string argument)
        {
            if (Parameters.Length == 1)
            {
                return Parameters[0].Differentiate(argument) / Parameters[0];
            }
            else
            {
                return (new Logarithm(Parameters[0]) / new Logarithm(Parameters[1])).Differentiate(argument);
            }
        }

        public override Term Evaluate()
        {
            return this;
        }

        public override string ToLaTeX()
        {
            if (Parameters.Length == 1) return $@"\log{"{" + Parameters[0].ToLaTeX() + "}"}";
            return $@"\log_{"{" + Parameters[1].ToLaTeX() + "}"} {Parameters[0].ToLaTeX()}";
        }

        public override string ToPrefix()
        {
            return "ln[" + Parameters[0] + "]";
        }

        public override string ToString()
        {
            return ToPrefix();
        }

        public override Term Reduce()
        {
            return this;
        }
    }
}
