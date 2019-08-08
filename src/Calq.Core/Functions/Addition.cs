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
            return Parameters[0].Evaluate() + Parameters[1].Evaluate();
        }
        public override Term Approximate()
        {
            return Parameters[0].Evaluate() + Parameters[1].Evaluate();
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
