using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Logarithm : Function
    {
        public Logarithm(params Term[] p) : base(FuncType.Log, p)
        {
        }

        public override Term Approximate()
        {
            throw new NotImplementedException();
        }

        public override Term Differentiate(string argument)
        {
            throw new NotImplementedException();
        }

        public override Term Evaluate()
        {
            throw new NotImplementedException();
        }

        public override string ToLaTeX()
        {
            throw new NotImplementedException();
        }

        public override string ToPrefix()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
