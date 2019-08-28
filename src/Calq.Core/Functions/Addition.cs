using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Addition : Function
    {
        public Addition(params Term[] p) : base(FuncType.Addition, false, false, p) { }
        public Addition(bool isAddInverse, bool isMulInverse, params Term[] p) : base(FuncType.Addition, isAddInverse, isMulInverse, p) { }

        public override Term Reduce()
        {
            List<Term> paras = Parameters.Select(t => t.Reduce()).ToList();
            
            for(int i = 0; i< paras.Count; i++)
            {
                if(paras[i].GetType() == typeof(Addition))
                {
                    foreach(Term t in ((Addition)paras[i]).Parameters)
                    {
                        paras.Add(t);
                    }
                    paras.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < paras.Count - 1; i++)
            {
                for (int k = i + 1; k < paras.Count; k++)
                {
                    Term check = paras[i].CheckAddReduce(paras[k]);
                    if (check != null)
                    {
                        paras[i] = check;
                        paras.RemoveAt(k);
                        k--;
                        
                        if(check == 0)
                        {
                            paras.RemoveAt(i);
                            i--;
                        }
                    }
                }
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

        public override Term CheckAddReduce(Term t)
        {
            return null;
        }
    }
}
