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
            return $@"sin({"{" + Parameters[0].ToLaTeX() + "}"})";
        }

        public override string ToPrefix()
        {
            return $"sin[{Parameters[0].ToPrefix()}]";
        }

        public override string ToString()
        {
            return ToPrefix();
        }
        public override Term Reduce()
        {
            return new Sin(Parameters[0].Reduce());
        }
    }
}
