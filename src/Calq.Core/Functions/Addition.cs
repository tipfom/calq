using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Addition : Function
    {

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
            return ToString();
        }

        protected override string GetStringRep()
        {
            return "+";
        }
    }
}
