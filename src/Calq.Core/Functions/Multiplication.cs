using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Multiplication : Function
    {
        public Multiplication(params Term[] p) : base(FuncType.Subtraction, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Multiplication needs at least 2 arguments");
        }

        public override Term Differentiate(string argument)
        {
            Term sum = Parameters[0].Differentiate(argument);
            for (int i = 1; i < Parameters.Length; i++)
                sum *= Parameters[i].Differentiate(argument);
            return sum;
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            Term sum = Parameters[0];

            for (int i = 1; i < Parameters.Length; i++)
                sum *= Parameters[i].Evaluate();
            return sum;
        }
        public override Term Approximate()
        {
            Term sum = Parameters[0];

            for (int i = 1; i < Parameters.Length; i++)
                sum *= Parameters[i].Approximate();
            return sum;
        }

        public override string ToLaTeX()
        {
            return ToString();
        }

        protected override string GetStringRep()
        {
            return "*";
        }
    }
}
