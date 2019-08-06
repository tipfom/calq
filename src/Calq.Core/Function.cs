using System.Collections.Generic;
using System.Linq;

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

            Ln, Sin, Cos,

            Int,
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
            "ln","sin", "cos", "int", "solve"
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
            System.Diagnostics.Debug.WriteLine(s);

            for (int i = 0; i < PrefixOperators.Length; i++)
            {
                if (s.StartsWith(PrefixOperators[i]))
                {
                    s = s.Substring(PrefixOperators[i].Length);
                    s = s.Substring(1, s.Length - 2);

                    return new Function((Operators)(i + InfixOperators.Length), s.Split(',').Select(x => TermFromMixedString(x)).ToList());
                }
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

        public override Term Evaluate()
        {
            throw new System.NotImplementedException();
        }

        public override Term Approximate()
        {
            throw new System.NotImplementedException();
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
