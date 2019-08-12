using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Multiplication : Function
    {
        public Multiplication(params Term[] p) : base(FuncType.Multiplication, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Multiplication needs at least 2 arguments");
        }
        public Multiplication(bool isAddInverse, bool isMultInverse, params Term[] p) : base(FuncType.Multiplication, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Multiplication needs at least 2 arguments");

            IsAddInverse = isAddInverse;
            IsMulInverse = isMultInverse;
        }

        public override Term GetDerivative(string argument)
        {
            List<Term> sums = new List<Term>();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Term r = Parameters[i];

                if (r.IsMulInverse)
                    r = r.GetDerivative(argument) / (r ^ 2);
                else
                    r = r.GetDerivative(argument);

                for (int j = 0; j < Parameters.Length; j++)
                {
                    if (i == j) continue;
                    r *= Parameters[j];
                }
                sums.Add(r);
            }

            return new Addition(sums.ToArray());
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
            List<Term> inverse = Parameters.Where(t => t.IsMulInverse).ToList();

            if (inverse.Count > 0)
            {
                List<Term> normal = Parameters.Where(t => !t.IsMulInverse).ToList();

                if(normal.Count == 0)
                    return $@"\frac{"{1}"}{"{" + string.Join(@"\cdot ", inverse.Select(t => t.ToLaTeX())) + "}"}";
                else
                    return $@"\frac{"{" + string.Join(@"\cdot ", normal.Select(t => t.ToLaTeX())) + "}"}{"{" + string.Join(@"\cdot ", inverse.Select(t => t.ToLaTeX())) + "}"}";
            }
            else
            {
                return "(" + string.Join(@"\cdot ", Parameters.Select(t => t.ToLaTeX())) + ")";
            }
        }

        public override string ToPrefix()
        {
            return "*[" + string.Join(",", Parameters.Select(x => x.IsMulInverse ? $"/[{x.ToPrefix()}]" : x.ToPrefix())) + "]";
        }

        public override string ToString()
        {
            string s = "";

            s += "(" + Parameters[0].ToString() + ")";
            for (int i = 1; i < Parameters.Length; i++)
            {
                if (Parameters[i].IsMulInverse)
                    s += "/(" + Parameters[i].ToString() + ")";
                else
                    s += "*(" + Parameters[i].ToString() + ")";
            }

            return s;
        }
        public override Term Reduce()
        {
            Term[] copy = Parameters.Select(x => x.Reduce()).Where(x => !x.IsOne()).ToArray();
            List<Term> paras = new List<Term>();

            bool[] used = new bool[copy.Length];
            //inverse equvalent
            for (int i = 0; i < copy.Length; i++)
            {
                if (used[i]) continue;
                if (copy[i].IsOne()) continue;

                bool foundInverse = false;
                for (int j = i + 1; j < copy.Length; j++)
                {
                    if (used[j]) continue;
                    Term b = copy[j].Clone();
                    b.IsMulInverse = !b.IsMulInverse;

                    if (copy[i] == b)
                    {
                        foundInverse = true;
                        used[j] = true;
                        break;
                    }
                }

                if (!foundInverse)
                {
                    paras.Add(copy[i]);
                }
            }

            //multiple reals
            List<Real> reals = paras.Where(x => x.GetType() == typeof(Real)).Cast<Real>().ToList();
            if (reals.Count > 1)
            {
                double product = 1;
                for (int i = 0; i < reals.Count; i++)
                {
                    if (reals[i].IsMulInverse)
                        product /= reals[i].Value;
                    else
                        product *= reals[i].Value;
                }

                paras = paras.Where(x => x.GetType() != typeof(Real)).ToList();
                if (product != 1) paras.Add(product);
            }

            if (paras.Where(x => x.IsZero()).Count() > 0)
                return 0;


            switch (paras.Count)
            {
                case 0: return 1;
                case 1: if (paras[0].IsMulInverse)
                        return new Multiplication(IsAddInverse, IsMulInverse, 1, paras[0]);
                    else
                        return paras[0];
                default: return new Multiplication(IsAddInverse, IsMulInverse, paras.ToArray());
            }
        }
    }
}
