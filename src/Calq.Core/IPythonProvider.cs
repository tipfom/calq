using System.Collections.Generic;

namespace Calq.Core
{
    public interface IPythonProvider
    {
        Term Integrate(Term expr, IEnumerable<string> usedSymbols, Term var);
        Term Integrate(Term expr, IEnumerable<string> usedSymbols, Term var, Term upperLimit, Term lowerLimit);
        Term Limit(Term expr, IEnumerable<string> usedSymbols, Term var, Term limit);    
    }
}
