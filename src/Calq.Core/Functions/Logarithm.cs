using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Logarithm : Function
    {
        public Logarithm(params Term[] p) : base(FuncType.Log, false, false, p)
        {
            if (!(p.Length == 1 || p.Length == 2))
                throw new InvalidParameterCountException("Log takes one or two arguments");
        }
        public Logarithm(bool isAddInverse, bool isMulInverse, params Term[] p) : base(FuncType.Log, isAddInverse, isMulInverse, p)
        {
            if (!(p.Length == 1 || p.Length == 2))
                throw new InvalidParameterCountException("Log takes one or two arguments");
        }

        public override Term Approximate()
        {
            throw new NotImplementedException();
        }

        public override Term GetDerivative(string argument)
        {
            if (Parameters.Length == 1)
            {
                return Parameters[0].GetDerivative(argument) / Parameters[0];
            }
            else
            {
                return (new Logarithm(Parameters[0]) / new Logarithm(Parameters[1])).GetDerivative(argument);
            }
        }

        public override Term Evaluate()
        {
            return this;
        }

        public override string ToLaTeX()
        {
            if (Parameters.Length == 1)
                return GetSign() + $@"\log{"(" + Parameters[0].ToLaTeX() + ")"}";
            else
                return GetSign() + $@"\log_{"{" + Parameters[1].ToLaTeX() + "}"} {"(" + Parameters[0].ToLaTeX() + ")"}";
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Term Reduce()
        {
            return new Logarithm(IsAddInverse, IsMulInverse, Parameters.Select(x => x.Reduce()).ToArray());
        }
    }
}
