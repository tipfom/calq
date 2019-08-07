using System.Collections.Generic;
using System.Linq;
using MathNet.Symbolics;


namespace Calq.Core
{
    public class Function : Term
    {
        public enum Operators
        {
            Equals,
            Addition, Subtraktion,
            Multiplication, Division,
            Power,

            Sqrt, Log, Sin, Cos,

            //PythonDinger
            Lim, Int,
            Solve,

            Unknown
        }

        private static char[] InfixOperators = new char[]
        {
            '=',
            '+', '-',
            '*', '/',
            '^'
        };
        private static string[] PrefixOperators = new string[]
        {
            "sqrt", "log","sin", "cos",
            "lim", "int",
            "solve"
        };

        public readonly Operators Name;
        public readonly List<Term> Parameter;

        public Function(Operators name, List<Term> parameter)
        {
            Name = name;
            Parameter = parameter;
        }

        public static Function FunctionFromMixedString(string s)
        {
            List<Term> paras = new List<Term>();
            
            for (int i = 0; i < PrefixOperators.Length; i++)
            {
                bool possible = true;
                if (s.StartsWith(PrefixOperators[i]))
                {

                    int bracketDepth = 0;
                    for(int j = PrefixOperators[i].Length; j < s.Length - 1; j++)
                    {
                        if (s[j] == '(') bracketDepth++;
                        if (s[j] == ')') bracketDepth--;

                        if (bracketDepth == 0)
                        {
                            possible = false;
                            break;
                        }
                    }

                    if (possible)
                    {
                        s = s.Substring(PrefixOperators[i].Length);
                        s = s.Substring(1, s.Length - 2);

                        return new Function((Operators)(i + InfixOperators.Length), s.Split(',').Select(x => TermFromMixedString(x)).ToList());
                    }
                }

                if (!possible) break;
            }

            //infix-operators
            for (int i = 0; i < InfixOperators.Length; i++)
            {
                int bracketDepth = 0;
                List<int> pos = new List<int>();
                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == '(') bracketDepth++;
                    if (s[j] == ')') bracketDepth--;

                    if (s[j] == InfixOperators[i] && bracketDepth == 0)
                    {
                        pos.Add(j);
                    }
                }

                if (pos.Count > 0)
                {
                    pos.Insert(0, -1);
                    pos.Add(s.Length);

                    for (int j = 1; j < pos.Count; j++)
                    {
                       paras.Add(TermFromMixedString(s.Substring(pos[j - 1] + 1, pos[j] - pos[j - 1] - 1)));
                    }

                    return new Function((Operators)i, paras);
                }
            }

            if (s[0] == '(' && s[s.Length - 1] == ')')
            {
                return FunctionFromMixedString(s.Substring(1, s.Length - 2));
            }

