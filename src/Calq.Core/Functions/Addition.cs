using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Addition : Function
    {
        public Addition(params Term[] p) : base(FuncType.Addition, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Addition needs at least 2 arguments");
        }

        public override Term Differentiate(string argument)
        {
            return Parameters[0].Differentiate(argument) + Parameters[1].Differentiate(argument);
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            Term sum = Parameters[0];

            for (int i = 1; i < Parameters.Length; i++)
                sum += Parameters[i].Evaluate();
            return sum;
        }
        public override Term Approximate()
        {
            Term sum = Parameters[0];

            for (int i = 1; i < Parameters.Length; i++)
                sum += Parameters[i].Approximate();
            return sum;
        }

        public override string ToLaTeX()
        {
            return ToString();
        }

        protected override string GetStringRep()
        {
            return "+";
        }
    }
}
