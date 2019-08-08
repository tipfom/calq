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
            throw new NotImplementedException();
        }

        public override string ToLaTeX()
        {
            throw new NotImplementedException();
        }

        public override string ToPrefix()
        {
            return "ln[" + Parameters[0] + "]";
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
