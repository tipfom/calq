using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core.Python
{
    interface IPythonProvider
    {
        string Evaluate(string expression);
    }
}
