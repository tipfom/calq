using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Integrate : Function
    {
        private bool HasLimits { get { return Parameters.Length == 4; } }

        //fuction, argument, a, b
        public Integrate(params Term[] p) : base(FuncType.Integrate, false, false, p)
        {
            if (!(p.Length == 2 || p.Length == 4))
                throw new InvalidParameterCountException("Integrate takes two or four arguments");

            if (p[1].GetType() != typeof(Variable))
                throw new ArgumentException("The second argument of Inegrate needs to be a Variable");
        }

        public Integrate(bool isAddInverse, bool isMulInverse, params Term[] p) : base(FuncType.Integrate, isAddInverse, isMulInverse, p)
        {
            if (!(p.Length == 2 || p.Length == 4))
                throw new InvalidParameterCountException("Integrate takes two or four arguments");

            if (p[1].GetType() != typeof(Variable))
                throw new ArgumentException("The second argument of Inegrate needs to be a Variable");
        }

        public override Term Approximate()
        {
            return Evaluate();
        }

        public override Term Evaluate()
        {
            if (HasLimits)
            {
                Term r = PlatformPythonProvider.Integrate(Parameters[0], Parameters.SelectMany(t => t.GetVariableNames()), Parameters[1], Parameters[2], Parameters[3]);
                if (r != null) return r;
                return this;
            }
            else
            {
                Term r = PlatformPythonProvider.Integrate(Parameters[0], Parameters.SelectMany(t => t.GetVariableNames()), Parameters[1]);
                if (r != null) return r;
                return this;
            }
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
            return new Integrate(IsAddInverse, IsMulInverse, Parameters.Select(x => x.Reduce()).ToArray());
        }

        public override string ToLaTeX()
        {
            if (HasLimits)
                return GetSign() + $@"\int_{"{" + Parameters[2].ToLaTeX() + "}"}^{"{" + Parameters[3].ToLaTeX() + "}"} {Parameters[0].ToLaTeX()} d{Parameters[1].ToLaTeX()}";
            else
                return GetSign() + $@"\int {Parameters[0].ToLaTeX()} d{Parameters[1].ToLaTeX()}";
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
