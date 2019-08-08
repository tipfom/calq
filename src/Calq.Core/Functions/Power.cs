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
            return Parameters[0] ^ Parameters[1] * (Parameters[1] * Parameters[0].Differentiate(argument) / Parameters[0] + new Logarithm(Parameters[0]) * Parameters[1].Differentiate(argument));
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
            return ToString();
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
