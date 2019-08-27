using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Addition : Function
    {
        public Addition(params Term[] p) : base(FuncType.Addition,false, false, p) { }
        public Addition(bool isAddInverse, bool isMulInverse, params Term[] p) : base(FuncType.Addition, isAddInverse, isMulInverse, p) { }

        public override Term Reduce()
        {
            List<Term> paras = new List<Term>();
            bool[] used = new bool[Parameters.Length];

            //inverse equvalent
            for(int i = 0; i < Parameters.Length; i++)
            {
                if (used[i]) continue;

                bool foundInverse = false;
                for(int j = i + 1; j < Parameters.Length; j++)
                {
                    if (used[j]) continue;

                    if (Parameters[i] == -Parameters[j])
                    {
                        foundInverse = true;
                        used[j] = true;
                        break;
                    }
                }

                if (!foundInverse)
                {
                    paras.Add(Parameters[i]);
                }
            }

            paras = paras.Select(x => x.Reduce()).Where(x => !x.IsZero()).ToList();

            //Multiple reals
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

        public override Term GetDerivative(string argument)
        {
            return new Addition(IsAddInverse, IsMulInverse, Parameters.Select(x => x.GetDerivative(argument)).ToArray());
        }

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
            string buffer = "";
            if (IsAddInverse) buffer += "-(";

            buffer += Parameters[0].ToLaTeX();
            for (int i = 1; i < Parameters.Length; i++)
            {
                if (Parameters[i].IsAddInverse)
                    buffer += "-" + (-Parameters[i]).ToLaTeX();
                else
                    buffer += "+" + Parameters[i].ToLaTeX();
            }

            if (IsAddInverse) buffer += ")";

            return buffer;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