            return null;
        }

        public static string ToPrefix(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);

            for(int i = 0; i < PrefixOperators.Length; i++)
            {
                if (s.StartsWith(PrefixOperators[i]))
                {
                    s = s.Substring(PrefixOperators[i].Length);
                    s = s.Substring(1, s.Length - 2);

                    return PrefixOperators[i] + "[" + string.Join(",", s.Split(',').Select(x => ToPrefix(x))) + "]";
                }
            }

            //infix-operators
            for (int i = 0; i < InfixOperators.Length; i++)
            {
                int bracketDepth = 0;
                List<int> pos = new List<int>();
                for(int j = 0; j < s.Length; j++)
                {
                    if (s[j] == '(') bracketDepth++;
                    if (s[j] == ')') bracketDepth--;

                    if (s[j] == InfixOperators[i] && bracketDepth == 0)
                    {
                        pos.Add(j);
                    }
                }

                if(pos.Count > 0)
                {
                    string buffer = InfixOperators[i].ToString() + "[";

                    pos.Insert(0, -1);
                    pos.Add(s.Length);
                    for(int j = 1; j < pos.Count; j++)
                    {
                        buffer += ToPrefix(s.Substring(pos[j - 1] + 1, pos[j] - pos[j - 1] - 1));

                        if (j < pos.Count - 1) buffer += ",";
                    }

                    buffer += "]";

                    return buffer;
                }
            }

            if (s[0] == '(' && s[s.Length - 1] == ')')
            {
                return ToPrefix(s.Substring(1, s.Length - 2));
            }

            return s;
        }

        public override Expression Evaluate()
        {
            Expression buffer;
            switch (Name)
            {
                case Operators.Equals:
                    //python stuff
                    return null;
                case Operators.Int:
                    // TODO:
                    // - bessere Unterstützung der Python Formattierung (** ist dort ^, pinf ist oo, ninf ist -oo)
                    // - unbekannte Funktionen zulassen oder alle von Python (inverse Gaus usw) hinzufügen, oder beides am besten
                    // - bessere Methode um alle benutzten Variablen in einem Term zu finden
                    if(Parameter.Count == 2)
                    {
                        string pExpr = WebHelper.GetIntegral(Parameter[0].ToString(), new List<string>() { "x" }, Parameter[1].ToString()).Replace("**","^");
                        return Term.TermFromMixedString(pExpr).Evaluate();
                    } else if (Parameter.Count == 4)
                    {

                        string pExpr = WebHelper.GetIntegral(Parameter[0].ToString(), new List<string>() { "x" }, Parameter[1].ToString(), Parameter[2].ToString(), Parameter[3].ToString()).Replace("**", "^"); ;
                        return Term.TermFromMixedString(pExpr).Evaluate();
                    }
                    return null;
                case Operators.Lim:
                    if (Parameter.Count == 3)
                    {
                        string pExpr = WebHelper.GetLimit(Parameter[0].ToString(), new List<string>() { "x" }, Parameter[1].ToString(), Parameter[2].ToString()).Replace("**", "^").Replace("-oo","ninf").Replace("oo","pinf");
                        return Term.TermFromMixedString(pExpr).Evaluate();
                    }
                    else if (Parameter.Count == 4)
                    {

                        string pExpr = WebHelper.GetLimit(Parameter[0].ToString(), new List<string>() { "x" }, Parameter[1].ToString(), Parameter[2].ToString(), Parameter[3].ToString()).Replace("**", "^").Replace("-oo", "ninf").Replace("oo", "pinf");
                        return Term.TermFromMixedString(pExpr).Evaluate();
                    }
                    return null;
                case Operators.Solve:
                    //python stuff
                    return null;

                case Operators.Addition:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer += Parameter[i].Evaluate();
                    return buffer;
                case Operators.Subtraktion:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer -= Parameter[i].Evaluate();
                    return buffer;
                case Operators.Multiplication:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer *= Parameter[i].Evaluate();
                    return buffer;
                case Operators.Division:
                    buffer = Parameter[0].Evaluate();
                    for(int i = 1; i < Parameter.Count; i++)
                        buffer /= Parameter[i].Evaluate();
                    return buffer;
                case Operators.Power:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer = Expression.Pow(buffer, Parameter[i].Evaluate());
                    return buffer;

                case Operators.Sqrt:
                    return Expression.Sqrt(Parameter[0].Evaluate());
                case Operators.Log:
                    if (Parameter.Count == 1) return Expression.Ln(Parameter[0].Evaluate());
                    else return Expression.Log(Parameter[0].Evaluate(), Parameter[1].Evaluate());
                case Operators.Sin:
                    return Expression.Sin(Parameter[0].Evaluate());
                case Operators.Cos:
                    return Expression.Cos(Parameter[0].Evaluate());

            }

            return null;
        }

        public override string ToString()
        {
            if ((int)Name < InfixOperators.Length)
                return InfixOperators[(int)Name] + "[" + string.Join(",", Parameter) + "]";
            else
                return PrefixOperators[((int)Name) - InfixOperators.Length] + "[" + string.Join(",", Parameter) + "]";
        }
    }
}
