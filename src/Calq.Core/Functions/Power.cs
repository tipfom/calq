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

        public override Term Differentiate(string argument)
        {
            Term f = Parameters[0];
            Term g = Parameters[1];
            return (f ^ (g - new Real(1))) * (g * f.Differentiate(argument) + f * new Logarithm(f) * g.Differentiate(argument));
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
            return ToPrefix();
        }
    }
}
