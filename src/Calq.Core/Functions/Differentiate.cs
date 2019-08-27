using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    class Differentiate : Function
    {
        //Function, argument, N
        public Differentiate(params Term[] p) : base(FuncType.Differentiate, false, false, p)
        {
            if (!(p.Length == 2 || p.Length == 3))
                throw new InvalidParameterCountException("Differentiate takes two or three arguments");

            if(p[1].GetType() != typeof(Variable))
                throw new ArgumentException("The second argument of Differentiate needs to be a Variable");

            if(p.Length > 2)
            {
                if (p[2].GetType() != typeof(Real))
                    throw new ArgumentException("The second argument of Differentiate needs to be an Integer");
            }
        }
        public Differentiate(bool isAddInverse, bool isMulInverse, params Term[] p) : base(FuncType.Differentiate, isAddInverse, isMulInverse, p)
        {
            if (!(p.Length == 2 || p.Length == 3))
                throw new InvalidParameterCountException("Differentiate takes two or three arguments");

            if (p[1].GetType() != typeof(Variable))
                throw new ArgumentException("The second argument of Differentiate needs to be a Variable");

            if (p.Length > 2)
            {
                if (p[2].GetType() != typeof(Real))
                    throw new ArgumentException("The second argument of Differentiate needs to be an Integer");
            }
        }

        public override Term GetDerivative(string argument)
        {
            return Evaluate().GetDerivative(argument);
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            Term ret = Parameters[0];
            int n = 1;
            if (Parameters.Length == 3) n = (int)((Real)Parameters[2]).Value;
            for (int i = 0; i < n; i++)
                ret = ret.GetDerivative(Parameters[1].ToString());

            return ret;
        }

        public override Term Approximate()
        {
            return this;
        }

        public override string ToLaTeX()
        {
            if(Parameters.Length == 2)
                return GetSign() + @"\frac{d " + Parameters[0].ToLaTeX() + "}{d " + Parameters[1].ToLaTeX() + "}";
            else
                return GetSign() + @"\frac{d^{" + Parameters[2].ToLaTeX() + "} " + Parameters[0].ToLaTeX() + "}{d " + Parameters[1].ToLaTeX() + "^{" + Parameters[2].ToLaTeX() + "}}";

        }

        public override string ToString()
        {
            return base.ToString();
        }
        public override Term Reduce()
        {
            return new Differentiate(IsAddInverse, IsMulInverse, Parameters.Select(x => x.Reduce()).ToArray());
        }
    }
}
