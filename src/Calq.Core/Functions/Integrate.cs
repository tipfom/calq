using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Integrate : Function
    {
        private bool HasLimits { get { return Parameters.Length == 4; } }

        public Integrate(params Term[] paras) : base(FuncType.Integrate, paras)
        {
        }

        public override Term Approximate()
        {
            return Evaluate();
        }

        public override Term Evaluate()
        {
            if (WebHelper.IsOnline)
            {
                string term;
                if (HasLimits)
                {
                    WebHelper.GetIntegral(Parameters[0].ToPrefix(), Parameters[0].GetVariableNames(), Parameters[1].ToString(), Parameters[2].ToPrefix(), Parameters[3].ToPrefix(), out term);
                }
                else
                {
                    WebHelper.GetIntegral(Parameters[0].ToPrefix(), Parameters[0].GetVariableNames(), Parameters[1].ToString(), out term);
                }
                return Term.Parse(term.Replace("**", "^").Replace("-oo", "ninf").Replace("oo", "pinf"));
            }
            return this;
        }

        public override Term GetDerivative(string argument)
        {
            if (Parameters[1].Reduce() == argument) return Parameters[0];

            if (HasLimits)
                return new Integrate(Parameters[0].GetDerivative(argument), Parameters[1], Parameters[2], Parameters[2]);
            else
                return new Integrate(Parameters[0].GetDerivative(argument), Parameters[1]);
        }

        public override Term Reduce()
        {
            if (HasLimits)
                return new Integrate(Parameters[0].Reduce(), Parameters[1].Reduce(), Parameters[2].Reduce(), Parameters[3].Reduce());
            else
                return new Integrate(Parameters[0].Reduce(), Parameters[1].Reduce());
        }

        public override string ToLaTeX()
        {
            if (HasLimits)
                return $@"\int_{"{" + Parameters[2].ToLaTeX() + "}"}^{"{" + Parameters[3].ToLaTeX() + "}"} {Parameters[0].ToLaTeX()} d{Parameters[1].ToLaTeX()}";
            else
                return $@"\int {Parameters[0].ToLaTeX()} d{Parameters[1].ToLaTeX()}";
        }

        public override string ToPrefix()
        {
            return $"int[{string.Join(",", Parameters.Select(p => p.ToPrefix()))}]";
        }
    }
}
