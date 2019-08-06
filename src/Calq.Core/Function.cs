using System.Collections.Generic;

namespace Calq.Core
{
    class Function : ITerm
    {
        public List<ITerm> Parameter;

        public ITerm Approximate()
        {
            throw new System.NotImplementedException();
        }

        public ITerm Evaluate()
        {
            throw new System.NotImplementedException();
        }
    }
}
