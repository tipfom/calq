using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    interface ITerm
    {
        ITerm Evaluate();
        ITerm Approximate();
    }
}
