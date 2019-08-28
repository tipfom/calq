using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Cos : Function
    {
        public Cos(params Term[] p) : base(FuncType.Cos, false, false, p)
        {
            if (p.Length != 1)
                throw new InvalidParameterCountException("Cos takes exactly one argument");
        }
        public Cos(bool isAddInverse, bool isMulInverse, params Term[] p) : base(FuncType.Cos, isAddInverse, isMulInverse, p)
        {
            if (p.Length != 1)
                throw new InvalidParameterCountException("Cos takes exactly one argument");
        }

        public override Term GetDerivative(string argument)
        {
            return (-1) * new Sin(Parameters[0]) * Parameters[0].GetDerivative(argument);
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            return this;
        }

        public override Term Approximate()
        {
            return this;
        }

        public override string ToLaTeX()
        {
            return GetSign() + $"\\cos({"{" + Parameters[0].ToLaTeX() + "}"})";
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Term Reduce()
        {
            return new Cos(IsAddInverse, IsMulInverse, Parameters[0]);
        }

        public override Term CheckAddReduce(Term t)
        {
            return null;
        }
    }
}
