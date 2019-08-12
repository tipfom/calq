using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Sin : Function
    {
        public Sin(params Term[] p) : base(FuncType.Sin, p)
        {
            if (p.Length != 1)
                throw new InvalidParameterCountException("Sin takes exactly one argument");
        }
        public Sin(bool isAddInverse, bool isMultInverse, params Term[] p) : base(FuncType.Sin, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Sin takes exactly one argument");

            IsAddInverse = isAddInverse;
            IsMulInverse = isMultInverse;
        }

        public override Term GetDerivative(string argument)
        {
            return new Cos(Parameters[0]) * Parameters[0].GetDerivative(argument);
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
            return GetSign() + $"\\sin({"{" + Parameters[0].ToLaTeX() + "}"})";
        }
        public override string ToString()
        {
            return base.ToString();
        }

        public override Term Reduce()
        {
            return new Sin(IsAddInverse, IsMulInverse, Parameters[0].Reduce());
        }
    }
}
