using System.Text;

namespace Calq.Core
{
    public class Power : Function
    {
        public Power(params Term[] p) : base(FuncType.Power, p)
        {
            if (p.Length != 2)
                throw new InvalidParameterCountException("Power takes exactly two arguments");
        }

        public override Term GetDerivative(string argument)
        {
            Term f = Parameters[0];
            Term g = Parameters[1];
            return (f ^ (g - new Real(1))) * (g * f.GetDerivative(argument) + f * new Logarithm(f) * g.GetDerivative(argument));
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            return Parameters[0].Evaluate() ^ Parameters[1].Evaluate();
        }

        public override Term Approximate()
        {
            return Parameters[0].Approximate() ^ Parameters[1].Approximate();
        }

        public override string ToLaTeX()
        {
            return $@"({"{"+Parameters[0].ToLaTeX()+"}"}^{"{" + Parameters[1].ToLaTeX() + "}"})";
        }

        public override string ToPrefix()
        {
            return $"^[{Parameters[0].ToPrefix()},{Parameters[1].ToPrefix()}]";
        }

        public override string ToString()
        {
            return "(" + Parameters[0].ToPrefix() + ")^(" + Parameters[1].ToPrefix() + ")";
        }
        public override Term Reduce()
        {
            Term reducedBase = Parameters[0].Reduce();
            Term reducedExponent = Parameters[1].Reduce();
            if (reducedExponent.IsOne()) return reducedBase;

            Power arg0_parsed = reducedBase as Power;
            if (arg0_parsed != null)
            {
                return new Power(arg0_parsed.Parameters[0], arg0_parsed.Parameters[1] * reducedExponent).Reduce();
            }

            return new Power(reducedBase, reducedExponent);
        }
    }
}
