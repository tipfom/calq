using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Addition : Function
    {
        public Addition(params Term[] p) : base(FuncType.Addition, p) { }
        public Addition(bool isAddInverse, bool isMulInverse, params Term[] p) : base(FuncType.Addition, p)
        {
            IsAddInverse = isAddInverse;
            IsMulInverse = isMulInverse;
        }

        public override Term Reduce()
        {
            List<Term> paras = Parameters.Select(x => x.Reduce()).Where(x => !x.IsZero()).ToList();

            List<Real> reals = paras.Where(x => x.GetType() == typeof(Real)).Cast<Real>().ToList();
            if (reals.Count > 1)
            {
                double sum = 0;
                for (int i = 0; i < reals.Count; i++)
                {
                    if(reals[i].IsAddInverse)
                        sum -= reals[i].Value;
                    else
                        sum += reals[i].Value;
                }
                    
                paras = paras.Where(x => x.GetType() != typeof(Real)).ToList();
                if(sum != 0) paras.Add(sum);
            }

            switch (paras.Count)
            {
                case 0: return 0;
                case 1: return paras[0];
                default: return new Addition(IsAddInverse, IsMulInverse, paras.ToArray());
            }
        }

        public override Term Differentiate(string argument)
        {
            return new Addition(IsAddInverse, IsMulInverse, Parameters.Select(x => x.Differentiate(argument)).ToArray());
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            return this;
        }

        public override Term Approximate()
        {
            return this;
        }

        public override string ToLaTeX()
        {
            return string.Join(" + ", Parameters.Select(x => x.ToLaTeX()));
        }

        public override string ToPrefix()
        {
            StringBuilder builder = new StringBuilder();
            void append(Term t)
            {
                // TODO: hässlich, besser support mit nur einem Argument
                if (t.IsAddInverse) builder.Append("-[0," + t.ToPrefix() + "]");
                else builder.Append(t.ToPrefix());
            };

            builder.Append("+[");
            append(Parameters[0]);
            for (int i = 1; i < Parameters.Length; i++)
            {
                builder.Append(",");
                append(Parameters[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }

        public override string ToString()
        {
            return ToPrefix();
        }

    }
}
